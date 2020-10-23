using Raven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Nez;

namespace Chessnuts
{
    enum MoveKind
    {
        None,
        Move,
        Capture,
        ShortCastle,
        LongCastle,
        Promote,
        EnPassant,

        CaptureKing, // This is used for checkmate checking
    }

    // represents a (requested) move in chess
    struct ChessMove
    {
        public static ChessMove None = new ChessMove(ChessPiece.Empty, ChessPiece.Empty, MoveKind.None);
        public readonly ChessPiece fromPiece, toPiece;
        public readonly MoveKind kind;

        #region Constructors

        /// <summary>
        /// Flagged as a "movement" move
        /// 
        /// </summary>
        public ChessMove(ChessPiece piece, Point2 to)
        {
            this.fromPiece = piece;
            this.toPiece = new ChessPiece(to, PieceKind.Empty, PieceColor.Misc);
            this.kind = MoveKind.Move;
        }

        /// <summary>
        /// Flagged as a "Capture" move
        /// if ChessPiece is king, this is flagged as a "Capture King" move for CheckMate logic 
        /// </summary>
        public ChessMove(ChessPiece piece, ChessPiece occupant)
        {
            this.fromPiece = piece;
            if (occupant == ChessPiece.Empty) throw new Exception("attempting to 'capture' noting..");
            this.toPiece = occupant;
            if (occupant.kind == PieceKind.King)
                this.kind = MoveKind.CaptureKing;
            else
                this.kind = MoveKind.Capture;
        }

        /// <summary>
        /// Flagged however you like it
        /// </summary>
        public ChessMove(ChessPiece piece, ChessPiece occupant, MoveKind kind)
        {
            this.fromPiece = piece;
            this.toPiece = occupant;
            this.kind = kind;
        }

        #endregion
        #region Activation


        /// <summary>
        /// Apply the move to a boardstate. grid is NOT copied.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Apply(ref Grid2D<ChessPiece> grid)
        {
            switch (kind)
            {
                case MoveKind.None:
                    break;

                case MoveKind.Move:
                case MoveKind.Capture:
                    grid.Set(fromPiece.point, ChessPiece.Empty);
                    grid.Set(toPiece.point, new ChessPiece(toPiece.point, fromPiece.kind, fromPiece.color));
                    break;

                case MoveKind.ShortCastle:
                    break;
                case MoveKind.LongCastle:
                    break;

                case MoveKind.Promote:
                    grid.Set(fromPiece.point, ChessPiece.Empty);
                    grid.Set(toPiece.point, new ChessPiece(toPiece.point, PieceKind.Queen, fromPiece.color));
                    break;

                case MoveKind.EnPassant:

                    break;

                case MoveKind.CaptureKing:
                    //throw new Exception("this should never happen");
                    break;
            }
        }

        /// <summary>
        /// Return a grid with this move played in reverse. grid is NOT copied.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Revert(ref Grid2D<ChessPiece> grid)
        {
            switch (kind)
            {
                case MoveKind.None:
                    break;

                case MoveKind.Move:
                    grid.Set(toPiece.point, ChessPiece.Empty);
                    grid.Set(fromPiece.point, fromPiece);
                    break;
                case MoveKind.Capture:
                    grid.Set(fromPiece.point, fromPiece);
                    grid.Set(toPiece.point, toPiece);
                    break;

                case MoveKind.ShortCastle:
                    break;
                case MoveKind.LongCastle:
                    break;

                case MoveKind.Promote:
                    if (toPiece == ChessPiece.Empty)
                        grid.Set(toPiece.point, ChessPiece.Empty);
                    else
                        grid.Set(toPiece.point, toPiece);
                    grid.Set(fromPiece.point, fromPiece);
                    break;

                case MoveKind.EnPassant:

                    break;

                case MoveKind.CaptureKing:
                    // Debug.Log("this should never happen!!!!");
                    // throw new Exception("this should never happen");
                    break;
            }
        }


        #endregion
        #region Evaluation

        public bool IsValid(ChessBoard board)
        {
            if (kind == MoveKind.None) return false;
            return true;
        }

        public string ToClassicRep()
        {
            return "TODO";
        }

        public override string ToString()
        {
            return kind.ToString() + " | " + fromPiece.ToString() + " at " + fromPiece.point.ToString() + " to " + toPiece.point.ToString();
        }

        public bool IsLike(ChessMove other)
        {
            return this.kind == other.kind;
        }


        #endregion
    }
}
