using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Circular.Display.Effects;
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

        private GaussianBlur blurs;

        private RenderTarget2D renderTarget1L1, renderTarget1L2, renderTarget1L3;
        private RenderTarget2D renderTarget2L1, renderTarget2L2, renderTarget2L3;

        private int renderTargetWidth;
        private int renderTargetHeight;

        private Texture2D layer1, layer2, layer3;

        public override void Update ( GameTime gameTime ) {
        }

        private readonly Vector2 Layer2 = new Vector2( 0, 80 ),
                                 Layer1 = new Vector2( 0, 160 );

        public override void Draw ( GameTime gameTime ) {
            var spriteBatch = Game.SpriteBatch;
            float cameraX = Game.Camera.Position.X;
            float cameraY = Game.Camera.Position.Y;

            if ( layer1 == null ) {
                layer1 = blurs.PerformGaussianBlur( ContentManager.BG1, renderTarget1L1, renderTarget2L1, spriteBatch );
            }
            if ( layer2 == null ) {
                layer2 = blurs.PerformGaussianBlur( ContentManager.BG2, renderTarget1L2, renderTarget2L2, spriteBatch );
            }
            if ( layer3 == null ) {
                layer3 = blurs.PerformGaussianBlur( ContentManager.BG3, renderTarget1L3, renderTarget2L3, spriteBatch );
            }



            spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null );
            spriteBatch.Draw( layer1, Position + Layer1 + new Vector2( 0, cameraY * .2f ), new Rectangle( Convert.ToInt32( cameraX * .8f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw( layer2, Position + Layer2 + new Vector2( 0, cameraY * .1f ), new Rectangle( Convert.ToInt32( cameraX * 0.5f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw( layer3, Position + new Vector2( 0, cameraY * .08f ), new Rectangle( Convert.ToInt32( cameraX * 0.3f ), 0, Width, Height ), Color.White );
            spriteBatch.End();
        }

        private const int Width = 1200,
                          Height = 50;

#if !DEBUG
       private const int Width = 1650,
                         Height = 50;
#endif

        public override void Init () {
            blurs = new GaussianBlur( Game );
            blurs.ComputeKernel( 5, 5.0f );


            //Assumes all textures are the same size
            renderTargetWidth = ContentManager.BG1.Width / 2;
            renderTargetHeight = ContentManager.BG1.Height / 2;

            renderTarget1L1 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );

            renderTarget2L1 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );


            renderTarget1L2 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );

            renderTarget2L2 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );


            renderTarget1L3 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );


            renderTarget2L3 = new RenderTarget2D( Game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None );

            // The texture offsets used by the Gaussian blur shader depends
            // on the dimensions of the render targets. The offsets need to be
            // recalculated whenever the render targets are recreated.

            blurs.ComputeOffsets( renderTargetWidth, renderTargetHeight );
        }

        public override void Destroy ( bool animation ) {
        }
    }
}
