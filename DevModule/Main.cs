using PlumeAPI.Entities;
using PlumeAPI.Events;
using PlumeAPI.Graphics;
using PlumeAPI.Modularization;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using PlumeAPI.Networking;
using DevModule.Messages;
using System;

namespace DevModule {
	class Main : PlumeModule {
		Animation animation;
		int Frame = 0;
		public Main() {
			Debug.WriteLine("DevModule launched");
			EventController.RegisterEvent("pause");
			TextureController.RegisterTexture("excavator", "excavator.png", GetModule());
			TextureController.RegisterAnimation("excavator_walk", "excavator", 32, 64, 3, 6);
			CallOnEvent("pause", 0, "TogglePause");
			animation = TextureController.GetAnimationInstance("excavator_walk"); //Inefficient but good for testing
			MessageController.RegisterMessageType(new DebugMessage(null));
		}

		public override void AfterLoad() {
			Debug.WriteLine("DevModule loaded!");
		}

		public override void Update() {
			Frame += 1;
		}

		public override void UserConnected(Client user) {
			Console.WriteLine("DevModule: User connected, ayy!");
			user.Message(new DebugMessage("Hello!"));
		}

		public override void UserDisconnected(Client user) {
			Console.WriteLine("DevModule: " + user.Name + " disconnected!");
		}

		public void TogglePause(EventData eventData) {
			animation.Paused = !animation.Paused;
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
