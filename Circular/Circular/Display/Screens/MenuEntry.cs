using System;
using Circular.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display.Screens
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen.
    /// </summary>
    public sealed class MenuEntry : IComparable
    {
        private const float MaxTranslation = 10f;
        private const double HighlightTime = 0.3;
        private const double FadeTime = 0.4;

        private static float _targetHiddenX;
        private static float _targetVisibleX;

        private float _targetY;
        private Vector2 _currentPosition;
        private Vector2 _size;
        private Color _color;
        private Color _textColor;

        private LevelBase _screen;
        private Texture2D _preview;
        private bool _visible;

        private double _hoverFade;
        private double _selectionFade;
        private double _visibleFade;

        private string _text;

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, LevelBase screen, Texture2D preview)
        {
            _text = text;
            _screen = screen;
            _preview = preview;

            _hoverFade = 0.0;
            _selectionFade = 0.0;

            SpriteFont font = ContentWrapper.GetFont("fpsfont");
            _size = font.MeasureString(text);
        }

        public static void InitializeEntries(float hiddenX, float visibleX)
        {
            _targetHiddenX = hiddenX;
            _targetVisibleX = visibleX;
        }

        public void InitializePosition(float target, bool visible)
        {
            _visible = visible;
            _visibleFade = visible ? 1.0 : 0.0;
            _currentPosition.X = visible ? _targetVisibleX : _targetHiddenX;
            _currentPosition.Y = _targetY = target;
        }

        public string Text
        {
            get { return _text; }
        }

        public Vector2 Position
        {
            get { return _currentPosition; }
        }

        public float Target
        {
            set { _targetY = value; }
        }

        public Vector2 Origin
        {
            get { return _size / 2f; }
        }

        public Vector2 Size
        {
            get { return _size; }
        }

        public float Fade
        {
            get { return (float)_selectionFade; }
        }

        public GameScreen Screen
        {
            get { return _screen; }
        }

        public Texture2D Preview
        {
            get { return _preview; }
        }

        public Color TextColor
        {
            get { return _textColor; }
        }

        public Color TileColor
        {
            get { return _color; }
        }

        public float Scale
        {
            get { return 0.9f + 0.1f * (float)_hoverFade; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public float Alpha
        {
            get { return (float)_visibleFade; }
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update(bool isSelected, bool isHovered, GameTime gameTime)
        {
            _hoverFade = isHovered ? Math.Min(_hoverFade + (gameTime.ElapsedGameTime.TotalSeconds / HighlightTime), 1.0) : Math.Max(_hoverFade - (gameTime.ElapsedGameTime.TotalSeconds / HighlightTime), 0.0);
            _selectionFade = isSelected ? Math.Min(_selectionFade + (gameTime.ElapsedGameTime.TotalSeconds / HighlightTime), 1.0) : Math.Max(_selectionFade - (gameTime.ElapsedGameTime.TotalSeconds / HighlightTime), 0.0);

            _textColor = Color.Lerp(ContentWrapper.Beige, ContentWrapper.Gold, (float)_selectionFade);
            _color = Color.Lerp(ContentWrapper.Sky * 0.6f, ContentWrapper.Grey * 0.6f, (float)Math.Max(_selectionFade, _hoverFade));

            _visibleFade = _visible ? Math.Min(_visibleFade + (gameTime.ElapsedGameTime.TotalSeconds / FadeTime), 1.0) : Math.Max(_visibleFade - (gameTime.ElapsedGameTime.TotalSeconds / FadeTime), 0.0);
            _currentPosition.X = MathHelper.SmoothStep(_targetHiddenX, _targetVisibleX, (float)_visibleFade);

            float deltaY = _targetY - _currentPosition.Y;
            if (Math.Abs(deltaY) > MaxTranslation)
            {
                _currentPosition.Y += MaxTranslation * Math.Sign(deltaY);
            }
            else
            {
                _currentPosition.Y += deltaY;
            }
        }

        #region IComparable Members
        public int CompareTo(object obj)
        {
            MenuEntry entry = obj as MenuEntry;
            return entry == null ? 0 : System.String.Compare(_screen.GetType().ToString(), entry._screen.GetType().ToString(), System.StringComparison.Ordinal);
        }
        #endregion
    }
}