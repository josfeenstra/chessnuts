using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;
using Nez.Sprites;
using Nez.Textures;

using Raven;

namespace Chessnuts
{
    class ChessSceneController : ChessSceneComponent, IUpdatable
    {
        public ChessSceneBoard visualBoard;

        public ChessPiece selected = ChessPiece.Empty;
        private Player activePlayer = null;

        #region Visuals

        Entity cursorVisual;
        // Entity cursor;

        List<ChessMove> highlightMoves;
        List<Entity> highlightVisuals;

        #endregion

        public ChessSceneController(Scene scene)
        {
            // logical
            visualBoard = scene.GetSceneComponent<ChessSceneBoard>();
            selected = ChessPiece.Empty;
            ClearHighlights();

            // visuals 
            //cursor = scene.CreateEntity("cursor");
            //cursor.AddComponent(new SpriteRenderer(
            //    scene.GetSceneComponent<ChessSceneSprites>().Cursor)
            //{ RenderLayer = -9999 });

            cursorVisual = scene.CreateEntity("cursorVisual");
            cursorVisual.AddComponent(new SpriteRenderer(
                scene.GetSceneComponent<SceneSprites>().MouseHighlight)
            { RenderLayer = -10 });

            cursorVisual.AddComponent(new Lerper(SD.PIECE_LERP_FACTOR));
        }

        public override void Update()
        {
            // mouse cursor visuals
            //UpdateCursor();

            // check whats underneath the cursor for highlight and selection      
            Point2 point = visualBoard.pieceVisuals.ToLocalCentered(Input.MousePosition);

            bool noSelected = selected == ChessPiece.Empty;
            bool isMouseOnBoard = visualBoard.TryGetOccupant(point, out ChessPiece piece);
            bool hasClicked = Input.RightMouseButtonPressed;

            // --- update cursor visual

            if (!isMouseOnBoard)
            {
                cursorVisual.Enabled = false;
                return;
            }
            cursorVisual.Enabled = true;

            // show which normal square cursor is at 
            cursorVisual.GetComponent<Lerper>().Force(visualBoard.pieceVisuals.ToWorld(point));

            if (hasClicked)
            {
                visualBoard.ProvideFeedback(point);
                cursorVisual.Position += new Vector2(0, 3);
                cursorVisual.GetComponent<Lerper>().SetEnabled(true);
            }

            // --- update unit selected
            Act(point, noSelected, piece, hasClicked);
        }

        #region Player Management

        private void Act(Point2 point, bool noSelected, ChessPiece piece, bool hasClicked)
        {
            // TODO this logic looks stupid, fix it
            if (activePlayer == null)
                return;

            if (hasClicked && noSelected)
            {
                // deselect or select 
                TrySelect(piece);
            }
            else if (hasClicked && piece.color == activePlayer.ColorOwned)
            {
                // selecting other unit while having a unit selected
                TrySelect(piece);
            }
            else
            {
                var subset = highlightMoves.Where(x => x.toPiece.point == point).ToArray();
                if (subset.Length == 1)
                {
                    ChessMove move = subset[0];
                    // Hovering over a certain move...
                    // TODO add sparkly visuals

                    if (hasClicked)
                    {
                        // perform move 
                        activePlayer.CommitMove(move);
                        activePlayer = null;
                        Deselect();
                    }
                }
                else
                {
                    if (hasClicked)
                    {
                        Deselect();
                    }
                }
            }
        }

        public void SetActivePlayer(Player player)
        {
            this.activePlayer = player;
        }

        #endregion
        #region Selection And Highlights

        private void Deselect()
        {
            TrySelect(ChessPiece.Empty);
        }

        private void TrySelect(ChessPiece piece)
        { 
            ClearHighlights();
            selected = ChessPiece.Empty;
            if (piece == ChessPiece.Empty) return;
            if (piece == selected) return;
            if (piece.color != activePlayer.ColorOwned) return;

            // piece is valid and selectable!
            selected = piece;
            // Debug.Log($"selected a {piece.kind.ToString()}");
            
            SetHighlights(piece, visualBoard.BoardState);
        }

        private void SetHighlights(ChessPiece piece, ChessBoard board)
        {
            var moves = board.GetPossibleMoves(piece);

            // store values
            highlightMoves = moves;

            highlightVisuals.Add(CreateHighlightEntity(piece.point, Scene.GetSceneComponent<SceneSprites>().SelectHighlight));
            highlightVisuals.Add(CreateHighlightEntity(piece.point, Scene.GetSceneComponent<SceneSprites>().SelectHighlightBorder, -6));
            foreach (ChessMove move in moves)
            {
                if (move.kind == MoveKind.Move)
                {
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().SelectHighlight));
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().SelectHighlightBorder, -6));
                }
                else if (move.kind == MoveKind.Capture)
                {
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().AttackHighlight));
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().AttackHighlightBorder, -6));
                }
                else
                {
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().AttackHighlight));
                    highlightVisuals.Add(CreateHighlightEntity(move.toPiece.point, Scene.GetSceneComponent<SceneSprites>().AttackHighlightBorder, -6));
                }
            }


            foreach (var item in highlightMoves)
            {
                Debug.Log(item.ToString());
            }         
        }

        private void ClearHighlights()
        {
            highlightMoves = new List<ChessMove>();
            if (highlightVisuals != null)
            {
                foreach (Entity e in highlightVisuals)
                {
                    e.Destroy();
                }
            }
            highlightVisuals = new List<Entity>();
        }

        private Entity CreateHighlightEntity(Point2 point, Sprite sprite, int depth = -5)
        {
            var e = Scene.CreateEntity("highlight");
            e.Position = visualBoard.pieceVisuals.ToWorld(point);                       
            e.AddComponent(new SpriteRenderer(sprite)
            { RenderLayer = depth});
            return e;
        }

        #endregion
    }
}
