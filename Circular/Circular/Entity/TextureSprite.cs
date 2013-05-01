using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Circular.Display;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Entity {
    public class TextureSprite : Sprite {
        public TextureSprite ( CircularGame fluxGame ) : base( fluxGame ) { }
        public TextureSprite ( CircularGame fluxGame, Vector2 size ) : base( fluxGame, size ) { }
        public TextureSprite ( CircularGame fluxGame, Vector2 size, Vector2 position ) : base( fluxGame, size, position ) { }
        public TextureSprite ( CircularGame game, Texture2D image, Vector2 origin )
            : base( game ) {
            _image = image;
            Origin = origin;
        }

        public TextureSprite ( CircularGame game, Texture2D image )
            : base( game ) {
            Image = image;
        }

        private Texture2D _image;
        public Texture2D Image {
            get { return _image; }
            set {
                _image = value;
                Origin = new Vector2( _image.Width / 2f, _image.Height / 2f );
            }
        }



        #region Overrides of Sprite

        public override Vector2 Position { get; set; }
        public override float Rotation { get; set; }

        public override void Update ( GameTime gameTime ) {
        }

        public override void Init () {
        }

        public override void Destroy ( bool animation ) { }

        public override void Draw ( GameTime gameTime ) {
            Game.SpriteBatch.Begin( 0, BlendState.AlphaBlend, null, null, null, null, Game.Camera.View );
            Game.SpriteBatch.Draw( Image, ConvertUnits.ToDisplayUnits( Position ), null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f );
        }

        #endregion
    }
}
