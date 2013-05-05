using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Utils {
    public class TextureUtils {
        /// <summary>
        /// Creates a rectangle texture from a specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Texture2D CreateFromColor ( CircularGame game, Color color, int width, int height ) {
            var texture = new Texture2D ( game.GraphicsDevice, width, height, true, SurfaceFormat.Color );

            var colors = new Color[( width * height )];
            for ( int i = 0; i < colors.Length; i++ ) {
                colors [i] = color;
            }

            texture.SetData ( colors );

            return texture;
        }


        public static Texture2D CreateFromGradient ( CircularGame game, Color top, Color bottom, int width, int height ) {
            var texture = new Texture2D ( game.GraphicsDevice, width, height, true, SurfaceFormat.Color );

            var colors = new Color[width * height];
            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < width; x++ ) {
                    int a = 0xFF; //top.A * ( index / height ) + bottom.A * ( ( index / height ) - 1 );
                    var r = (int) ( top.R * ( y / (float) height ) + bottom.R * ( ( y / (float) height ) - 1 ) );
                    var g = (int) ( top.G * ( y / (float) height ) + bottom.G * ( ( y / (float) height ) - 1 ) );
                    var b = (int) ( top.B * ( y / (float) height ) + bottom.B * ( ( y / (float) height ) - 1 ) );


                    colors [x] = new Color ( Math.Abs ( r ), Math.Abs ( g ), Math.Abs ( b ), Math.Abs ( a ) );
                }
            }

            texture.SetData ( colors );

            return texture;
        }
    }
}