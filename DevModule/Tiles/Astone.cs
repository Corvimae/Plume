using PlumeAPI.Events;
using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeRPG.Entities;

namespace DevModule.Tiles {
	public class AStone : MapTile {

		bool Clicked = false;
		public AStone() : base() {
		}

		public AStone(int x, int y) : base(x, y) {
			UnregisterDrawOnLayer(2);
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
