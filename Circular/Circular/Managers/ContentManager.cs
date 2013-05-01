using Circular.Display;
using Circular.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Managers {
    public class ContentManager {

        #region Textures

        /// <summary>
        /// Background layers for parallaxing.
        /// </summary>
        public static Texture2D BG1, BG2, BG3;

        public static Texture2D CursorTexture;


        #endregion

        #region Fonts

        public static SpriteFont FPSFont;

        #endregion

        public ContentManager ( CircularGame fluxGame ) {
            BG1 = ContentWrapper.GetTexture("LayerOne" );
            BG2 = ContentWrapper.GetTexture( "LayerTwo" );
            BG3 = ContentWrapper.GetTexture( "LayerThree" );

            CursorTexture = ContentWrapper.GetTexture( "CursorTexture" );

            FPSFont = ContentWrapper.GetFont( "fpsfont" );
        }
    }
}
