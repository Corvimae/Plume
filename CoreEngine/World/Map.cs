using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using CoreEngine.Modularization;
using System.Diagnostics;
using CoreEngine.Entities;

namespace CoreEngine.World {
	public class Map {
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
					grid[x, y] = (MapTile) ModuleControl.CreateEntityByReferencer("MapTile.Stone", x, y);
				}
			}
		}

		public void draw(SpriteBatch spriteBatch) {
			//Find our starting point.
			Camera camera = GameServices.GetService<Camera>();
			Vector2 startPosition = camera.GetStartPosition();
			Vector2 endPosition = camera.GetEndPosition();
			int startX = (int) Math.Max(0, Math.Floor(startPosition.X / TileSize));
			int startY = (int )Math.Max(0, Math.Floor(startPosition.Y / TileSize));
			int endX = (int) Math.Min(grid.GetLength(0), Math.Ceiling(endPosition.X / TileSize));
			int endY = (int) Math.Min(grid.GetLength(1), Math.Ceiling(endPosition.Y / TileSize));
			for (int x = startX; x < endX; x++) {
				for (int y = startY; y < endY; y++) {
					MapTile tile = grid[x, y];
					tile.draw();
				}
			}
		}
	}
}
