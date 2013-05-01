
using Circular.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Circular.Entity {
    public class EntityCursor : Sprite {
        public EntityCursor ( CircularGame fluxGame ) : base ( fluxGame ) {}

        float X;
        float Y;

        public override void Update ( Microsoft.Xna.Framework.GameTime gameTime ) {
            MouseState state = Mouse.GetState();
            this.X = state.X;
            this.Y = state.Y;

        }

        public override Vector2 Position { get; set; }
        public override float Rotation { get; set; }

        public override void Draw ( Microsoft.Xna.Framework.GameTime gameTime ) {
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw( ContentManager.CursorTexture, new Vector2( X, Y ), Color.White );
            Game.SpriteBatch.End();
        }

        public override void Init () {
            ZIndex = 1000;
        }

        public override void Destroy ( bool animation ) {
        }
    }
}
