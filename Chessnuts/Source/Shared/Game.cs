
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace Chessnuts
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MyGame : Core
    {
        public MyGame() : base(240,240, false, "ChessNuts")
        {

        }

        protected override void Initialize()
        {
            // TODO create state machine for switching between scenes
            base.Initialize();
            Core.ExitOnEscapeKeypress = false;
            Core.Instance.IsMouseVisible = true;


            var scene = new SceneMainMenu();
            Core.Scene = scene;
        }
    }
}
