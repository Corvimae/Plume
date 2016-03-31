using Microsoft.Xna.Framework;
using PlumeAPI.Attributes;
using PlumeAPI.Entities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeRPG.Entities {
	public class Actor : BaseEntity {

		[Syncable, Interpolate]
		public override Vector2 Position { get; set; }

		[Syncable]
		public ActorMotion MotionState { get; set; }

		public Actor() : base() { }

		public Actor(int x, int y) : base() {
			Position = new Vector2(x, y);
		}

		public bool IsMovingInDirection(ActorMotion direction) {
			return (MotionState & direction) == direction;
		}

		static Actor() {
			EntitySnapshot.RegisterTypeHandler(
				typeof(ActorMotion),
				(message) => {
					return (ActorMotion)message.ReadInt32();
				},
				(obj, message) => {
					message.Write((int) obj);
				}
			);
		}
	}

	[Flags]
	public enum ActorMotion {
		None = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8
	}
}
