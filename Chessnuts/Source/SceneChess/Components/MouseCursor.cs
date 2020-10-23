using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;
using Nez.Sprites;
using Nez.Textures;
using Raven;

namespace Chessnuts
{
    class MouseCursor : Component, IUpdatable
    {
        public MouseCursor()
        {

        }

        public void Update()
        {
            
        }

        
        private void UpdateCursor()
        {
            //// manage cursor
            //cursor.Position = Input.MousePosition;
            //if (Input.RightMouseButtonDown)
            //{
            //    cursor.GetComponent<SpriteRenderer>().Sprite = Scene.GetSceneComponent<ChessSceneSprites>().CursorClicked;
            //}
            //else
            //{
            //    cursor.GetComponent<SpriteRenderer>().Sprite = Scene.GetSceneComponent<ChessSceneSprites>().Cursor;
            //}
        }

        /// <summary>
        /// Factory method for adding a custom cursor to a scene
        /// EXPECTS CHESSSCENESPRITES present in scene
        /// </summary>
        public static Entity Create(Scene scene)
        {
            // visuals 
            Entity cursor = scene.CreateEntity("cursor");
            Sprite normal = scene.GetSceneComponent<SceneSprites>().Cursor;
            Sprite clicked = scene.GetSceneComponent<SceneSprites>().CursorClicked;

            cursor.AddComponent(
                new SpriteRenderer(normal)
                { RenderLayer = -9999 }
            );
            // cursor.AddComponent<>

            return cursor;

        }
    }
}
