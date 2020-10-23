using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nez;
using Raven;
using Nez.Sprites;
using Nez.Textures;
using Nez.UI;

namespace Chessnuts
{
    /// <summary>
    /// Functionalities shared among scenes
    /// </summary>
    static class Shared
    {
        public static void PrintBackground(Scene scene)
        {
            var sprites = scene.GetSceneComponent<SceneSprites>();

            // print a checkerboard
            var tileGrid = new Grid2D<Entity>(new Vector2(SD.TILE_SIZE, SD.TILE_SIZE), SD.BOARD_WIDTH, SD.BOARD_HEIGHT, SD.TILE_SIZE);
            foreach (Point2 point in tileGrid.IteratePoints())
            {
                Sprite sprite = (point.X % 2 == 0) ^ (point.Y % 2 == 0) ? sprites.BlackTile : sprites.WhiteTile;
                Entity e = scene.CreateEntity("tile");
                e.AddComponent(new SpriteRenderer(sprite) { RenderLayer = 0 });
                e.Position = tileGrid.ToWorld(point);
                e.AddComponent(new Lerper(SD.PIECE_LERP_FACTOR));
                tileGrid.Set(point, e);
            }

            // put grass right on top of it
            Entity en = scene.CreateEntity("grass");
            en.AddComponent(new SpriteRenderer(sprites.Grass) { RenderLayer = -7 });
            en.Position = Vector2.Zero;
        }

        public static Table InitTable(Scene scene, out Label a, out Label b)
        {
            // canvas
            var canvas = scene.CreateEntity("ui").AddComponent(new UICanvas());
            canvas.IsFullScreen = true;
            canvas.RenderLayer = SD.SCREEN_RENDER_LAYER;

            // table 
            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(false).SetBounds(0, 0, SD.SCREEN_WIDTH, SD.SCREEN_HEIGHT);
            table.Center().Top().PadTop(50);
            table.SetScale(SD.SCALE);
            Cell c;

            // title 
            var labelStyle = new LabelStyle(Color.Brown);
            a = new Label("", labelStyle).SetFontScale(4);
            c = table.Add(a);
            table.Row().SetPadTop(10);

            // under thing
            labelStyle = new LabelStyle(Color.Brown);
            b = new Label("", labelStyle).SetFontScale(1);
            c = table.Add(b);
            table.Row().SetPadTop(30);

            return table;
        }

        #region SceneTransitions

        public static void TransitionToMainMenu(Button button)
        {
            Core.StartSceneTransition(new FadeTransition(() => new SceneMainMenu()));
        }

        public static void TransitionToWin(Button button)
        {
            Core.StartSceneTransition(new FadeTransition(() => new SceneWin())
            {

            });
        }

        public static void TransitionToLose(Button button)
        {
            Core.StartSceneTransition(new FadeTransition(() => new SceneLose())
            {

            });
        }

        public static void TransitionToGame(Button button)
        {
            Core.StartSceneTransition(new FadeTransition(() => new ChessScene())
            {

            });
        }

        #endregion

    }
}
