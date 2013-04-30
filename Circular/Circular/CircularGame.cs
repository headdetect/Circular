using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Circular.Display;
using Circular.Entity;
using Circular.Managers;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FluxEngine;
using FluxEngine.Display;
using FluxEngine.Managers;
using FluxEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circular {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CircularGame : BaseFluxGame {

        /// <summary>
        /// Gets the version.
        /// </summary>
        public Version Version {
            get {
                return Assembly.GetAssembly( typeof( CircularGame ) ).GetName().Version;
            }
        }

        public ContentManager TextureManager;

        private DebugViewXNA _debugView;

        private ParallaxingBackgrounds background;

        public readonly int Width;
        public readonly int Height;

        //-- Game Entities --//

        public CircularGame () {
            Graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            ConvertUnits.SetDisplayUnitToSimUnitRatio( 64f );

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

            Camera = new Camera( this );
            HUD = new HUD( this );

            HUD.HUDObjects.Add( new CursorComponent( this ) );
            HUD.HUDObjects.Add( new FPSComponent( this ) );

            Components.Add( HUD );

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

            TextureManager = new ContentManager( this );
            SpriteManager = new SpriteManager();
            HUD.Initialize();


            SetupDebug();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent () {
            // TODO: Unload any non ContentManager content here
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

            PhysicsWorld.Step( (float) gameTime.ElapsedGameTime.TotalMilliseconds * .001f );
            Camera.Update( gameTime );
            background.Update( gameTime );
            SpriteManager.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw ( GameTime gameTime ) {
            GraphicsDevice.Clear( Color.Black );

            SpriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.View );

            SpriteManager.Draw( gameTime );

            SpriteBatch.End();

            background.Draw( gameTime );

            DrawDebugData();

            base.Draw( gameTime );
        }

        #region DEBUG

        private void SetupDebug () {
            // create and configure the debug view
            _debugView = new DebugViewXNA( this, PhysicsWorld );

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
