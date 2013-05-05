using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Helpers {
    public class ContentHelper : GameComponent {
        public static Color Gold = new Color ( 246, 187, 53 );
        public static Color Red = new Color ( 215, 1, 51 );
        public static Color Green = new Color ( 102, 158, 68 );
        public static Color Orange = new Color ( 218, 114, 44 );
        public static Color Brown = new Color ( 123, 40, 11 );

        public static Color Beige = new Color ( 233, 229, 217 );
        public static Color Cream = new Color ( 246, 87, 84 );
        public static Color Lime = new Color ( 146, 201, 43 );
        public static Color Teal = new Color ( 66, 126, 120 );
        public static Color Grey = new Color ( 73, 69, 69 );

        public static Color Black = new Color ( 28, 19, 11 );
        public static Color Sunset = new Color ( 194, 73, 24 );
        public static Color Sky = new Color ( 185, 216, 221 );

        public static Color Cyan = new Color ( 50, 201, 251 );
        public static Color Blue = new Color ( 44, 138, 153 );
        public static Color Ocean = new Color ( 57, 143, 171 );


        private static readonly Dictionary < string, Texture2D > _textureList = new Dictionary < string, Texture2D > ();
        private static readonly Dictionary < string, SpriteFont > _fontList = new Dictionary < string, SpriteFont > ();
        private static readonly Dictionary < string, Effect > _effectList = new Dictionary < string, Effect > ();
        private static readonly Dictionary < string, SoundEffect > _soundList = new Dictionary < string, SoundEffect > ();

        private static ContentHelper _contentHelper;
        private static BasicEffect _effect;

        private static int _soundVolume;

        protected ContentHelper ( Game game )
            : base ( game ) {
            DirectoryInfo currentAssetFolder;
            FileInfo[] currentFileList;

            // First create a blank texture
            _textureList ["blank"] = new Texture2D ( game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color );
            _textureList ["blank"].SetData ( new[] { Color.White } );
            _textureList ["blank"].Name = "blank";

            // Load all graphics
            string[] gfxFolders = { "Images", "Images/Backgrounds/", "Images/Sprites/", "Images/UI/", "Images/Materials/", "Images/Maps/" };
            foreach ( string folder in gfxFolders ) {
                currentAssetFolder = new DirectoryInfo ( game.Content.RootDirectory + "/" + folder );
                currentFileList = currentAssetFolder.GetFiles ( "*.xnb" );
                for ( int i = 0; i < currentFileList.Length; i++ ) {
                    string textureName = Path.GetFileNameWithoutExtension ( currentFileList [i].Name ).ToLower ();
                    _textureList [textureName] = game.Content.Load < Texture2D > ( folder + "/" + textureName );
                    _textureList [textureName].Name = textureName;
                }
            }

            // Load all effects
            currentAssetFolder = new DirectoryInfo ( game.Content.RootDirectory + "/Effects" );
            currentFileList = currentAssetFolder.GetFiles ( "*.xnb" );
            for ( int i = 0; i < currentFileList.Length; i++ ) {
                string effectName = Path.GetFileNameWithoutExtension ( currentFileList [i].Name ).ToLower ();
                _effectList [effectName] = game.Content.Load < Effect > ( "Effects/" + effectName );
                _effectList [effectName].Name = effectName;
            }


            // Add samples fonts
            currentAssetFolder = new DirectoryInfo ( game.Content.RootDirectory + "/Fonts" );
            currentFileList = currentAssetFolder.GetFiles ( "*.xnb" );

            for ( int i = 0; i < currentFileList.Length; i++ ) {
                string fontName = Path.GetFileNameWithoutExtension ( currentFileList [i].Name ).ToLower ();
                _fontList [fontName] = game.Content.Load < SpriteFont > ( "Fonts/" + fontName );
            }

            // Add basic effect for texture generation
            _effect = new BasicEffect ( game.GraphicsDevice );

            // Initialize audio playback
            if ( Directory.Exists ( game.Content.RootDirectory + "/Sounds" ) ) {
                currentAssetFolder = new DirectoryInfo ( game.Content.RootDirectory + "/Sounds" );
                currentFileList = currentAssetFolder.GetFiles ( "*.xnb" );

                for ( int i = 0; i < currentFileList.Length; i++ ) {
                    string soundName = Path.GetFileNameWithoutExtension ( currentFileList [i].Name ).ToLower ();
                    _soundList [soundName] = game.Content.Load < SoundEffect > ( "Sounds/" + soundName );
                    _soundList [soundName].Name = soundName;
                }

                try {
                    SoundVolume = 100;
                }
                catch ( NoAudioHardwareException ) {
                    // silently fall back to silence
                }
            }
        }

        public static int SoundVolume {
            get { return _soundVolume; }
            set {
                _soundVolume = (int) MathHelper.Clamp ( value, 0f, 100f );
                SoundEffect.MasterVolume = _soundVolume / 100f;
            }
        }


        public static void Initialize ( Game game ) {
            if ( _contentHelper == null && game != null ) {
                _contentHelper = new ContentHelper ( game );
                game.Components.Add ( _contentHelper );
            }
        }

        public static Texture2D GetTexture ( string textureName ) {
            textureName = textureName.ToLower ();

            if ( _contentHelper != null && _textureList.ContainsKey ( textureName ) ) {
                return _textureList [textureName];
            }
#if WINDOWS
            Console.WriteLine ( "Texture \"" + textureName + "\" not found!" );
#endif
            return null;
        }

        public static Effect GetEffect ( string effectName ) {
            effectName = effectName.ToLower ();

            if ( _contentHelper != null && _effectList.ContainsKey ( effectName ) ) {
                return _effectList [effectName];
            }
#if WINDOWS
            Console.WriteLine ( "Effect \"" + effectName + "\" not found!" );
#endif
            return null;
        }

        public static SpriteFont GetFont ( string fontName ) {
            if ( _contentHelper != null && _fontList.ContainsKey ( fontName ) ) {
                return _fontList [fontName];
            }
            throw new FileNotFoundException ();
        }
    }
}