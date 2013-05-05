using System;
using Circular.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display.Screens {
    public enum EntryType {
        Screen,
        Separator,
        ExitItem
    }

    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public sealed class MenuEntry {
        private readonly MenuScreen _menu;
        private readonly GameScreen _screen;
        private readonly EntryType _type;
        public bool IsHovered;
        public bool IsSelected;
        private float _alpha;
        private Vector2 _baseOrigin;

        private float _height;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        private Vector2 _position;

        private float _scale;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        private string _text;

        private float _width;

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry ( MenuScreen menu, string text, EntryType type, GameScreen screen, Texture2D preview ) {
            _text = text;
            _screen = screen;
            _type = type;
            _menu = menu;
            _scale = 0.9f;
            _alpha = 1.0f;
            Preview = preview;
        }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry ( MenuScreen menu, string text, EntryType type, GameScreen screen )
            : this ( menu, text, type, screen, null ) {}

        public Texture2D Preview { get; set; }


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }

        public float Alpha {
            get { return _alpha; }
            set { _alpha = value; }
        }

        public GameScreen Screen {
            get { return _screen; }
        }

        public void Initialize () {
            SpriteFont font = ContentHelper.GetFont ( "menufont" );

            _baseOrigin = new Vector2 ( font.MeasureString ( Text ).X, font.MeasureString ( "|" ).Y ) * 0.5f;

            _width = font.MeasureString ( Text ).X * 0.8f;
            _height = font.MeasureString ( "|" ).Y * 0.8f;
        }

        public bool IsExitItem () {
            return _type == EntryType.ExitItem;
        }

        public bool IsSelectable () {
            return _type != EntryType.Separator;
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update ( GameTime gameTime ) {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            if ( _type != EntryType.Separator ) {
                float fadeSpeed = (float) gameTime.ElapsedGameTime.TotalSeconds * 4;
                _selectionFade = IsSelected || IsHovered ? Math.Min ( _selectionFade + fadeSpeed, 1f ) : Math.Max ( _selectionFade - fadeSpeed, 0f );
                _scale = 0.7f + 0.1f * _selectionFade;
            }
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public void Draw () {
            SpriteFont font = ContentHelper.GetFont ( "menufont" );
            SpriteBatch batch = _menu.ScreenManager.SpriteBatch;

            Color color = _type == EntryType.Separator ? Color.DarkOrange : Color.Lerp ( Color.White, new Color ( 255, 210, 0 ), _selectionFade );
            color *= _alpha;

            // Draw text, centered on the middle of each line.
            batch.DrawString ( font, _text, _position - _baseOrigin * _scale + Vector2.One,
                               Color.DarkSlateGray * _alpha * _alpha, 0, Vector2.Zero, _scale, SpriteEffects.None, 0 );
            batch.DrawString ( font, _text, _position - _baseOrigin * _scale, color, 0, Vector2.Zero, _scale,
                               SpriteEffects.None, 0 );
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public int GetHeight () {
            return (int) _height;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public int GetWidth () {
            return (int) _width;
        }
    }
}