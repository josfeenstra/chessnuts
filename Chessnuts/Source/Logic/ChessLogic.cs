using Nez;
using Raven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chessnuts
{
    enum Occupancy
    {
        OutOfBounds = 0,
        Empty = 1,
        Enemy = 2,
        Friendly = 3
    }

    enum CheckStatus
    {
        None,
        Check,
        CheckMate,
        StaleMate,
    }

    /// <summary>
    /// Static method for calculating al sorts of chess related actions
    /// 
    /// This is mainly made because ChessBoard started to become quite big, and these operations can be seen as separate things
    /// </summary>
    static class ChessLogic
    {
        #region Debug

        public  static int BoardStatesChecked { get => _BoardStatesChecked; }
        private static int _BoardStatesChecked = 0;

        #endregion

        #region Vectors

        private static Point2 up = new Point2(0, -1);
        private static Point2 down = new Point2(0, 1);
        private static Point2 left = new Point2(-1, 0);
        private static Point2 right = new Point2(1, 0);

        private static Point2[] cardinals = new Point2[4] {
            up,down,left,right
        };

        private static Point2[] diagonals = new Point2[4] {
            up + left,
            up + right,
            down + left,
            down + right
        };

        private static Point2[] all = new Point2[8] {
            up, down, left, right,
            up + left, up + right, down + left, down + right
        };

        private static Point2[] horseJumps = new Point2[8] {
            new Point2(2, 1),
            new Point2(2, -1),
            new Point2(-2, 1),
            new Point2(-2, -1),
            new Point2(1, 2),
            new Point2(1, -2),
            new Point2(-1, 2),
            new Point2(-1, -2)
        };

        #endregion

        #region Private

        private static void GetPawnMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            Insist.IsTrue(piece.kind == PieceKind.Pawn);

            // make some things based upon the color
            Point2 vector = (piece.color == PieceColor.White) ? up : down;
            int startRow = (piece.color == PieceColor.White) ? SD.WHITE_PAWN_STARTING_ROW : SD.BLACK_PAWN_STARTING_ROW;
            int promoteRow = (piece.color == PieceColor.White) ? SD.WHITE_PAWN_PROMOTE_ROW : SD.BLACK_PAWN_PROMOTE_ROW;


            var pawnMoves = new Point2[4] {
                piece.point + vector,
                piece.point + vector + vector,
                piece.point + vector + left,
                piece.point + vector + right
            };

            ChessPiece occupant;
            if (grid.TryGet(pawnMoves[0], out occupant) && occupant == ChessPiece.Empty)
                AddOrPromote(grid, new ChessMove(piece, pawnMoves[0]), legalChecking, promoteRow, ref moves);

            if (piece.point.Y == startRow && grid.TryGet(pawnMoves[1], out occupant) && occupant == ChessPiece.Empty)
                AddOrPromote(grid, new ChessMove(piece, pawnMoves[1]), legalChecking, promoteRow, ref moves);
   
            if (grid.TryGet(pawnMoves[2], out occupant) && occupant != ChessPiece.Empty && occupant.color != piece.color)
                AddOrPromote(grid, new ChessMove(piece, occupant), legalChecking, promoteRow, ref moves);

            if (grid.TryGet(pawnMoves[3], out occupant) && occupant != ChessPiece.Empty && occupant.color != piece.color)
                AddOrPromote(grid, new ChessMove(piece, occupant), legalChecking, promoteRow, ref moves);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddOrPromote(Grid2D<ChessPiece> grid, ChessMove move, bool legalChecking, int promoteRow, ref List<ChessMove> moves)
        {
            // check if the pawn will arrive at the end row. if so, flag it with a promote MoveKind
            if (move.toPiece.point.Y == promoteRow)
                AddIfLegal(grid, new ChessMove(move.fromPiece, move.toPiece, MoveKind.Promote), legalChecking, ref moves);
            else
                AddIfLegal(grid, move, legalChecking, ref moves);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetKingMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            foreach (Point2 vector in all)
            {
                AddMoveOrCapture(grid, piece, piece.point + vector, ref moves, legalChecking);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetQueenMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            foreach (Point2 vector in all)
            {
                KeepAdding(grid, piece, piece.point, vector, ref moves, legalChecking);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetBishopMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            foreach (Point2 vector in diagonals)
            {
                KeepAdding(grid, piece, piece.point, vector, ref moves, legalChecking);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetRookMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            foreach (Point2 vector in cardinals)
            {
                KeepAdding(grid, piece, piece.point, vector, ref moves, legalChecking);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetKnightMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking)
        {
            foreach (Point2 vector in horseJumps)
            {
                AddMoveOrCapture(grid, piece, piece.point + vector, ref moves, legalChecking);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void KeepAdding(Grid2D<ChessPiece> grid, ChessPiece piece, Point2 moveTo, Point2 vector, ref List<ChessMove> moves, bool legalChecking)
        {
            moveTo += vector;
            if (AddMoveOrCapture(grid, piece, moveTo, ref moves, legalChecking))
                KeepAdding(grid, piece, moveTo, vector, ref moves, legalChecking);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AddMoveOrCapture(Grid2D<ChessPiece> grid, ChessPiece piece, Point2 moveTo, ref List<ChessMove> moves, bool legalChecking)
        {
            // TODO add "setting check is not allowed"
            if (!grid.TryGet(moveTo, out ChessPiece occupant))
            {
                // out of bounds
                return false;
            }
            if (occupant == ChessPiece.Empty)
            {
                // friendly
                AddIfLegal(grid, new ChessMove(piece, moveTo), legalChecking, ref moves); 
                return true;
            }     
            if (occupant.color != piece.color)
            {
                // enemy
                AddIfLegal(grid, new ChessMove(piece, occupant), legalChecking, ref moves);
                return false;
            }

            // friendly block 
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddIfLegal(Grid2D<ChessPiece> grid, ChessMove move, bool legalChecking, ref List<ChessMove> moves)
        {
            if (legalChecking)
            {
                if (IsLegal(grid, move))
                    moves.Add(move);
            }
            else
            {
                moves.Add(move);
            }
        }

        // check if this move is legal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLegal(Grid2D<ChessPiece> grid, ChessMove move)
        {
            // get my color and the other color (s)
            var myColor = move.fromPiece.color;
            var enemyColor = myColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // apply move, check if check, and turn back 
            move.Apply(ref grid);
            var allEnemyMoves = GetMoves(grid, enemyColor, false);
            bool check = !allEnemyMoves.TrueForAll(x => x.kind != MoveKind.CaptureKing);
            move.Revert(ref grid);

            // legal move if not check
            return !check;
        }

        #endregion

        #region Public Move Getters

        public static void GetMoves(Grid2D<ChessPiece> grid, ChessPiece piece, ref List<ChessMove> moves, bool legalChecking = false)
        {
            switch (piece.kind)
            {
                case PieceKind.Pawn:
                    ChessLogic.GetPawnMoves(grid, piece, ref moves, legalChecking);
                    break;
                case PieceKind.Rook:
                    ChessLogic.GetRookMoves(grid, piece, ref moves, legalChecking);
                    break;
                case PieceKind.Knight:
                    ChessLogic.GetKnightMoves(grid, piece, ref moves, legalChecking);
                    break;
                case PieceKind.Bishop:
                    ChessLogic.GetBishopMoves(grid, piece, ref moves, legalChecking);
                    break;
                case PieceKind.Queen:
                    ChessLogic.GetQueenMoves(grid, piece, ref moves, legalChecking);
                    break;
                case PieceKind.King:
                    ChessLogic.GetKingMoves(grid, piece, ref moves, legalChecking);
                    break;
            }
        }

        public static List<ChessMove> GetMoves(Grid2D<ChessPiece> grid, ChessPiece piece, bool legalChecking = false)
        {
            var moves = new List<ChessMove>();
            GetMoves(grid, piece, ref moves, legalChecking);
            return moves;
        }

        public static List<ChessMove> GetMoves(Grid2D<ChessPiece> grid, List<ChessPiece> pieces, bool legalChecking = false)
        {
            var moves = new List<ChessMove>();
            foreach (var piece in pieces) GetMoves(grid, piece, ref moves, legalChecking);
            return moves;
        }

        public static List<ChessMove> GetMoves(Grid2D<ChessPiece> grid, PieceColor color, bool legalChecking = false)
        {
            var moves = new List<ChessMove>();
            foreach (var piece in GetAllPieces(grid, color)) GetMoves(grid, piece, ref moves, legalChecking);
            return moves;
        }

        public static List<ChessPiece> GetAllPieces(Grid2D<ChessPiece> grid, PieceColor color)
        {
            return grid.Where(x => x.color == color).ToList();
        }

        /// <param name="grid"> </param>
        /// <param name="color"> Color whose turn it is right now </param>
        public static CheckStatus CheckForCheck(Grid2D<ChessPiece> grid, PieceColor color, PieceColor colorEnemy)
        {
            var allEnemyMoves = GetMoves(grid, colorEnemy);
            bool IsCheck = !allEnemyMoves.TrueForAll(x => x.kind != MoveKind.CaptureKing);

            var allMoves = GetMoves(grid, color, true);
            bool noMoves = (allMoves.Count == 0);

            if (IsCheck)
                if (noMoves)
                    return CheckStatus.CheckMate;
                else
                    return CheckStatus.Check;
            else
                if (noMoves)
                    return CheckStatus.StaleMate;
                else
                    return CheckStatus.None;
        }

        #endregion

        #region Move Application and Resetting

        public static void ApplyMove(ref Grid2D<ChessPiece> grid, ChessMove move)
        {
            _BoardStatesChecked += 1;
            move.Apply(ref grid);
        }

        public static void RevertMove(ref Grid2D<ChessPiece> grid, ChessMove move)
        {
            move.Revert(ref grid);
        }

        #endregion

        #region Public State Calculation

        public static float GetStateValue(Grid2D<ChessPiece> grid, PieceColor color)
        {
            var pieces = GetAllPieces(grid, color);
            float score = 0.0f;
            foreach (ChessPiece piece in pieces)
            {
                score += piece.score;
            }
            return score;
        }

        #endregion
        #region AI

        /// <summary>
        /// The current best move for the player 
        /// Depth = how many moves to think ahead
        /// </summary>
        public static ChessMove GetBestMove(Grid2D<ChessPiece> grid, PieceColor color, int MovesToThinkAhead, bool isCheck, out float highScore)
        {
            // semantics, and get all moves 
            highScore = -1000f;
            int depth = MovesToThinkAhead; // semantics
            var moves = GetMoves(grid, color, isCheck);
            if (moves.Count == 0) return ChessMove.None;

            // sort away the best ones
            float[] scores = new float[moves.Count];      
            List<int> bests = new List<int>();
            for (int i = 0; i < moves.Count; i++)
            {
                scores[i] = CalculateDeepScore(grid, moves[i], color, depth);
                if (scores[i] == highScore)
                {
                    bests.Add(i);
                }

                if (scores[i] > highScore)
                {
                    bests = new List<int>();
                    bests.Add(i);
                    highScore = scores[i];
                }

                // quit early if it is good enough
                if (highScore > 10f)
                {
                    break;
                }
            }

            // choose a random best one
            int choice = Nez.Random.Range(0, bests.Count);
            int best = bests[choice];

            // return it
            ChessMove bestMove = moves[best];
            return bestMove;
        }

        /// <summary>
        /// Return how much value difference you gain by doing this move. 
        /// Can also be used to compell the computer to do something. 
        /// </summary>
        /// <returns></returns>
        public static float CalculateScore(ChessMove move)
        {
            switch (move.kind)
            {
                case MoveKind.None:
                    return 0f;

                case MoveKind.Move:
                    return 0f; // TODO calc positional score gain 

                case MoveKind.Capture:
                    return move.toPiece.score;

                case MoveKind.ShortCastle:
                    return 0.5f;

                case MoveKind.LongCastle:
                    return 0.5f;

                case MoveKind.Promote:
                    return 8f + move.toPiece.score;

                case MoveKind.EnPassant:
                    return move.toPiece.score;

                case MoveKind.CaptureKing:
                    return 100f;

                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Return how much value difference you gain by doing this move. 
        /// Can also be used to compell the computer to do something. 
        /// </summary>
        /// <returns></returns>
        private static float CalculateDeepScore(Grid2D<ChessPiece> grid, ChessMove move, PieceColor color, int depth)
        {
            // base case : just return the score of this move
            if (depth < 1)
            {
                return CalculateScore(move);
            }

            // recurse : return the score of this move, PLUS the consequences of this move
            float score = CalculateScore(move);
            ApplyMove(ref grid, move);
            ChessMove bestOpponentMove = GetBestMove(grid, color.Next(), depth - 1, false, out float opponentScoreGain);  
            RevertMove(ref grid, move);
            return score - opponentScoreGain; ;
        }

        #endregion
    }
}