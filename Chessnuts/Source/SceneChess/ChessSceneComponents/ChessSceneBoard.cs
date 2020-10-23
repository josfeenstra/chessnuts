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
    /// <summary>
    /// This is the visuals of the board. ALl logic is done in ChessBoard. This is a visuals layer, 
    /// meant for interaction with this underlying logic
    /// </summary>
    class ChessSceneBoard : ChessSceneComponent
    {
        private ChessBoard board;
        public ChessBoard BoardState { get => board; }

        public Grid2D<PieceVisual> pieceVisuals;
        private Grid2D<Entity> tileGrid;

        public ChessSceneBoard(Scene scene)
        {
            Scene = scene;

            // create the logical board 
            board = new ChessBoard();
            board.InitializeClassicSetup();
            InitGraphics();
            SetBoardPieces(board);
        }

        private void InitGraphics()
        {
            // print a checkerboard
            tileGrid = new Grid2D<Entity>(new Vector2(SD.TILE_SIZE, SD.TILE_SIZE), SD.BOARD_WIDTH, SD.BOARD_HEIGHT, SD.TILE_SIZE);
            foreach (Point2 point in tileGrid.IteratePoints())
            {
                Sprite sprite = (point.X % 2 == 0) ^ (point.Y % 2 == 0) ? sprites.BlackTile : sprites.WhiteTile;
                Entity e = Scene.CreateEntity("tile");
                e.AddComponent(new SpriteRenderer(sprite) { RenderLayer = 0 });
                e.Position = tileGrid.ToWorld(point);
                e.AddComponent(new Lerper(SD.PIECE_LERP_FACTOR));
                tileGrid.Set(point, e);
            }

            // put grass right on top of it
            Entity en = Scene.CreateEntity("grass");
            en.AddComponent(new SpriteRenderer(sprites.Grass) { RenderLayer = -7 });
            en.Position = Vector2.Zero;
        }


        public void SetBoardPieces(ChessBoard newBoard)
        {
            // create visual representations of the boardstate that can be clicked
            pieceVisuals = new Grid2D<PieceVisual>(new Vector2(SD.TILE_SIZE, SD.TILE_SIZE), SD.BOARD_WIDTH, SD.BOARD_HEIGHT, SD.TILE_SIZE);
            pieceVisuals.Fill(null);

            foreach (var point in newBoard.grid.IteratePoints())
            {
                var piece = newBoard.grid.Get(point);
                if (piece == ChessPiece.Empty) continue;
                CreatePiece(piece);
            }
        }


        /// <summary>
        /// Create a piece on the grid. 
        /// </summary>
        public PieceVisual CreatePiece(ChessPiece piece)
        {
            Point2 point = piece.point;
            Entity e = Scene.CreateEntity("tile");    
            PieceVisual visuals = e.AddComponent(new PieceVisual(pieceVisuals.ToWorld(point)));
            pieceVisuals.Set(point, visuals);

            // choose sprite based upon color and piecetype
            Sprite sprite = null;
            switch (piece.kind)
            {
                case PieceKind.Pawn:
                    sprite = piece.color == PieceColor.White ? sprites.WhitePawn : sprites.BlackPawn;
                    break;
                case PieceKind.Knight:
                    sprite = piece.color == PieceColor.White ? sprites.WhiteKnight : sprites.BlackKnight;
                    break;
                case PieceKind.Bishop:
                    sprite = piece.color == PieceColor.White ? sprites.WhiteBishop : sprites.BlackBishop;
                    break;
                case PieceKind.Rook:
                    sprite = piece.color == PieceColor.White ? sprites.WhiteRook : sprites.BlackRook;
                    break;
                case PieceKind.Queen:
                    sprite = piece.color == PieceColor.White ? sprites.WhiteQueen : sprites.BlackQueen;
                    break;
                case PieceKind.King:
                    sprite = piece.color == PieceColor.White ? sprites.WhiteKing : sprites.BlackKing;
                    break;
            }

            e.AddComponent(new SpriteRenderer(sprite) { RenderLayer = -point.Y * 10});
            return visuals;
        }

        public bool TryGetOccupant(Point2 point, out ChessPiece occupant)
        {
            return board.grid.TryGet(point, out occupant);
        }

        public ChessBoard GetCurrentBoardState()
        {
            return board.Copy();
        }

        public bool CheckAndApplyPlay(ChessMove move)
        {
            if (board.CheckAndApplyPlay(move))
            {
                ApplyMove(move);
                return true;
            }
            return false;
        }

        // make the piece dance
        private void ApplyMove(ChessMove move)
        {
            ChessPiece piece = move.fromPiece;
            Point2 to = move.toPiece.point;
            Point2 from = piece.point;
            PieceVisual pieceVisual = pieceVisuals.Get(from);
            PieceVisual other;
            switch (move.kind)
            {
                case MoveKind.None:

                    break;
                case MoveKind.Move:
                    pieceVisual.Move(pieceVisuals.ToWorld(to));
                    MovePieceVisual(pieceVisual, from, to);
                    break;
                case MoveKind.Capture:
                    pieceVisual.Attack(pieceVisuals.ToWorld(to));
                    other = pieceVisuals.Get(to);
                    other.Destroy();
                    MovePieceVisual(pieceVisual, from, to);
                    break;
                case MoveKind.ShortCastle:

                    break;
                case MoveKind.LongCastle:

                    break;
                case MoveKind.Promote:
                    if (move.toPiece == ChessPiece.Empty)
                    {
                        // like move
                        pieceVisual.Move(pieceVisuals.ToWorld(to));
                    }
                    else
                    {
                        // like attack
                        pieceVisual.Attack(pieceVisuals.ToWorld(to));
                        other = pieceVisuals.Get(to);
                        other.Destroy();
                    }

                    // TODO sparkly promote effect 
                    
                    // turn to queen
                    pieceVisual.Destroy();
                    var newVisual = CreatePiece(new ChessPiece(to, PieceKind.Queen, piece.color));
                    MovePieceVisual(newVisual, from, to);

                    break;
                case MoveKind.EnPassant:

                    break;
                case MoveKind.CaptureKing:
                    break;
            }    
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MovePieceVisual(PieceVisual visual, Point2 from, Point2 to)
        {
            visual.piece = new ChessPiece(to, visual.piece.kind, visual.piece.color);
            pieceVisuals.Set(from, null);
            pieceVisuals.Set(to, visual);
        }

        // reverting move
        private bool RevertMove(ChessMove move)
        {
            switch (move.kind)
            {
                case MoveKind.None:

                    break;
                case MoveKind.Move:

                    break;
                case MoveKind.Capture:

                    break;
                case MoveKind.ShortCastle:

                    break;
                case MoveKind.LongCastle:

                    break;
                case MoveKind.Promote:

                    break;
                case MoveKind.EnPassant:

                    break;
                case MoveKind.CaptureKing:

                    break;
            }
            return false;
        }

        #region Animation

        public void ProvideFeedback(Point2 point)
        {
            // get the tile and make it shake
            if (tileGrid.TryGet(point, out Entity tile))
            {
                tile.Position += new Vector2(0, 2);
                tile.GetComponent<Lerper>().SetEnabled(true);
            }

            if (pieceVisuals.TryGet(point, out PieceVisual piece))
            {
                if (piece == null) return;
                piece.Entity.Position += new Vector2(0, 2);
            }
        }

        #endregion
    }
}
