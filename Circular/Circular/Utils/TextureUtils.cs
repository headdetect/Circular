using System;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Utils {
    public class TextureUtils {

        /// <summary>
        /// Creates a rectangle texture from a specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Texture2D CreateFromColor ( CircularGame game, Microsoft.Xna.Framework.Color color, int width, int height ) {
            var texture = new Texture2D ( game.GraphicsDevice, width, height, true, SurfaceFormat.Color );

            Microsoft.Xna.Framework.Color[] colors = new Microsoft.Xna.Framework.Color[ (int) ( width * height ) ];
            for ( int i = 0; i < colors.Length; i++ ) {
                colors[ i ] = color;
            }

            texture.SetData ( colors );

            return texture;
        }


        public static Texture2D CreateFromGradient ( CircularGame game, Microsoft.Xna.Framework.Color top, Microsoft.Xna.Framework.Color bottom, int width, int height ) {
            var texture = new Texture2D ( game.GraphicsDevice, width, height, true, SurfaceFormat.Color );

            Microsoft.Xna.Framework.Color[] colors = new Microsoft.Xna.Framework.Color[ width * height ];
            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < width; x++ ) {
                    int a = 0xFF; //top.A * ( index / height ) + bottom.A * ( ( index / height ) - 1 );
                    int r = (int) ( top.R * (float) ( (float) y / (float) height ) + bottom.R * ( (float) ( (float) y / (float) height ) - 1 ) );
                    int g = (int) ( top.G * (float) ( (float) y / (float) height ) + bottom.G * ( (float) ( (float) y / (float) height ) - 1 ) );
                    int b = (int) ( top.B * (float) ( (float) y / (float) height ) + bottom.B * ( (float) ( (float) y / (float) height ) - 1 ) );



                    colors[ x ] = new Microsoft.Xna.Framework.Color ( Math.Abs ( r ), Math.Abs ( g ), Math.Abs ( b ), Math.Abs ( a ) );
                }
            }

            texture.SetData ( colors );

            return texture;
        }


    }
}
