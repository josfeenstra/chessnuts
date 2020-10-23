using Microsoft.Xna.Framework;
using Nez;
using Raven;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chessnuts
{
    /// <summary>
    /// This is the TRUE logical chessboard
    /// </summary>
    class ChessBoard
    {
        public Grid2D<ChessPiece> grid;
        public Stack<ChessMove> history = new Stack<ChessMove>();
        // PieceColor initiative = ...

        public ChessBoard()
        {
            grid = new Grid2D<ChessPiece>(new Vector2(SD.TILE_SIZE, SD.TILE_SIZE), SD.BOARD_WIDTH, SD.BOARD_HEIGHT, SD.TILE_SIZE);
            grid.Fill(ChessPiece.Empty);
        }

        public ChessBoard(Grid2D<ChessPiece> grid)
        {
            this.grid = grid;
        }


        public ChessBoard Copy()
        {
            return new ChessBoard(grid.Copy());
        }

        /// <summary>
        /// // TODO optimize this
        /// </summary>
        public List<ChessPiece> GetPieces(PieceColor owner)
        {        
            List<ChessPiece> pieces = new List<ChessPiece>();
            foreach (var point in grid.IteratePoints())
            {
                var piece = grid.Get(point);
                if (piece.color == owner) pieces.Add(piece);
            }
            return pieces;
        }

        public bool CheckAndApplyPlay(ChessMove play)
        {
            if (CheckPlay(play))
            {
                ApplyPlay(play);
                return true;
            }
            return false;
        }

        private bool CheckPlay(ChessMove play)
        {
            // TODO make more efficient
            var checkPlays = GetPossibleMoves(play.fromPiece);

            // debug
            // Debug.Log(play.ToString());
            // foreach (var checkPlay in checkPlays) Debug.Log(checkPlay.ToString());        

            if (checkPlays.Contains(play))
            {
                return true;
            }

            return false;
        }

        public void ApplyPlay(ChessMove play)
        {
            history.Push(play);
            play.Apply(ref grid);
        }

        public void Undo()
        {
            var play = history.Pop();
            play.Revert(ref grid);
        }

        public void CreatePiece(Point2 point, PieceKind kind, PieceColor owner)
        {
            grid.Set(point, new ChessPiece(point, kind, owner));
        }

        #region ChessLogic Passtrough

        public List<ChessMove> GetPossibleMoves(ChessPiece piece)
        {
            return ChessLogic.GetMoves(grid, piece, true);
        }

        public CheckStatus CheckForCheck(PieceColor friendly, PieceColor enemy)
        {
            return ChessLogic.CheckForCheck(grid, friendly, enemy);
        }

        public float GetValue(PieceColor color)
        {
            return ChessLogic.GetStateValue(grid, color);
        }

        public async Task<ChessMove> GetBestMoveAsync(PieceColor color, int depth, bool isCheck)
        {
            int boardstates = ChessLogic.BoardStatesChecked;
            ChessMove bestMove = await Task.Run(() => ChessLogic.GetBestMove(grid, color, depth, isCheck, out float highscore));
            Debug.Log(string.Format("evalulated {0} boardstates", ChessLogic.BoardStatesChecked - boardstates));
            return bestMove;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void InitializeClassicSetup()
        {
            CreatePiece(new Point2(0, 0), PieceKind.Rook, PieceColor.Black);
            CreatePiece(new Point2(1, 0), PieceKind.Knight, PieceColor.Black);
            CreatePiece(new Point2(2, 0), PieceKind.Bishop, PieceColor.Black);
            CreatePiece(new Point2(3, 0), PieceKind.Queen, PieceColor.Black);
            CreatePiece(new Point2(4, 0), PieceKind.King, PieceColor.Black);
            CreatePiece(new Point2(5, 0), PieceKind.Bishop, PieceColor.Black);
            CreatePiece(new Point2(6, 0), PieceKind.Knight, PieceColor.Black);
            CreatePiece(new Point2(7, 0), PieceKind.Rook, PieceColor.Black);

            // create the board
            for (int i = 0; i < 8; i++)
            {
                CreatePiece(new Point2(i, 1), PieceKind.Pawn, PieceColor.Black);
            }


            // create the board
            for (int i = 0; i < 8; i++)
            {
                CreatePiece(new Point2(i, 6), PieceKind.Pawn, PieceColor.White);
            }

            CreatePiece(new Point2(0, 7), PieceKind.Rook, PieceColor.White);
            CreatePiece(new Point2(1, 7), PieceKind.Knight, PieceColor.White);
            CreatePiece(new Point2(2, 7), PieceKind.Bishop, PieceColor.White);
            CreatePiece(new Point2(3, 7), PieceKind.Queen, PieceColor.White);
            CreatePiece(new Point2(4, 7), PieceKind.King, PieceColor.White);
            CreatePiece(new Point2(5, 7), PieceKind.Bishop, PieceColor.White);
            CreatePiece(new Point2(6, 7), PieceKind.Knight, PieceColor.White);
            CreatePiece(new Point2(7, 7), PieceKind.Rook, PieceColor.White);
        }


        public override string ToString()
        {
            List<string> texts = new List<string>();
            foreach (ChessPiece piece in grid.Iterate())
            {
                texts.Add(piece.ToString());
            }
            return string.Join(" ", texts);
        }
    }
}
