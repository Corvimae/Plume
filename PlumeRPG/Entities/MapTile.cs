using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Diagnostics;
using PlumeAPI.Entities;
using PlumeAPI.Graphics;
using Lidgren.Network;
using System;
using PlumeAPI.Modularization;

namespace PlumeRPG.Entities {
	public class MapTile : BaseEntity {
		public Vector2 Coordinates;
		public Texture2D Texture;

		public override void RegisterClient() {
			if(GetType().Name == "MapTile") return;
			TextureController.RegisterTexture(GetLocalName() + "_texture", GetLocalName() + ".png", this.GetModule());
			Debug.WriteLine("Calling registration function for " + MethodBase.GetCurrentMethod().DeclaringType.Name + "(as " + GetName() + ")");
			SetEntityProperty("draw", true);
			SetEntityProperty("click", true);
		}

		public MapTile() : base() { }

		public MapTile(int x, int y) {
			Coordinates = new Vector2(x, y);
			SetPosition(x * 32, y * 32);
			SetDrawDimensions(32, 32);

			Texture = TextureController.GetTexture(GetLocalName() + "_texture");
		}

		public override void Draw() {
			Canvas.DrawTexture(Texture, Position.X, Position.Y, Color.White);
		}

		public override void PackageForInitialTransfer(NetOutgoingMessage message) {
			base.PackageForInitialTransfer(message);
			message.Write((int) Coordinates.X);
			message.Write((int) Coordinates.Y);
		}

		public override void PackageForUpdate(NetOutgoingMessage message) {
			base.PackageForUpdate(message);
			message.Write((int)Position.X);
			message.Write((int)Position.Y);
		}

		public override void UpdateFromMessage(object[] arguments) {
			SetPosition((int)arguments[0], (int)arguments[1]);
		}

		public new static object[] UnpackageFromUpdate(NetIncomingMessage message) {
			int x = message.ReadInt32();
			int y = message.ReadInt32();
			return new object[] { x, y };
		}

		public new static object[] UnpackageFromInitialTransfer(NetIncomingMessage message) {
			int id = message.ReadInt32();
			int x = message.ReadInt32();
			int y = message.ReadInt32();
			return new object[] { id, x, y };
		}

	}
}