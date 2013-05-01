using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Circular.Display;
using Circular.Display.Screens;
using Circular.Entity;
using Circular.Helpers;
using Circular.Levels;
using Circular.Managers;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Circular;
using Circular.Display;
using Circular.Managers;
using Circular.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circular {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CircularGame : Game {

        /// <summary>
        /// Gets the version.
        /// </summary>
        public Version Version {
            get {
                return Assembly.GetAssembly( typeof( CircularGame ) ).GetName().Version;
            }
        }

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public SpriteManager SpriteManager;

        public Camera Camera;
        public HUD HUD;

        public World PhysicsWorld;

        private List<GameScreen> _screens = new List<GameScreen>();
        private List<GameScreen> _screensToUpdate = new List<GameScreen>();

        private List<RenderTarget2D> _transitions = new List<RenderTarget2D>();
        private List<RenderTarget2D> _previews = new List<RenderTarget2D>();

        private QuadRenderer _quadRenderer;
        private LineBatch _lineBatch;

        private MenuScreen _menuScreen;

        public ContentManager TextureManager;

        private InputHelper _input;

        private DebugViewXNA _debugView;

        private ParallaxingBackgrounds background;

        public readonly int Width;
        public readonly int Height;

        private bool _isExiting;

        public CircularGame () {
            Graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            ConvertUnits.SetDisplayUnitToSimUnitRatio( 24f );

            PhysicsWorld = new World( PhysicsUtils.EarthGravity );

            Graphics.PreferredBackBufferHeight = 600;
            Graphics.PreferredBackBufferWidth = 1200;

            Width = 1200;
            Height = 600;

#if !DEBUG
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.PreferredBackBufferWidth = 1650;
            Graphics.PreferMultiSampling = false;
            Graphics.IsFullScreen = true;

            Width = 1650;
            Height = 1080;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize () {
#if DEBUG
            this.Window.Title = "Circular (Debug Mode) - v" + Version;
            this.IsMouseVisible = false;
#else
            this.Window.Title = "Circular - v" + Version;
#endif

            ContentWrapper.Initialize( this );

            Camera = new Camera( GraphicsDevice );
            HUD = new HUD( this );
            SpriteBatch = new SpriteBatch( GraphicsDevice );

            SpriteManager = new SpriteManager();

            _input = new InputHelper( this );

            HUD.HUDObjects.Add( new CursorComponent( this ) );
            HUD.HUDObjects.Add( new FPSComponent( this ) );

            Components.Add( HUD );

            _isExiting = false;

            background = new ParallaxingBackgrounds( this );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent () {
            background.Init();

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch( GraphicsDevice );
            _quadRenderer = new QuadRenderer( GraphicsDevice );
            _lineBatch = new LineBatch( GraphicsDevice );

            TextureManager = new ContentManager( this );
            HUD.Initialize();

            _input.LoadContent( GraphicsDevice.Viewport );

            SetupDebug();
            // Create rendertarget for transitions
            PresentationParameters _pp = GraphicsDevice.PresentationParameters;
            _transitions.Add( new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight, false,
                                                SurfaceFormat.Color, _pp.DepthStencilFormat, _pp.MultiSampleCount,
                                                RenderTargetUsage.DiscardContents ) );

            _menuScreen = new MenuScreen();

            Assembly SamplesFramework = Assembly.GetExecutingAssembly();
            foreach ( LevelBase DemoScreen in ( from SampleType in SamplesFramework.GetTypes()
                                                where SampleType.IsSubclassOf( typeof( LevelBase ) )
                                                select SamplesFramework.CreateInstance( SampleType.ToString() ) ).OfType<LevelBase>() ) {
#if WINDOWS
                Console.WriteLine( "Loading demo: " + DemoScreen.GetTitle() );
#endif
                RenderTarget2D preview = new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth / 2, _pp.BackBufferHeight / 2, false,
                                                             SurfaceFormat.Color, _pp.DepthStencilFormat, _pp.MultiSampleCount,
                                                             RenderTargetUsage.DiscardContents );

                DemoScreen.Framework = this;
                DemoScreen.IsExiting = false;

                DemoScreen.Sprites = SpriteBatch;
                DemoScreen.Quads = _quadRenderer;
                DemoScreen.Lines = _lineBatch;

                DemoScreen.LoadContent();

                // "Abuse" transition rendertarget to render screen preview
                GraphicsDevice.SetRenderTarget( _transitions[ 0 ] );
                GraphicsDevice.Clear( Color.Transparent );

                _quadRenderer.Begin();
                _quadRenderer.Render( Vector2.Zero, new Vector2( _transitions[ 0 ].Width, _transitions[ 0 ].Height ), null, true, ContentWrapper.Grey, Color.White * 0.3f );
                _quadRenderer.End();
                // Update ensures that the screen is fully visible, we "cover" it so that no physics are run
                DemoScreen.Update( new GameTime( DemoScreen.TransitionOnTime, DemoScreen.TransitionOnTime ), true, false );
                DemoScreen.Draw( new GameTime() );
                DemoScreen.Draw( new GameTime() );

                GraphicsDevice.SetRenderTarget( preview );
                GraphicsDevice.Clear( Color.Transparent );

                SpriteBatch.Begin();
                SpriteBatch.Draw( _transitions[ 0 ], preview.Bounds, Color.White );
                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget( null );

                DemoScreen.ExitScreen();
                DemoScreen.Update( new GameTime( DemoScreen.TransitionOffTime, DemoScreen.TransitionOffTime ), true, false );
                _menuScreen.AddMenuItem( DemoScreen, preview );
            }

            //AddScreen( new BackgroundScreen() );
            AddScreen( _menuScreen );
            AddScreen( new LogoScreen( TimeSpan.FromSeconds( 5.0 ) ) );

            ResetElapsedTime();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent () {
            foreach ( GameScreen screen in _screens ) {
                screen.UnloadContent();
            }
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        [DebuggerStepThrough]
        protected override void Update ( GameTime gameTime ) {
            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.F12 ) )
                this.Exit();
            _input.Update( gameTime );

            if ( ( _input.IsNewButtonPress( Buttons.Y ) || _input.IsNewKeyPress( Keys.F5 ) ) &&
                !( _screens[ _screens.Count - 1 ] is OptionsScreen || _screens[ _screens.Count - 1 ] is LogoScreen ) ) {
                AddScreen( new OptionsScreen() );
            }

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();
            _screensToUpdate.AddRange( _screens );

            bool otherScreenHasFocus = !IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while ( _screensToUpdate.Count > 0 ) {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[ _screensToUpdate.Count - 1 ];

                _screensToUpdate.RemoveAt( _screensToUpdate.Count - 1 );

                // Update the screen.
                screen.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );

                if ( screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active ) {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if ( !otherScreenHasFocus && !_isExiting ) {
                        _input.ShowCursor = screen.HasCursor;
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

            if ( _isExiting && _screens.Count == 0 ) {
                Exit();
            }

            PhysicsWorld.Step( (float) gameTime.ElapsedGameTime.TotalMilliseconds * .001f );
            SpriteManager.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw ( GameTime gameTime ) {

            int transitionCount = 0;
            foreach ( GameScreen screen in _screens ) {
                if ( screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.TransitionOff ) {
                    transitionCount++;
                    if ( _transitions.Count < transitionCount ) {
                        PresentationParameters _pp = GraphicsDevice.PresentationParameters;
                        _transitions.Add( new RenderTarget2D( GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight, false,
                                                            SurfaceFormat.Color, _pp.DepthStencilFormat, _pp.MultiSampleCount,
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

                if ( screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.TransitionOff ) {
                    SpriteBatch.Begin( 0, BlendState.AlphaBlend );
                    if ( screen is LevelBase ) {
                        Vector2 position = Vector2.Lerp( _menuScreen.PreviewPosition, new Vector2( GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height ) / 2f, 1f - screen.TransitionPosition );
                        SpriteBatch.Draw( _transitions[ transitionCount ], position, null, Color.White * Math.Min( screen.TransitionAlpha / 0.2f, 1f ), 0f,
                                          new Vector2( _transitions[ transitionCount ].Width, _transitions[ transitionCount ].Height ) / 2f, 0.5f + 0.5f * ( 1f - screen.TransitionPosition ), SpriteEffects.None, 0f );
                    }
                    else {
                        SpriteBatch.Draw( _transitions[ transitionCount ], Vector2.Zero, Color.White * screen.TransitionAlpha );
                    }
                    SpriteBatch.End();

                    transitionCount++;
                }
                else {
                    screen.Draw( gameTime );
                }
            }

            _input.Draw( gameTime );


            base.Draw( gameTime );
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen ( GameScreen screen ) {
            screen.Framework = this;
            screen.IsExiting = false;

            screen.Sprites = SpriteBatch;
            screen.Lines = _lineBatch;
            screen.Quads = _quadRenderer;

            // Tell the screen to load content.
            screen.LoadContent();
            // Loading my take a while so elapsed time is reset to prevent hick-ups
            ResetElapsedTime();
            _screens.Add( screen );
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen ( GameScreen screen ) {
            // Tell the screen to unload content.
            screen.UnloadContent();
            _screens.Remove( screen );
            _screensToUpdate.Remove( screen );
        }

        public void ExitGame () {
            foreach ( GameScreen screen in _screens ) {
                screen.ExitScreen();
            }
            _isExiting = true;
        }

        #region DEBUG

        private void SetupDebug () {
            // create and configure the debug view
            _debugView = new DebugViewXNA( PhysicsWorld );

            _debugView.AppendFlags( DebugViewFlags.PerformanceGraph );
            _debugView.AppendFlags( DebugViewFlags.CenterOfMass );
            _debugView.AppendFlags( DebugViewFlags.Shape );
            _debugView.AppendFlags( DebugViewFlags.DebugPanel );
            //_debugView.AppendFlags(DebugViewFlags.AABB);

            _debugView.DefaultShapeColor = Color.White;
            _debugView.SleepingShapeColor = Color.LightGray;
            _debugView.LoadContent( GraphicsDevice, Content );
        }

        private void DrawDebugData () {
            Matrix proj = Matrix.CreateOrthographicOffCenter( 0f, HUD.Width, HUD.Height, 0f, 0f, 1f );
            Matrix view2 = Matrix.CreateScale( 64 );
            view2 *= Camera.View;
            _debugView.RenderDebugData( ref proj, ref view2 );
        }

        #endregion
    }
}
