using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Entity {
    public class PrimitiveSprite {
        public Vector2 Origin;
        public Texture2D Texture;

        public PrimitiveSprite ( Texture2D texture, Vector2 origin ) {
            Texture = texture;
            Origin = origin;
        }

        public PrimitiveSprite ( Texture2D sprite ) {
            Texture = sprite;
            Origin = new Vector2 ( sprite.Width / 2f, sprite.Height / 2f );
        }
    }
}