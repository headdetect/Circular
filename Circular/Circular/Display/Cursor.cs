using Circular.Managers;
using Circular.Display;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Circular.Display {
    public class CursorComponent : IHUDComponent  {

        float X;
        float Y;

        readonly CircularGame _game;

        public CursorComponent ( CircularGame game ) {
            this._game = game;
        }

        public override void Update ( Microsoft.Xna.Framework.GameTime gameTime ) {
            MouseState state = Mouse.GetState();
            this.X = state.X;
            this.Y = state.Y;

        }

        public override void Draw ( Microsoft.Xna.Framework.GameTime gameTime ) {
            _game.SpriteBatch.Begin();

            _game.SpriteBatch.Draw( ContentManager.CursorTexture, new Vector2( X, Y ), Color.White );

            _game.SpriteBatch.End();
        }

        public override void Init () {
            ZIndex = 1000;
        }
    }
}
