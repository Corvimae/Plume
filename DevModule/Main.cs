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
		int Frame = 0;
		public Main() {
			Debug.WriteLine("DevModule launched");
		}

		public override void Register() {
			base.Register();
			EventController.RegisterEvent("pause");
			MessageController.RegisterMessageType(new DebugMessage(null));
			Module.LoadEntitiesFromFile("Assets/Entities/tiles.json");
		}

		public override void RegisterClient() {
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
