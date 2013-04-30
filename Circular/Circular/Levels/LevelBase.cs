using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Circular.Levels {
    public abstract class LevelBase {

        public abstract void Init ();
        public abstract void Update ( GameTime gameTime );
        public abstract void Draw ( GameTime gameTime );

        protected CircularGame Game;

        protected LevelBase ( CircularGame game ) {
            Game = game;
        }

    }
}
