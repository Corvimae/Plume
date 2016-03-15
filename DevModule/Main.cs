using CoreEngine.Attributes;
using CoreEngine.Entities;
using CoreEngine.Events;
using CoreEngine.Graphics;
using CoreEngine.Modularization;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DevModule {
	[PlumeStartup]
	class Main : PlumeModule {
		Animation animation;
		int Frame = 0;
		public Main() {
			Debug.WriteLine("DevModule launched");
			EventController.RegisterEvent("pause");
			RegisterTexture("excavator", "excavator.png");
			RegisterAnimation("excavator_walk", "excavator", 32, 64, 3, 6);
			CallOnEvent("pause", 0, "TogglePause");
			animation = GetAnimationInstance("excavator_walk"); //Inefficient but good for testing
		}

		public override void AfterLoad() {
			Debug.WriteLine("DevModule loaded!");
		}

		public override void Update() {
			Frame += 1;
		}

		public void TogglePause(EventData eventData) {
			animation.Paused = !animation.Paused;
		}

		public override void Draw() {
			Canvas.DrawAnimation(animation, 0, 0, Color.White);
			if(Frame % 100 == 0) {
				animation.FlipHorizontal = !animation.FlipHorizontal;
			}
			if(Frame % 1000 == 0) {
				animation.FlipVertical = !animation.FlipVertical;
			}
		}
	}
}
