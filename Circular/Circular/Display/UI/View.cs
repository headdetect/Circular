using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display.UI {
    public class View  {

        public Color BackgroundColor { get; set; }
        public Color ForgroundColor { get; set; }
        public Texture2D ForgroundImage { get; set; }
        protected CircularGame Game { get; set; }

        public Rectangle Bounds { get; set; }

        public int ID { get; internal set; }

        public bool Visible { get; set; }

        public int ZIndex { get; set; }

        public View ( CircularGame game ) {
            this.Game = game;
        }


        public virtual void Draw ( GameTime gameTime ) {
            if ( !Visible ) {
                return;
            }
        }

        public virtual void Update ( GameTime gameTime ) {
            if ( !Visible ) {
                return;
            }

        }

        public virtual void Init () {

        }

    }
}
