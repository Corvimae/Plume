using PlumeAPI.Entities;
using PlumeAPI.Events;
using PlumeAPI.Graphics;
using PlumeAPI.Modularization;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;

namespace DevModule {
	class Main : PlumeModule {
		public Main() {
			Debug.WriteLine("DevModule launched");
		}

		public override void Register() {
			base.Register();

			TextureController.RegisterTexture("Stone_texture", "Stone.png", Module);
			TextureController.RegisterTexture("AStone_texture", "AStone.png", Module);
			TextureController.RegisterTexture("player", "excavator.png", Module);
			TextureController.RegisterAnimation("player_walk", "player", 32, 63, 3, 6);

			EventController.RegisterEvent("pause");
			Module.LoadEntitiesFromDirectory("Assets/Entities");
		}

		public override void AfterLoad() {
			Debug.WriteLine("DevModule loaded!");
		}

		public override void Update() {
		}

		public override void Draw() {
			
			/*Canvas.DrawAnimation(animation, 0, 0, Color.White);
			if(Frame % 100 == 0) {
				animation.FlipHorizontal = !animation.FlipHorizontal;
			}
			if(Frame % 1000 == 0) {
				animation.FlipVertical = !animation.FlipVertical;
			}*/
		}
	}
}
