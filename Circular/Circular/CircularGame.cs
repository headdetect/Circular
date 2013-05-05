using System;
using Circular.Display;
using Circular.Display.Screens;
using Circular.Helpers;
using Circular.Levels;
using Circular.Managers;
using Circular.Utils;
using FarseerPhysics.SamplesFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular {

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CircularGame : Game {

        private readonly GraphicsDeviceManager _graphics;

        public ScreenManager ScreenManager { get; set; }

        public CircularGame () {
            Window.Title = "Farseer Samples Framework";
            _graphics = new GraphicsDeviceManager( this ) {
                PreferMultiSampling = true,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

#if WINDOWS || XBOX
            ConvertUnits.SetDisplayUnitToSimUnitRatio( 24f );
            IsFixedTimeStep = true;
#elif WINDOWS_PHONE
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f);
            IsFixedTimeStep = false;
#endif

#if WINDOWS
            _graphics.IsFullScreen = false;
#elif XBOX || WINDOWS_PHONE
            _graphics.IsFullScreen = true;
#endif

            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager( this );
            Components.Add( ScreenManager );

            FPSComponent frameRateCounter = new FPSComponent( ScreenManager ) { DrawOrder = 101 };
            Components.Add( frameRateCounter );
        }

    }
}