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
		private BaseEntity[,] grid;

		public Map(int width, int height) {
			this.Width = width;
			this.Height = height;
			grid = new BaseEntity[Height, Width];
			for (int x = 0; x < grid.GetLength(0); x++) {
				for (int y = 0; y < grid.GetLength(1); y++) {
					if(x % 2 == 0) {
						grid[x, y] = ModuleController.CreateEntityByReferencer("MapTile.Stone", x, y);
					} else {
						grid[x, y] = ModuleController.CreateEntityByReferencer("MapTile.AStone", x, y);

					}
				}
			}
		}
	}
}
