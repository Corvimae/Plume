using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeRPG.Entities;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;
using PlumeAPI.Attributes;

namespace DevModule.Tiles {
	public class Stone : MapTile {
		public Stone() : base() { }

		[Syncable]
		public byte Bazoople { get; set; }
		public Stone(int x, int y) : base(x, y) {
			DrawOnLayer(2, "SecondDraw");
			SetEntityProperty("update", true);
			Bazoople = 0;
		}

		public override void UpdateServer() {
			SetPosition(Position.X + 1, Position.Y);
			Bazoople += 1;
		}

		public override void Draw() {
			base.Draw();
			Canvas.DrawString(FontRepository.System, "" + Bazoople, (int) Position.X, (int) Position.Y, Color.White);
		}
		public void SecondDraw() {
			Canvas.DrawFilledRect((int) Position.X, (int) Position.Y, 4, 4, Color.Black);
		}
	}
}
