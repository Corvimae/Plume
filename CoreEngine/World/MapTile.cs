using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoreEngine.World {
	class MapTile {
		public MapTile() {

		}

		public void draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(new Texture2D(spriteBatch.GraphicsDevice, 1, 1), new Vector2(0, 0), Color.Orange);
		}
	}
}
