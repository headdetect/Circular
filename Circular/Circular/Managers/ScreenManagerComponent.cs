using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Circular.Display;
using Circular.Display.Screens;
using Circular.Helpers;
using Circular.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Circular.Managers {
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent {
        private readonly InputHelper _input;

        private readonly List < GameScreen > _screens;
        private readonly List < GameScreen > _screensToUpdate;

        private readonly List < RenderTarget2D > _transitions;
        private readonly ContentManager _xnaContentManager;
        private AssetCreator _assetCreator;
        private bool _isInitialized;
        private LineBatch _lineBatch;
        private SpriteBatch _spriteBatch;

        private MenuScreen _menuScreen;


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager ( Game game )
            : base( game ) {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            _xnaContentManager = game.Content;
            _xnaContentManager.RootDirectory = "Content";
            _input = new InputHelper( this );

            _screens = new List<GameScreen>();
            _screensToUpdate = new List<GameScreen>();
            _transitions = new List<RenderTarget2D>();
        }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch {
            get { return _spriteBatch; }
        }

        public LineBatch LineBatch {
            get { return _lineBatch; }
        }

        public ContentManager Content {
            get { return _xnaContentManager; }
        }

        public AssetCreator Assets {
            get { return _assetCreator; }
        }

        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize () {
            base.Initialize();

            _isInitialized = true;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent () {
            ContentHelper.Initialize( Game );
            _spriteBatch = new SpriteBatch( GraphicsDevice );
            _lineBatch = new LineBatch( GraphicsDevice );
            _assetCreator = new AssetCreator( GraphicsDevice );
            _assetCreator.LoadContent();
            _input.LoadContent();

            PresentationParameters _pp = GraphicsDevice.PresentationParameters;
            _transitions.Add( new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight, false,
                                                SurfaceFormat.Color, _pp.DepthStencilFormat, _pp.MultiSampleCount,
                                                RenderTargetUsage.DiscardContents ) );

            _menuScreen = new MenuScreen();

            _menuScreen.AddMenuItem( "Levels", EntryType.Separator, null );

            var assembly = Assembly.GetExecutingAssembly();
            foreach ( LevelBase level in ( from SampleType in assembly.GetTypes()
                                           where SampleType.IsSubclassOf( typeof( LevelBase ) )
                                           select assembly.CreateInstance( SampleType.ToString() ) ).OfType<LevelBase>() ) {

                RenderTarget2D preview = new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth / 2, _pp.BackBufferHeight / 2, false,
                                                             SurfaceFormat.Color, _pp.DepthStencilFormat, _pp.MultiSampleCount,
                                                             RenderTargetUsage.DiscardContents );

                level.ScreenManager = this;
                level.LoadContent();

                // "Abuse" transition rendertarget to render screen preview
                GraphicsDevice.SetRenderTarget( _transitions[ 0 ] );
                GraphicsDevice.Clear( Color.Transparent );

                level.Update( new GameTime( level.TransitionOnTime, level.TransitionOnTime ), true, false );
                level.Draw(new GameTime());
                level.Draw(new GameTime());

                GraphicsDevice.SetRenderTarget( preview );
                GraphicsDevice.Clear( Color.Transparent );

                SpriteBatch.Begin();
                SpriteBatch.Draw( _transitions[ 0 ], preview.Bounds, Color.White );
                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget( null );
                level.ExitScreen();

                level.Update( new GameTime( level.TransitionOffTime, level.TransitionOffTime ), true, false );

                _menuScreen.AddLevelItem( new LevelTutorial(), preview );
            }

            _menuScreen.AddMenuItem( "", EntryType.Separator, null );
            _menuScreen.AddMenuItem( "Exit", EntryType.ExitItem, null );

            AddScreen( new BackgroundScreen() );
            AddScreen( _menuScreen );
            AddScreen( new LogoScreen( TimeSpan.FromSeconds( 3.0 ) ) );

            // Tell each of the screens to load their content.
            foreach ( GameScreen screen in _screens ) {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent () {
            // Tell each of the screens to unload their content.
            foreach ( GameScreen screen in _screens ) {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update ( GameTime gameTime ) {
            // Read the keyboard and gamepad.
            _input.Update( gameTime );

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach ( GameScreen screen in _screens ) {
                _screensToUpdate.Add( screen );
            }

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while ( _screensToUpdate.Count > 0 ) {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[ _screensToUpdate.Count - 1 ];

                _screensToUpdate.RemoveAt( _screensToUpdate.Count - 1 );

                // Update the screen.
                screen.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );

                if ( screen.ScreenState == ScreenState.TransitionOn ||
                     screen.ScreenState == ScreenState.Active ) {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if ( !otherScreenHasFocus ) {
                        _input.ShowCursor = screen.HasCursor;
                        _input.EnableVirtualStick = screen.HasVirtualStick;
                        screen.HandleInput( _input, gameTime );
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if ( !screen.IsPopup ) {
                        coveredByOtherScreen = true;
                    }
                }
            }
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw ( GameTime gameTime ) {
            int transitionCount = 0;
            foreach ( GameScreen screen in _screens ) {
                if ( screen.ScreenState == ScreenState.TransitionOn ||
                     screen.ScreenState == ScreenState.TransitionOff ) {
                    ++transitionCount;
                    if ( _transitions.Count < transitionCount ) {
                        PresentationParameters _pp = GraphicsDevice.PresentationParameters;
                        _transitions.Add( new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight,
                                                                false,
                                                                SurfaceFormat.Color, _pp.DepthStencilFormat,
                                                                _pp.MultiSampleCount,
                                                                RenderTargetUsage.DiscardContents ) );
                    }
                    GraphicsDevice.SetRenderTarget( _transitions[ transitionCount - 1 ] );
                    GraphicsDevice.Clear( Color.Transparent );
                    screen.Draw( gameTime );
                    GraphicsDevice.SetRenderTarget( null );
                }
            }

            GraphicsDevice.Clear( Color.Black );

            transitionCount = 0;
            foreach ( GameScreen screen in _screens ) {
                if ( screen.ScreenState == ScreenState.Hidden ) {
                    continue;
                }

                if ( screen.ScreenState == ScreenState.TransitionOn ||
                     screen.ScreenState == ScreenState.TransitionOff ) {
                    _spriteBatch.Begin( 0, BlendState.AlphaBlend );
                    _spriteBatch.Draw( _transitions[ transitionCount ], Vector2.Zero, Color.White * screen.TransitionAlpha );
                    _spriteBatch.End();

                    ++transitionCount;
                }
                else {
                    screen.Draw( gameTime );
                }
            }
            _input.Draw();
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen ( GameScreen screen ) {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if ( _isInitialized ) {
                screen.LoadContent();
            }

            _screens.Add( screen );

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen ( GameScreen screen ) {
            // If we have a graphics device, tell the screen to unload content.
            if ( _isInitialized ) {
                screen.UnloadContent();
            }

            _screens.Remove( screen );
            _screensToUpdate.Remove( screen );

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if ( _screens.Count > 0 ) {
                TouchPanel.EnabledGestures = _screens[ _screens.Count - 1 ].EnabledGestures;
            }
        }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens () {
            return _screens.ToArray();
        }
    }
}