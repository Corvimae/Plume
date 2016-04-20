using PlumeAPI.Modularization;
using System.Diagnostics;
using PlumeAPI.Networking;
using PlumeRPG.Entities;
using PlumeAPI.Entities;
using PlumeAPI.World;
using PlumeAPI.Commands.Builtin;
using PlumeRPG.Networking.Handlers;
using System;
using PlumeRPG.World;
using PlumeAPI.Graphics;
using PlumeAPI.Entities.Components;
using Microsoft.Xna.Framework;

namespace PlumeRPG {
	class Main : PlumeModule {

		public static BaseEntity ActivePlayer { get; set; }
		public Main() {
			Debug.WriteLine("PlumeCore launched");
		}

		public override void Register() {
			base.Register();
			MovementCommands.Register();
			MessageController.RegisterMessageType(new SendUserDataMessageHandler(null));
			Module.LoadEntitiesFromFile("Assets/Entities/player.json");
			EntitySnapshot.RegisterTypeHandler(
				typeof(ActorMotion),
				(message) => {
					return (ActorMotion)message.ReadInt32();
				},
				(obj, message) => {
					message.Write((int)obj);
				}
			);
		}

		public override void RegisterClient() {
			TextureController.RegisterTexture("Stone_texture", "Stone.png", Module);
			TextureController.RegisterTexture("AStone_texture", "AStone.png", Module);
			TextureController.RegisterTexture("player", "excavator.png", Module);
			TextureController.RegisterAnimation("player_walk", "player", 32, 63, 3, 6);
			Console.WriteLine("Registration completed");


		}

		public override void RegisterServer() {
		}

		public override void AfterLoadServer() {
			Map testMap = new Map("MyMap", 50, 50);
		}

		public override void UpdateServer() {

		}
		public override void UserFullyLoaded(Client user) {
			BaseEntity playerEntity = EntityController.CreateNewEntity("Player", user.Scope);
			playerEntity.GetComponent<PositionalComponent>().Position = new Vector2(400, 400);
			user["PlayerEntity"] = playerEntity;
			playerEntity.GetComponent<NetworkedComponent>().ForceUpdate();
			user.Message(new SendUserDataMessageHandler(user));
		}

	}
}