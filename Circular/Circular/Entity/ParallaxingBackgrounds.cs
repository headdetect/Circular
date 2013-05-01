using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Circular.Managers;
using Circular;
using Circular.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Entity {
    public class ParallaxingBackgrounds : Sprite {

        public ParallaxingBackgrounds ( CircularGame fluxGame ) : base( fluxGame ) { }

        public override Vector2 Position { get; set; }
        public override float Rotation { get; set; }

        public override void Update ( GameTime gameTime ) {
        }

        private readonly Vector2 Layer2 = new Vector2( 0, 80 ),
                                 Layer1 = new Vector2( 0, 160 );

        public override void Draw ( GameTime gameTime ) {
            var spriteBatch = Game.SpriteBatch;
            float cameraX = Game.Camera.Position.X;
            float cameraY = Game.Camera.Position.Y;

            spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null );
            spriteBatch.Draw( ContentManager.BG1, Position + Layer1 + new Vector2(0, cameraY * .2f), new Rectangle( Convert.ToInt32( cameraX * .8f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw( ContentManager.BG2, Position + Layer2 + new Vector2( 0, cameraY * .1f ), new Rectangle( Convert.ToInt32( cameraX * 0.5f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw( ContentManager.BG3, Position + new Vector2( 0, cameraY * .08f ), new Rectangle( Convert.ToInt32( cameraX * 0.3f ), 0, Width, Height ), Color.White );
            spriteBatch.End();
        }

        private const int Width = 1200,
                          Height = 50;

#if !DEBUG
       private const int Width = 1650,
                         Height = 50;
#endif

        public override void Init () {


        }

        public override void Destroy ( bool animation ) {
        }
    }
}
