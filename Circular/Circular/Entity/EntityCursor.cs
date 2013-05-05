using Circular.Helpers;
using Circular.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Circular.Entity {
    public class EntityCursor : Sprite {
        private float X;
        private float Y;

        public EntityCursor ( ScreenManager manager ) : base ( manager ) {}

        public override void Update ( GameTime gameTime ) {
            MouseState state = Mouse.GetState ();
            X = state.X;
            Y = state.Y;
        }

        public override void Init () {
            Texture = ContentHelper.GetTexture ( "cursor" );
            base.Init ();
        }
    }
}