using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreEngine.World;
using CoreEngine.Modularization;
using System.Diagnostics;
using CoreEngine.Scripting;

namespace CoreEngine.Entities {
	public class MapTile : BaseEntity {
		
		protected Texture2D texture;
		protected Vector2 Coordinates;
		public MapTile() { }
		public MapTile(int x, int y) : base() {
			Coordinates = new Vector2(x, y);
			Position = new Vector2(x * 32, y * 32);
			DrawDimensions = new Vector2(32, 32);
		}
		protected override void Register() {
			RegisterTexture("texture", Metadata.Name + ".png");
		}

		protected override void Create() {
			texture = GetTexture("texture");
		}

		public override void Draw() {
			Vector2 position = GetPosition();
			Canvas.DrawTexture(@texture, position.X, position.Y, CoreColor.White);
		}
	}
}
