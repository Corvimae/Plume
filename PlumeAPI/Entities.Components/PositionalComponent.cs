using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Networking;
using PlumeAPI.Events;

namespace PlumeAPI.Entities.Components {
	public class PositionalComponent : EntityComponent {
		Vector2 _position;
		[Syncable, Interpolate]
		public Vector2 Position {
			get {
				return _position;
			}
			set {
				_position = value;
				this.MarkPropertyDirty("Position");
			}
		}

		public PositionalComponent(BaseEntity entity) : base(entity) {
			Position = new Vector2(0, 0);
		}

		public override void PackageForInitialTransfer(OutgoingMessage message) {
			message.Write((int)Position.X);
			message.Write((int)Position.Y);
		}
		public override void UnpackageFromInitialTransfer(IncomingMessage message) {
			Position = new Vector2(message.ReadInt32(), message.ReadInt32());
		}

		public override EntityComponent Clone(BaseEntity newEntity) {
			PositionalComponent newComponent = new PositionalComponent(newEntity);
			newComponent.Position = Position;
			return newComponent;
		}
	}
}
