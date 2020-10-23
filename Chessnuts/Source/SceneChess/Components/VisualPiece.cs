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

namespace Chessnuts
{
    /// <summary>
    /// A Visual Representation of a thing on the board. 
    /// </summary>
    class PieceVisual : Component, IUpdatable
    {
        public ChessPiece piece;
        public Vector2 renderTargetPosition;

        public PieceVisual(Vector2 pos)
        {
            renderTargetPosition = pos;
        }

        /// <summary>
        /// Set a new position internally, and notify the board
        /// </summary>
        /// <param name="point"></param>
        public void Set(Vector2 position)
        {
            // some teleport animation or something
            renderTargetPosition = position;
        }

        public void Move(Vector2 position)
        {
            // some move animation
            renderTargetPosition = position;
        }

        public void Attack(Vector2 position)
        {
            // some attack animation
            renderTargetPosition = position;
        }

        public void Destroy()
        {
            Entity.Destroy(); // destroy the entity you are attached to
        }

        public void Update()
        {
            Entity.Position = Lerp.Vector(Entity.Position, renderTargetPosition, SD.PIECE_LERP_FACTOR);
            var sr = Entity.GetComponent<SpriteRenderer>().RenderLayer = -10 + -piece.point.Y * 10;
        }
    }
}
