using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Circular.Display;
using Circular.Utils;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Entity {
    public class PrimitiveSprite {

        public Vector2 Origin;
        public Texture2D Texture;

        public PrimitiveSprite ( Texture2D texture, Vector2 origin ) {
            this.Texture = texture;
            this.Origin = origin;
        }

        public PrimitiveSprite ( Texture2D sprite ) {
            Texture = sprite;
            Origin = new Vector2( sprite.Width / 2f, sprite.Height / 2f );
        }
    }
}
