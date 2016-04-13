using Microsoft.Xna.Framework;
using PlumeAPI.Entities;
using PlumeAPI.Graphics;
using PlumeAPI.Networking;
using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeRPG.Entities {
	public class Player : Actor {

		static int WalkSpeed = 5;

		public override Vector2 Position { get; set; }
		
		public override ActorMotion MotionState { get; set; }

		public Client Client { get; set; }


		public string Name { get; set; }

		Animation playerTexture;

		public Player() : base() { }

		public Player(string name, int x, int y) : base(x, y) {
			this.MotionState = ActorMotion.None;
			playerTexture = TextureController.GetAnimationInstance("player_walk");
			Main.ActivePlayer = this;
			RegisterProperties();
		}

		public Player(Client client) : base() {
			this.Position = new Vector2(250, 250);
			this.Client = client;
			this.Name = Client.Name;
			RegisterProperties();
		}

		private void RegisterProperties() {
			Console.WriteLine("Registering player properties");
			RegisterProperty(EntityPropertyType.Syncable, "Name", () => { return Name; }, (value) => { Name = (string)value; });
			RegisterProperty(EntityPropertyType.Syncable | EntityPropertyType.Interpolatable | EntityPropertyType.ClientControlledValue,
				"Position", () => { return Position; }, (value) => { Position = (Vector2)value; });
			RegisterProperty(EntityPropertyType.Syncable | EntityPropertyType.ClientControlledValue, "MotionState", () => { return MotionState; }, (value) => { MotionState = (ActorMotion)value; });
		}

		public override void RegisterClient() {
			TextureController.RegisterTexture("player", "excavator.png", Module);
			TextureController.RegisterAnimation("player_walk", "player", 32, 64, 3, 6);
			SetEntityProperty("draw", true);
		}

		protected override bool IsClientControlled() {
			object activeId = DataStore.Retrieve("PlayerEntityId");
			return activeId != null && (int) activeId == Id;
		}

		public override void UpdateClient() {
			if(MotionState == ActorMotion.None) {
				playerTexture.Reset();
			} else if(IsMovingInDirection(MotionState, ActorMotion.West)) {
				playerTexture.FlipHorizontal = true;
				playerTexture.Paused = false;
			} else if(IsMovingInDirection(MotionState, ActorMotion.East)) {
				playerTexture.FlipHorizontal = false;
				playerTexture.Paused = false;
			}
			UpdateSyncableProperties();
		}

		public override void UpdateServer() {
			UpdatePlayerPosition();
		}

		public void UpdatePlayerPosition() {
			if(IsMovingInDirection(MotionState, ActorMotion.West)) {
				SetPosition(Position.X - WalkSpeed, Position.Y);
			} else if(IsMovingInDirection(MotionState, ActorMotion.East)) {
				SetPosition(Position.X + WalkSpeed, Position.Y);
			}

			if(IsMovingInDirection(MotionState, ActorMotion.North)) {
				SetPosition(Position.X, Position.Y - WalkSpeed);
			} else if(IsMovingInDirection(MotionState, ActorMotion.South)) {
				SetPosition(Position.X, Position.Y + WalkSpeed);
			}
		}

		public override void UpdateClientControlledValues() {
			UpdatePlayerPosition();
		}

		public override void Draw() {
			playerTexture.Draw(Position, Color.White);
		}

		public override void PackageForInitialTransfer(OutgoingMessage message) {
			base.PackageForInitialTransfer(message);
			message.Write(Name);
			message.Write((int)Position.X);
			message.Write((int)Position.Y);
		}

		public new static object[] UnpackageFromInitialTransfer(IncomingMessage message) {
			int id = message.ReadInt32();
			string name = message.ReadString();
			int x = message.ReadInt32();
			int y = message.ReadInt32();
			return new object[] { id, name, x, y };
		}

		static Player() {
			EntityController.AddEntityType(typeof(Player).FullName);
		}
	}

}
