using System;
using System.Collections.Generic;
using Circular.Helpers;
using Circular.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display.Screens {
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class MenuScreen : GameScreen {
#if WINDOWS || XBOX
        private const float NumEntries = 13;
#elif WINDOWS_PHONE
        private const float NumEntries = 6;
#endif
        private const int PADDING = 8;

        private readonly List < MenuEntry > _menuEntries = new List < MenuEntry > ();
        private int _selectedEntry;
        private float _menuBorderTop;
        private float _menuBorderBottom;
        private float _menuBorderMargin;
        private float _menuOffset;
        private float _maxOffset;

        private Texture2D _texScrollButton;
        private Texture2D _texSlider;
        private Texture2D _texHeader;
        private Texture2D _texBG;

        private MenuButton _scrollUp;
        private MenuButton _scrollDown;
        private MenuButton _scrollSlider;
        private bool _scrollLock;

        private Rectangle _headerRegion;
        private Vector2 _headerPos;

        private Vector2 _previewPosition;
        private Vector2 _previewOrigin;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen () {
            TransitionOnTime = TimeSpan.FromSeconds ( 0.7 );
            TransitionOffTime = TimeSpan.FromSeconds ( 0.7 );
            HasCursor = true;
        }

        public void AddMenuItem ( string name, EntryType type, GameScreen screen ) {
            var entry = new MenuEntry ( this, name, type, screen );
            _menuEntries.Add ( entry );
        }

        public void AddLevelItem ( LevelBase level ) {
            var entry = new MenuEntry ( this, level.GetTitle (), EntryType.Screen, level, ContentHelper.GetTexture ( "logo" ) );
            _menuEntries.Add ( entry );
        }

        public override void LoadContent () {
            base.LoadContent ();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteFont font = ContentHelper.GetFont ( "menufont" );

            _texScrollButton = ContentHelper.GetTexture ( "arrow" );
            _texSlider = ContentHelper.GetTexture ( "slider" );

            float scrollBarPos = _texScrollButton.Width + PADDING;
            for ( int i = 0; i < _menuEntries.Count; ++i ) {
                _menuEntries [i].Initialize ();
            }

            _texHeader = ContentHelper.GetTexture ( "header" );
            _texBG = ContentHelper.GetTexture ( "blank" );

            _headerRegion = new Rectangle ( 0, 0, viewport.Width, PADDING + _texHeader.Height + PADDING );
            _headerPos = new Vector2 ( viewport.Width / 2 - _texHeader.Width / 2, PADDING );

            _menuBorderMargin = font.MeasureString ( "M" ).Y * 0.8f;
            _menuBorderTop = PADDING + ( ( viewport.Height - _menuBorderMargin * ( NumEntries - 1 ) ) / 2f ) + PADDING;
            _menuBorderBottom = ( viewport.Height + _menuBorderMargin * ( NumEntries - 1 ) ) / 2f;

            _menuOffset = 0f;
            _maxOffset = Math.Max ( 0f, ( _menuEntries.Count - NumEntries ) * _menuBorderMargin );

            _previewOrigin = new Vector2 ( viewport.Width / 4f, viewport.Height / 4f );
            _previewPosition = new Vector2 ( viewport.Width - _previewOrigin.X, ( viewport.Height - _headerRegion.Height ) / 2f + _headerRegion.Height );

            _scrollUp = new MenuButton ( _texScrollButton, false, new Vector2 ( scrollBarPos, _menuBorderTop - _texScrollButton.Height ), this );
            _scrollDown = new MenuButton ( _texScrollButton, true, new Vector2 ( scrollBarPos, _menuBorderBottom + _texScrollButton.Height ), this );
            _scrollSlider = new MenuButton ( _texSlider, false, new Vector2 ( scrollBarPos, _menuBorderTop ), this );

            _scrollLock = false;
        }

        /// <summary>
        /// Returns the index of the menu entry at the position of the given mouse state.
        /// </summary>
        /// <returns>Index of menu entry if valid, -1 otherwise</returns>
        private int GetMenuEntryAt ( Vector2 position ) {
            int index = 0;
            foreach ( MenuEntry entry in _menuEntries ) {
                float width = entry.GetWidth ();
                float height = entry.GetHeight ();
                var rect = new Rectangle ( (int) ( entry.Position.X - width / 2f ),
                                           (int) ( entry.Position.Y - height / 2f ),
                                           (int) width, (int) height );
                if ( rect.Contains ( (int) position.X, (int) position.Y ) && entry.Alpha > 0.1f ) {
                    return index;
                }
                ++index;
            }
            return -1;
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput ( InputHelper input, GameTime gameTime ) {
            // Mouse or touch on a menu item
            int hoverIndex = GetMenuEntryAt ( input.Cursor );
            for ( int i = 0; i < _menuEntries.Count; i++ ) {
                MenuEntry item = _menuEntries [i];
                item.IsHovered = i == hoverIndex;
            }
            if ( hoverIndex > -1 && _menuEntries [hoverIndex].IsSelectable () && !_scrollLock ) {
                _selectedEntry = hoverIndex;
            }
            else {
                _selectedEntry = -1;
            }

            _scrollSlider.Hover = false;
            if ( input.IsCursorValid ) {
                _scrollUp.Collide ( input.Cursor );
                _scrollDown.Collide ( input.Cursor );
                _scrollSlider.Collide ( input.Cursor );
            }
            else {
                _scrollUp.Hover = false;
                _scrollDown.Hover = false;
                _scrollLock = false;
            }

            // Accept or cancel the menu? 
            if ( input.IsMenuSelect () && _selectedEntry != -1 ) {
                MenuEntry item = _menuEntries [_selectedEntry];
                if ( item.IsExitItem () ) {
                    ScreenManager.Game.Exit ();
                }
                else if ( item.Screen != null ) {
                    if ( item.IsSelected ) {
                        item.IsSelected = false;
                        ScreenManager.AddScreen ( item.Screen );
                        if ( item.Screen is LevelBase ) {
                            ScreenManager.AddScreen (
                                new MessageBoxScreen ( ( item.Screen as LevelBase ).GetDetails () ) );
                        }
                    }
                    else {
                        item.IsSelected = true;
                    }
                }
            }
            else if ( input.IsMenuCancel () ) {
                ScreenManager.Game.Exit ();
            }

            if ( input.IsMenuPressed () ) {
                if ( _scrollUp.Hover ) {
                    _menuOffset = Math.Max ( _menuOffset - 200f * (float) gameTime.ElapsedGameTime.TotalSeconds, 0f );
                    _scrollLock = false;
                }
                if ( _scrollDown.Hover ) {
                    _menuOffset = Math.Min ( _menuOffset + 200f * (float) gameTime.ElapsedGameTime.TotalSeconds, _maxOffset );
                    _scrollLock = false;
                }
                if ( _scrollSlider.Hover ) {
                    _scrollLock = true;
                }
            }
            if ( input.IsMenuReleased () ) {
                _scrollLock = false;
            }
            if ( _scrollLock ) {
                _scrollSlider.Hover = true;
                _menuOffset = Math.Max ( Math.Min ( ( ( input.Cursor.Y - _menuBorderTop ) / ( _menuBorderBottom - _menuBorderTop ) ) * _maxOffset, _maxOffset ), 0f );
            }
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations () {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = (float) Math.Pow ( TransitionPosition, 2 );

            Vector2 position = Vector2.Zero;
            position.Y = _menuBorderTop - _menuOffset;

            // update each menu entry's location in turn
            for ( int i = 0; i < _menuEntries.Count; ++i ) {
                position.X = PADDING + ( PADDING + _scrollDown.Position.X + _scrollDown.Sprite.Width + PADDING ) + PADDING * 3;
                if ( ScreenState == ScreenState.TransitionOn ) {
                    position.X -= transitionOffset * 256;
                }
                else {
                    position.X += transitionOffset * 256;
                }

                // set the entry's position
                _menuEntries [i].Position = position;

                if ( position.Y < _menuBorderTop ) {
                    _menuEntries [i].Alpha = 1f -
                                             Math.Min ( _menuBorderTop - position.Y, _menuBorderMargin ) / _menuBorderMargin;
                }
                else if ( position.Y > _menuBorderBottom ) {
                    _menuEntries [i].Alpha = 1f -
                                             Math.Min ( position.Y - _menuBorderBottom, _menuBorderMargin ) /
                                             _menuBorderMargin;
                }
                else {
                    _menuEntries [i].Alpha = 1f;
                }

                // move down for the next entry the size of this entry
                position.Y += _menuEntries [i].GetHeight ();
            }
            Vector2 scrollPos = _scrollSlider.Position;
            scrollPos.Y = MathHelper.Lerp ( _menuBorderTop, _menuBorderBottom, _menuOffset / _maxOffset );
            _scrollSlider.Position = scrollPos;
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update ( GameTime gameTime, bool otherScreenHasFocus,
                                      bool coveredByOtherScreen ) {
            base.Update ( gameTime, otherScreenHasFocus, coveredByOtherScreen );

            // Update each nested MenuEntry object.
            for ( int i = 0; i < _menuEntries.Count; ++i ) {
                _menuEntries [i].Update ( gameTime );
            }

            _scrollUp.Update ( gameTime );
            _scrollDown.Update ( gameTime );
            _scrollSlider.Update ( gameTime );
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw ( GameTime gameTime ) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations ();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ContentHelper.GetFont ( "menufont" );

            spriteBatch.Begin ();
            // Draw each menu entry in turn.
            for ( int i = 0; i < _menuEntries.Count; ++i ) {
                _menuEntries [i].Draw ();
                if ( _menuEntries [i].IsSelected && _menuEntries [i].Preview != null ) {
                    spriteBatch.Draw ( _menuEntries [i].Preview, _previewPosition, null, Color.White * 0.6f, 0f, _previewOrigin, 1f, SpriteEffects.None, 0f );
                }
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = new Vector2 ( 0f, (float) Math.Pow ( TransitionPosition, 2 ) * 100f );

            spriteBatch.Draw ( _texBG, _headerRegion, Color.FromNonPremultiplied ( 0xFF, 0xFF, 0xFF, 0x88 ) );
            spriteBatch.Draw ( _texHeader, _headerPos, Color.White );

            _scrollUp.Draw ();
            _scrollSlider.Draw ();
            _scrollDown.Draw ();
            spriteBatch.End ();
        }
    }
}