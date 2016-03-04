using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CoreEngine.World {
	class Map {
		public static int TileSize = 32;
		protected int Width;
		protected int Height;
		private MapTile[,] grid;

		public Map(int width, int height) {
			this.Width = width;
			this.Height = height;
			grid = new MapTile[Height, Width];
			for (int x = 0; x < grid.GetLength(0); x++) {
				for (int y = 0; y < grid.GetLength(1); y++) {
					grid[x, y] = new MapTile("Dirt", x, y);
				}
			}
		}

		public void draw(SpriteBatch spriteBatch) {
			for (int x = 0; x < grid.GetLength(0); x++) {
				for (int y = 0; y < grid.GetLength(1); y++) {
					MapTile tile = grid[x, y];
					tile.draw(spriteBatch);
				}
			}
		}
	}
}
