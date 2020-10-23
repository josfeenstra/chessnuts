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
    public class SceneLose : Scene
    {
        public const int SCREEN_RENDER_LAYER = -999;
        Renderer renderer;
        Table table;
        UICanvas canvas;

        public SceneLose()
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
            canvas = CreateEntity("ui").AddComponent(new UICanvas());
            canvas.IsFullScreen = true;
            canvas.RenderLayer = SCREEN_RENDER_LAYER;

            // table 
            table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(false).SetBounds(0, 0, SD.SCREEN_WIDTH, SD.SCREEN_HEIGHT);
            table.Left().Top();
            table.SetScale(SD.SCALE);
            Cell c;

            // title 
            var labelStyle = new LabelStyle(Color.Black);
            c = table.Add(new Label("You Lose!", labelStyle).SetFontScale(4));
            table.Row().SetPadTop(10);
            c = table.Add(new Label("try again?").SetFontScale(1));
            table.Row().SetPadTop(30);

            // button style
            var buttonStyle = new TextButtonStyle(
                new PrimitiveDrawable(Color.Black, 10f),
                new PrimitiveDrawable(Color.Yellow), 
                new PrimitiveDrawable(Color.DarkSlateBlue))
                { DownFontColor = Color.Black };

            // buttons  
            c = table.Add(new TextButton("To Main Menu", buttonStyle)).SetMinHeight(30);
            c.GetElement<Button>().OnClicked += Shared.TransitionToMainMenu;
            table.Row().SetPadTop(10);

        }
    }
}
