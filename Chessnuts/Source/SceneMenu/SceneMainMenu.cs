using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nez;
using Nez.Sprites;
using Nez.Textures;
using Raven;
using Nez.UI;

namespace Chessnuts
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SceneMainMenu : Scene
    {
        Renderer renderer;
        Table table;
        UICanvas canvas;

        public SceneMainMenu()
        {
            // base.policy = Scene.SceneResolutionPolicy.ExactFit;    
        }

        public override void Initialize()
        {
            // scene 
            AddSceneComponent(new SceneSprites(this));
            Shared.PrintBackground(this);

            // setup a pixel perfect screen
            ClearColor = Color.Black;
            SetDesignResolution(SD.SCREEN_WIDTH, SD.SCREEN_HEIGHT, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(SD.SCREEN_WIDTH * SD.SCALE, SD.SCREEN_HEIGHT * SD.SCALE);

            // add a renderer
            renderer = AddRenderer(new DefaultRenderer());

            // canvas
            table = Shared.InitTable(this, out Label a, out Label b);
            a.SetText("ChessNuts");
            b.SetText("A Game by Sainer");

            // button style
            var buttonStyle = new TextButtonStyle(
                new PrimitiveDrawable(Color.Black, 10f),
                new PrimitiveDrawable(Color.Yellow), 
                new PrimitiveDrawable(Color.DarkSlateBlue))
                { DownFontColor = Color.Black };

            // buttons  
            var c = table.Add(new TextButton("Play", buttonStyle)).SetMinHeight(30);
            c.GetElement<Button>().OnClicked += Shared.TransitionToGame;
            table.Row().SetPadTop(10);

            c = table.Add(new TextButton("Settings", buttonStyle)).SetMinHeight(30);
            table.Row().SetPadTop(10);

            c = table.Add(new TextButton("Quit", buttonStyle)).SetMinHeight(30);
            c.GetElement<Button>().OnClicked += button => { Core.Exit(); };
            table.Row().SetPadTop(10);

            // put table in the middle
            // var value = (SD.SCREEN_WIDTH - width) / 2;
            // table.PadLeft(value);
            // Debug.Log(value);
            // Debug.Log(width);
        }

        bool toggle = false;
        public void OnToggleSomething(Button button)
        {
            toggle = !toggle;
            Debug.Log("toggle! " + toggle);
        }


    }
}
