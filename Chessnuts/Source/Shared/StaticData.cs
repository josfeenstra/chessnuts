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
    /// Static Data concerning all of chessnuts
    /// </summary>
    public static class SD
    {
        public const int TILE_SIZE = 32;
        public const int SCREEN_WIDTH = 320;
        public const int SCREEN_HEIGHT = 320;
        public const int SCALE = 3;

        public const int BOARD_HEIGHT = 8;
        public const int BOARD_WIDTH = 8;

        public const int WHITE_PAWN_STARTING_ROW = 6;
        public const int WHITE_PAWN_PROMOTE_ROW = 0;

        public const int BLACK_PAWN_STARTING_ROW = 1;
        public const int BLACK_PAWN_PROMOTE_ROW = 7;

        public const float PIECE_LERP_FACTOR = 0.07f;

        public const int SCREEN_RENDER_LAYER = -999;
    }
}
