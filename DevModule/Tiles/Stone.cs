using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeRPG.Entities;

namespace DevModule.Tiles {
	public class Stone : MapTile {
		int Iterations = 0;
		public Stone(int x, int y) : base(x, y) {
			DrawOnLayer(2, "SecondDraw");
			SetEntityProperty("update", true);
		}

		public override void Update() {
			Iterations += 1;
		}

		public override void Draw() {
			base.Draw();
			Canvas.DrawString(FontRepository.System, Iterations.ToString(), (int) Position.X, (int) Position.Y, Color.White);
		}
		public void SecondDraw() {
			Canvas.DrawFilledRect((int) Position.X, (int) Position.Y, 4, 4, Color.Black);
		}
	}
}
