using FluxEngine;
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

        public ContentManager ( BaseFluxGame fluxGame ) {
            BG1 = fluxGame.Content.Load<Texture2D>( "Images/Backgrounds/LayerOne" );
            BG2 = fluxGame.Content.Load<Texture2D>( "Images/Backgrounds/LayerTwo" );
            BG3 = fluxGame.Content.Load<Texture2D>( "Images/Backgrounds/LayerThree" );

            CursorTexture = fluxGame.Content.Load<Texture2D>( "Images/CursorTexture" );

            FPSFont = fluxGame.Content.Load<SpriteFont>( "Fonts/fpsfont" );
        }
    }
}
