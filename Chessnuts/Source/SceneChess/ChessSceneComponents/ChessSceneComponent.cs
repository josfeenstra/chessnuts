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
    /// SceneComponent with shortcuts 
    /// </summary>
    class ChessSceneComponent : SceneComponent
    {
        public SceneSprites sprites { get => Scene.GetSceneComponent<SceneSprites>(); }
        // public BoardPortal board { get => Scene.GetSceneComponent<BoardPortal>(); }
    }
}
