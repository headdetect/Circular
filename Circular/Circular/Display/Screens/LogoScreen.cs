using System;
using Circular.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display.Screens
{
    public class LogoScreen : GameScreen
    {
        private TimeSpan _duration;
        private Texture2D _farseerLogoTexture;
        private int width, height;

        public LogoScreen(TimeSpan duration)
        {
            _duration = duration;
            HasCursor = false;
            TransitionOffTime = TimeSpan.FromSeconds(0.6);
        }

        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            _farseerLogoTexture = ContentWrapper.GetTexture("logo");
            Viewport viewport = Framework.GraphicsDevice.Viewport;
            width = viewport.Width;
            height = viewport.Height;
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            if (input.IsMenuSelect() || input.IsMenuCancel())
            {
                _duration = TimeSpan.Zero;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _duration -= gameTime.ElapsedGameTime;
            if (_duration <= TimeSpan.Zero)
            {
                ExitScreen();
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Framework.GraphicsDevice.Clear(Color.White);

            Sprites.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.AnisotropicClamp, null, RasterizerState.CullNone);
            Sprites.Draw(_farseerLogoTexture, new Rectangle(0, 0, width, height), Color.White);
            Sprites.End();
        }
    }
}