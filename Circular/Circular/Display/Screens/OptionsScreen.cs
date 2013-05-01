using System;
using System.Collections.Generic;
using Circular.Helpers;
using Circular.Utils;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Circular.Levels;

namespace Circular.Display.Screens {
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class OptionsScreen : GameScreen {
        private const float HorizontalPadding = 24f;
        private const float VerticalPadding = 24f;
        private const int EntrySpacer = 5;

        private Vector2 _optionEntrySize;
        private Vector2 _optionTextOffset;
        private Vector2 _optionCheckOffset;
        private float _optionStart;
        private float _optionSpacing;

        private Vector2 _topLeft;
        private Vector2 _bottomRight;

        private List<OptionEntry> _optionEntries = new List<OptionEntry>();

        private int _hoverEntry;

        private SpriteFont _font;
        private Texture2D _checkmark;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsScreen () {
            IsPopup = true;
            HasCursor = true;

            TransitionOnTime = TimeSpan.FromSeconds( 0.4 );
            TransitionOffTime = TimeSpan.FromSeconds( 0.4 );

            _optionEntrySize = Vector2.Zero;
            _hoverEntry = -1;
        }

        private void LoadOptions () {
            _optionEntries.Add( new OptionEntry( "Show debug view", LevelBase.RenderDebug ) );
            _optionEntries.Add( new OptionEntry( "Debug draw data panel", ( LevelBase.Flags & DebugViewFlags.DebugPanel ) == DebugViewFlags.DebugPanel ) );
            _optionEntries.Add( new OptionEntry( "Debug draw performance graph", ( LevelBase.Flags & DebugViewFlags.PerformanceGraph ) == DebugViewFlags.PerformanceGraph ) );
            _optionEntries.Add( new OptionEntry( "Debug draw shapes", ( LevelBase.Flags & DebugViewFlags.Shape ) == DebugViewFlags.Shape ) );
            _optionEntries.Add( new OptionEntry( "Debug draw polygon vertices", ( LevelBase.Flags & DebugViewFlags.PolygonPoints ) == DebugViewFlags.PolygonPoints ) );
            _optionEntries.Add( new OptionEntry( "Debug draw joint connections", ( LevelBase.Flags & DebugViewFlags.Joint ) == DebugViewFlags.Joint ) );
            _optionEntries.Add( new OptionEntry( "Debug draw axis aligned bounding boxes", ( LevelBase.Flags & DebugViewFlags.AABB ) == DebugViewFlags.AABB ) );
            _optionEntries.Add( new OptionEntry( "Debug draw center of mass", ( LevelBase.Flags & DebugViewFlags.CenterOfMass ) == DebugViewFlags.CenterOfMass ) );
            _optionEntries.Add( new OptionEntry( "Debug draw contact points", ( LevelBase.Flags & DebugViewFlags.ContactPoints ) == DebugViewFlags.ContactPoints ) );
            _optionEntries.Add( new OptionEntry( "Debug draw contact normals", ( LevelBase.Flags & DebugViewFlags.ContactNormals ) == DebugViewFlags.ContactNormals ) );
            _optionEntries.Add( new OptionEntry( "Debug draw controllers", ( LevelBase.Flags & DebugViewFlags.Controllers ) == DebugViewFlags.Controllers ) );
            _optionEntries.Add( new OptionEntry( "Play sound effects", ContentWrapper.SoundVolume == 100 ) );

            for ( int i = 0; i < _optionEntries.Count; i++ ) {
                _optionEntrySize.X = Math.Max( _optionEntrySize.X, _optionEntries[ i ].Size.X );
                _optionEntrySize.Y = Math.Max( _optionEntrySize.Y, _optionEntries[ i ].Size.Y );
            }
            _optionEntrySize.X += 20f + _optionEntrySize.Y;
            _optionTextOffset = new Vector2( -_optionEntrySize.Y / 2f, 0f );
            _optionCheckOffset = new Vector2( _optionEntrySize.X - _optionEntrySize.Y, 0f );
        }

        public override void LoadContent () {
            base.LoadContent();

            LoadOptions();

            Viewport viewport = Framework.GraphicsDevice.Viewport;

            _font = ContentWrapper.GetFont( "menuFont" );
            _checkmark = ContentWrapper.GetTexture( "checkmark" );

            _optionStart = ( viewport.Height - ( _optionEntries.Count - 1 ) * ( _optionEntrySize.Y + EntrySpacer ) ) / 2f;
            _optionSpacing = _optionEntrySize.Y + EntrySpacer;

            for ( int i = 0; i < _optionEntries.Count; i++ ) {
                _optionEntries[ i ].InitializePosition( new Vector2( viewport.Width / 2f, _optionStart + _optionSpacing * i ) );
            }

            // The background includes a border somewhat larger than the text itself.
            _topLeft.X = viewport.Width / 2f - _optionEntrySize.X / 2f - HorizontalPadding;
            _topLeft.Y = _optionStart - _optionEntrySize.Y / 2f - VerticalPadding;
            _bottomRight.X = viewport.Width / 2f + _optionEntrySize.X / 2f + HorizontalPadding;
            _bottomRight.Y = _optionStart + ( _optionEntries.Count - 1 ) * _optionSpacing + _optionEntrySize.Y / 2f + VerticalPadding;
        }

        /// <summary>
        /// Returns the index of the menu entry at the position of the given mouse state.
        /// </summary>
        /// <returns>Index of menu entry if valid, -1 otherwise</returns>
        private int GetOptionEntryAt ( Vector2 position ) {
            for ( int i = 0; i < _optionEntries.Count; i++ ) {
                Rectangle boundingBox = new Rectangle( (int) ( _optionEntries[ i ].Position.X - _optionEntrySize.X / 2f ), (int) ( _optionEntries[ i ].Position.Y - _optionEntrySize.Y / 2f ), (int) _optionEntrySize.X, (int) _optionEntrySize.Y );
                if ( boundingBox.Contains( (int) position.X, (int) position.Y ) ) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput ( InputHelper input, GameTime gameTime ) {
            // Mouse on a menu item
            _hoverEntry = GetOptionEntryAt( input.Cursor );

            // Accept or cancel the menu? 
            if ( input.IsMenuSelect() ) {
                if ( _hoverEntry != -1 ) {
                    _optionEntries[ _hoverEntry ].Switch();

                    switch ( _hoverEntry ) {
                        case 0:
                            LevelBase.RenderDebug = _optionEntries[ _hoverEntry ].IsChecked;
                            break;
                        case 1:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.DebugPanel : LevelBase.Flags & ~DebugViewFlags.DebugPanel;
                            break;
                        case 2:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.PerformanceGraph : LevelBase.Flags & ~DebugViewFlags.PerformanceGraph;
                            break;
                        case 3:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.Shape : LevelBase.Flags & ~DebugViewFlags.Shape;
                            break;
                        case 4:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.PolygonPoints : LevelBase.Flags & ~DebugViewFlags.PolygonPoints;
                            break;
                        case 5:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.Joint : LevelBase.Flags & ~DebugViewFlags.Joint;
                            break;
                        case 6:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.AABB : LevelBase.Flags & ~DebugViewFlags.AABB;
                            break;
                        case 7:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.CenterOfMass : LevelBase.Flags & ~DebugViewFlags.CenterOfMass;
                            break;
                        case 8:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.ContactPoints : LevelBase.Flags & ~DebugViewFlags.ContactPoints;
                            break;
                        case 9:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.ContactNormals : LevelBase.Flags & ~DebugViewFlags.ContactNormals;
                            break;
                        case 10:
                            LevelBase.Flags = _optionEntries[ _hoverEntry ].IsChecked ? LevelBase.Flags | DebugViewFlags.Controllers : LevelBase.Flags & ~DebugViewFlags.Controllers;
                            break;
                        case 11:
                            ContentWrapper.SoundVolume = _optionEntries[ _hoverEntry ].IsChecked ? 100 : 0;
                            break;
                    }
                    ContentWrapper.PlaySoundEffect( "click" );
                }
            }

            if ( input.IsScreenExit() ) {
                ExitScreen();
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update ( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen ) {
            base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );

            // Update each nested MenuEntry object.
            for ( int i = 0; i < _optionEntries.Count; i++ ) {
                bool isHovered = IsActive && ( i == _hoverEntry );
                _optionEntries[ i ].Update( isHovered, gameTime );
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw ( GameTime gameTime ) {
            Quads.Begin();
            Quads.Render( _topLeft, _bottomRight, null, true, ContentWrapper.Black, ContentWrapper.Grey * 0.95f );
            Quads.End();

            // Draw each menu entry in turn.
            Quads.Begin();
            foreach ( OptionEntry entry in _optionEntries ) {
                Quads.Render( entry.Position - _optionEntrySize / 2f, entry.Position + _optionEntrySize / 2f, null, true,
                             ContentWrapper.Black * TransitionAlpha, entry.TileColor * TransitionAlpha );
                Quads.Render( entry.Position - _optionEntrySize / 2f + _optionCheckOffset, entry.Position + _optionEntrySize / 2f, null, true,
                             ContentWrapper.Black * TransitionAlpha, entry.TileColor * TransitionAlpha );
            }
            Quads.End();

            Sprites.Begin();
            foreach ( OptionEntry entry in _optionEntries ) {
                Sprites.DrawString( _font, entry.Text, entry.Position + Vector2.One + _optionTextOffset, ContentWrapper.Black * TransitionAlpha,
                                   0f, entry.Origin, entry.Scale, SpriteEffects.None, 0f );
                Sprites.DrawString( _font, entry.Text, entry.Position + _optionTextOffset, entry.TextColor * TransitionAlpha,
                                   0f, entry.Origin, entry.Scale, SpriteEffects.None, 0f );
                Sprites.Draw( _checkmark, entry.Position - _optionEntrySize / 2f + _optionCheckOffset, Color.White * entry.CheckedFade );
            }
            Sprites.End();
        }
    }
}