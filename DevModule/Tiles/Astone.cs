using CoreEngine.Events;
using CoreEngine.Graphics;
using Microsoft.Xna.Framework;

namespace DevModule.Tiles {
	public class AStone : Stone {

		bool Clicked = false;
		public AStone(int x, int y) : base(x, y) {
			UnregisterDrawOnLayer(2);
		}

		public new static void Register() {
		}

		public override void OnClick(EventData bundle) {
			base.OnClick(bundle);
			Clicked = true;
		}

		public override void Draw() {
			base.Draw();
			Canvas.DrawString(FontRepository.System, "A", Position.X + 14, Position.Y + 14, Clicked ? Color.Black : Color.White);
		}
	}
}
