using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nez;
using Nez.Sprites;
using Nez.Textures;
using Raven;

namespace Chessnuts
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ChessScene : Scene
    {
        /// <summary>
        /// The loaded scene content
        /// </summary>

        // meta
        // SceneResolutionPolicy policy;
        Renderer renderer;

        public ChessScene()
        {
            // base.policy = Scene.SceneResolutionPolicy.ExactFit;    
        }

        public override void Initialize()
        {
            // setup a pixel perfect screen
            ClearColor = Color.Black;
            SetDesignResolution(SD.SCREEN_WIDTH, SD.SCREEN_HEIGHT, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(SD.SCREEN_WIDTH * SD.SCALE, SD.SCREEN_HEIGHT * SD.SCALE);

            // add a renderer
            renderer = AddRenderer(new DefaultRenderer());

            // setup scene components
            base.Initialize();
            AddSceneComponent(new SceneSprites(this));
            var b = AddSceneComponent(new ChessSceneBoard(this));
            var c = AddSceneComponent(new ChessSceneController(this));
            var players = new Player[2] { new PlayerAI(PieceColor.White, Intelligence.VeryHard), new PlayerAI(PieceColor.Black, Intelligence.Hard) };
            var t = AddSceneComponent(new TurnSystem(b, players));

            t.Activate();
        }
    }
}
