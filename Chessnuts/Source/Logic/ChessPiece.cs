using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;
using Nez.Textures;
using Nez.Sprites;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Raven;
using System.Runtime.CompilerServices;

namespace Chessnuts
{
    enum PieceKind
    {
        Empty,
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    enum PieceColor
    {
        White,
        Black,
        Misc
    }

    static class PieceColorExtensions
    {
        public static PieceColor Next(this PieceColor color)
        {
            switch (color)
            {
                case PieceColor.White:
                    return PieceColor.Black;
                case PieceColor.Black:
                    return PieceColor.White;
                default:
                    return PieceColor.Misc;
            }
        }
    }

    /// <summary>
    /// A thing on the board.
    /// Just a data type. No functionality!
    /// </summary>
    struct ChessPiece
    {
        public static ChessPiece Empty = new ChessPiece(Point2.Zero, PieceKind.Empty, PieceColor.Misc);

        public Point2 point;
        public PieceKind kind;
        public PieceColor color;
        public float score;
        public float posScore;

        public ChessPiece(Point2 point, PieceKind kind, PieceColor color)
        {
            this.point = point;
            this.kind = kind;
            this.color = color;
            this.score = CalcScore(kind);
            this.posScore = CalcScore(kind);
        }

        #region Scores

        private static float CalcScore(PieceKind kind)
        {
            switch (kind)
            {
                case PieceKind.Empty:
                    return 0;
                case PieceKind.Pawn:
                    return 1;
                case PieceKind.Rook:
                    return 5;
                case PieceKind.Knight:
                    return 3;
                case PieceKind.Bishop:
                    return 3.25f;
                case PieceKind.Queen:
                    return 9f;
                case PieceKind.King:
                    return 100f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Get advanced score based upon hardcoded positioning ideas
        /// </summary>
        /// <returns></returns>
        public float CalcPosScore(List<ChessMove> movesInNewPosition)
        {
            // TODO
            switch (kind)
            {
                case PieceKind.Empty:
                    break; // EMPTY SPOTS CAN HAVE A VALUE -> outposts
                case PieceKind.Pawn:
                    break; 
                case PieceKind.Rook:
                    break; // MORE VALUABLE ON OPEN FILES, MOVE VALUABLE BASED UPON THINGS IT CAN DO
                case PieceKind.Knight:
                    break; // MORE VALUABLE BASED UPON HOW MANY MOVES IT HAS 
                case PieceKind.Bishop:
                    break; // MORE VALUABLE BASED UPON HOW MANY MOVES IT HAS 
                case PieceKind.Queen:
                    break; // ALWAYS INSANELY VALUABLE 
                case PieceKind.King:
                    break; // INFINITE VALUABLE
            }
            return CalcScore(kind);
        }

        /// <summary>
        /// MORE VALUABLE THE SOONER IT CAN PROMOTE 
        /// </summary>
        private float GetPawnScore()
        {
            int stepsUntilPromotion;
            if (color == PieceColor.White)
                stepsUntilPromotion = Math.Abs(SD.WHITE_PAWN_PROMOTE_ROW - point.Y);
            else // (color == PieceColor.Black)
                stepsUntilPromotion = Math.Abs(SD.BLACK_PAWN_PROMOTE_ROW - point.Y);

            return 1.0f;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return color.ToString() + " " + kind.ToString();
        }

        /// <summary>
        /// NOTE : DOES NOT HAVE TO BE ON THE SAME POINT 
        /// </summary>
        public static bool operator==(ChessPiece self, ChessPiece other)
        {
            return (self.kind == other.kind && self.color == other.color);
        }
        public static bool operator !=(ChessPiece self, ChessPiece other)
        {
            return (self.point != other.point || self.kind != other.kind || self.color != other.color);
        }

        public static ChessPiece operator +(ChessPiece self, Point2 other)
        {
            return new ChessPiece(other, self.kind, self.color);
        }

        #endregion
    }
}
