using System;
using Circular.Display;
using Circular.Display.Effects;
using Circular.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Entity {
    public class ParallaxingBackgrounds : GameScreen {
        private GaussianBlur blurs;

        private RenderTarget2D renderTarget1L1, renderTarget1L2, renderTarget1L3;
        private RenderTarget2D renderTarget2L1, renderTarget2L2, renderTarget2L3;

        private int renderTargetWidth;
        private int renderTargetHeight;

        private Texture2D layer1, layer2, layer3;

        public Vector2 CarPosition;

        private readonly Vector2 Layer2 = new Vector2 ( 0, 80 ),
                                 Layer1 = new Vector2 ( 0, 160 );


        public override void Draw ( GameTime gameTime ) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            float cameraX = CarPosition.X;
            float cameraY = CarPosition.Y;

            if ( layer1 == null ) {
                layer1 = blurs.PerformGaussianBlur ( ContentHelper.GetTexture ( "layerone" ), renderTarget1L1, renderTarget2L1, spriteBatch );
            }
            if ( layer2 == null ) {
                layer2 = blurs.PerformGaussianBlur ( ContentHelper.GetTexture ( "layertwo" ), renderTarget1L2, renderTarget2L2, spriteBatch );
            }
            if ( layer3 == null ) {
                layer3 = blurs.PerformGaussianBlur ( ContentHelper.GetTexture ( "layerthree" ), renderTarget1L3, renderTarget2L3, spriteBatch );
            }


            spriteBatch.Begin ( SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null );
            spriteBatch.Draw ( layer1, Layer1 + new Vector2 ( 0, cameraY * .2f ), new Rectangle ( Convert.ToInt32 ( cameraX * .8f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw ( layer2, Layer2 + new Vector2 ( 0, cameraY * .1f ), new Rectangle ( Convert.ToInt32 ( cameraX * 0.5f ), 0, Width, Height ), Color.White );
            spriteBatch.Draw ( layer3, new Vector2 ( 0, cameraY * .08f ), new Rectangle ( Convert.ToInt32 ( cameraX * 0.3f ), 0, Width, Height ), Color.White );
            spriteBatch.End ();
        }

        private const int Width = 1200,
                          Height = 50;

#if !DEBUG
       private const int Width = 1650,
                         Height = 50;
#endif

        public override void LoadContent () {
            blurs = new GaussianBlur ( ScreenManager.Game );
            blurs.ComputeKernel ( 5, 5.0f );


            //Assumes all textures are the same size
            renderTargetWidth = ContentHelper.GetTexture ( "layerone" ).Width / 2;
            renderTargetHeight = ContentHelper.GetTexture ( "layerone" ).Height / 2;

            renderTarget1L1 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );

            renderTarget2L1 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );


            renderTarget1L2 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );

            renderTarget2L2 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );


            renderTarget1L3 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );


            renderTarget2L3 = new RenderTarget2D ( ScreenManager.GraphicsDevice,
                                                   renderTargetWidth, renderTargetHeight, false,
                                                   ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                   DepthFormat.None );

            // The texture offsets used by the Gaussian blur shader depends
            // on the dimensions of the render targets. The offsets need to be
            // recalculated whenever the render targets are recreated.

            blurs.ComputeOffsets ( renderTargetWidth, renderTargetHeight );
        }
    }
}