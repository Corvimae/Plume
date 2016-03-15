using PlumeAPI.Entities;
using PlumeAPI.Modularization;

namespace PlumeEngine.World {
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
						grid[x, y] = ModuleController.CreateEntityByReferencer("DevModule.Tiles.Stone", x, y);
					} else {
						grid[x, y] = ModuleController.CreateEntityByReferencer("DevModule.Tiles.AStone", x, y); //Astone
					}
				}
			}
		}
	}
}
