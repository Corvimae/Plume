using PlumeAPI.Modularization;
using System.Diagnostics;
using PlumeRPG.Entities;
using PlumeAPI.Entities;
using PlumeAPI.World;
using PlumeAPI.Commands.Builtin;
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
			Module.LoadEntitiesFromDirectory("Assets/Entities");

			MovementCommands.Register();

			Console.WriteLine("Registration completed");
		}


		public override void AfterLoad() {
			Map testMap = new Map("MyMap", 50, 50);
			BaseEntity playerEntity = EntityController.CreateNewEntity("Player", testMap);
			playerEntity.GetComponent<PositionalComponent>().Position = new Vector2(400, 400);
		}
	}
}