using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;

using Raven;

namespace Chessnuts
{
    class Lerper : Component, IUpdatable
    {
        private Vector2 targetPosition = Vector2.Zero;
        private float factor;

        public Lerper(float factor)
        {
            this.factor = factor;
        }

        public override void OnAddedToEntity()
        {
            targetPosition = Entity.Position;
        }

        public void Set(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
            SetEnabled(true);
        }

        // skip lerping altogether
        public void Force(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
            Entity.Position = targetPosition;
            SetEnabled(false);
        }

        public void Update()
        {
            if (Entity.Position == targetPosition)
                SetEnabled(false);
            Entity.Position = Lerp.Vector(Entity.Position, targetPosition, factor);
        }
    }
}
