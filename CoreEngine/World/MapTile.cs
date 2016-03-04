using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreEngine.World;
using CoreEngine.Modularization;
using System.Diagnostics;

namespace CoreEngine.World {
	class MapTile : ReferenceType {
		ReferenceData Definition;
		Vector2 Position;
		public MapTile(string referencer, int x, int y) {
			Definition = MapTile.FindReferenceData(referencer);
			Position = new Vector2(x, y);
		}

		public void draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(Definition.Texture, new Vector2(Position.X * Map.TileSize, Position.Y * Map.TileSize), Color.Orange);
			Definition.AttemptLuaFunctionCall("onDraw");
		}
	}
}
