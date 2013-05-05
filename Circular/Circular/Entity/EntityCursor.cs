
using Circular.Helpers;
using Circular.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Circular.Entity {
    public class EntityCursor : Sprite {

        float X;
        float Y;

        public EntityCursor ( ScreenManager manager ) : base ( manager ) {}

        public override void Update ( Microsoft.Xna.Framework.GameTime gameTime ) {
            MouseState state = Mouse.GetState();
            this.X = state.X;
            this.Y = state.Y;
        }

        public override void Init () {
            Texture = ContentHelper.GetTexture ( "cursor" );
            base.Init();
        }

    }
}
