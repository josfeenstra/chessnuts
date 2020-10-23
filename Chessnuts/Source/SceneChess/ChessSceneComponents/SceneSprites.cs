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

namespace Chessnuts
{
    public class SceneSprites : SceneComponent
    {
        public Sprite WhitePawn { get => GetSprite24(2, 0); }
        public Sprite WhiteRook { get => GetSprite24(2, 1); }
        public Sprite WhiteKnight { get => GetSprite24(2, 2); }
        public Sprite WhiteBishop { get => GetSprite24(2, 3); }
        public Sprite WhiteQueen { get => GetSprite24(2, 4); }
        public Sprite WhiteKing { get => GetSprite24(2, 5); }

        public Sprite BlackPawn { get => GetSprite24(1, 0); }
        public Sprite BlackRook { get => GetSprite24(1, 1); }
        public Sprite BlackKnight { get => GetSprite24(1, 2); }
        public Sprite BlackBishop { get => GetSprite24(1, 3); }
        public Sprite BlackQueen { get => GetSprite24(1, 4); }
        public Sprite BlackKing { get => GetSprite24(1, 5); }

        public Sprite Cursor { get => GetSprite24(4, 3); }
        public Sprite CursorClicked { get => GetSprite24(4, 4); }

        public Sprite WhiteTile { get => GetSprite32(0, 0); }
        public Sprite BlackTile { get => GetSprite32(0, 1); }

        public Sprite MouseHighlight { get => GetSprite32(0, 3); }
        public Sprite SelectHighlight { get => GetSprite32(0, 2); }
        public Sprite SelectHighlightBorder { get => GetSprite32(1, 0); }
        public Sprite AttackHighlight { get => GetSprite32(0, 4); }
        public Sprite AttackHighlightBorder { get => GetSprite32(2, 0); }

        public Sprite Grass;

        private Raven.SpriteAtlas atlas24;
        private Raven.SpriteAtlas atlas32;

        public SceneSprites(Scene scene)
        {
            var atlasTexture = scene.Content.Load<Texture2D>(Content.Aseprite.Sprites24);
            atlas24 = new Raven.SpriteAtlas(atlasTexture, 24, 36, Core.GraphicsDevice);

            var atlasTexture2 = scene.Content.Load<Texture2D>(Content.Aseprite.Sprites32);
            atlas32 = new Raven.SpriteAtlas(atlasTexture2, 32, 64, Core.GraphicsDevice);

            var grassTexture = scene.Content.Load<Texture2D>(Content.Aseprite.Grass);
            Grass = new Sprite(grassTexture);
            Grass.Origin = new Vector2(0, 0);
        }

        private Sprite GetSprite24(int y, int x)
        {
            var sprite = new Sprite(atlas24.Get(y, x));
            sprite.Origin = new Vector2(-4, 8);
            return sprite;
        }

        private Sprite GetSprite32(int y, int x)
        {
            var sprite = new Sprite(atlas32.Get(y, x));
            sprite.Origin = new Vector2(0, 32);
            return sprite;
        }
    }
}
