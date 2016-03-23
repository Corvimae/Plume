using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeRPG.Entities;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;

namespace DevModule.Tiles {
	public class Stone : MapTile {
		int Iterations = 0;
		public Stone() : base() { }

		public Stone(int x, int y) : base(x, y) {
			DrawOnLayer(2, "SecondDraw");
			SetEntityProperty("update", true);
		}

		public override void UpdateServer() {
			Position.X += 1;
			ServerMessageDispatch.Broadcast(new UpdateEntityMessageHandler(this));
		}

		public override void Draw() {
			base.Draw();
			Canvas.DrawString(FontRepository.System, "" + Id, (int) Position.X, (int) Position.Y, Color.White);
		}
		public void SecondDraw() {
			Canvas.DrawFilledRect((int) Position.X, (int) Position.Y, 4, 4, Color.Black);
		}
	}
}
