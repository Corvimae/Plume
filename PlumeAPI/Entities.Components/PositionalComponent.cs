using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Events;

namespace PlumeAPI.Entities.Components {
	public class PositionalComponent : EntityComponent {
		public Vector2 Position { get; set; }

		public PositionalComponent(BaseEntity entity) : base(entity) {
			Position = new Vector2(0, 0);
		}

		public override EntityComponent Clone(BaseEntity newEntity) {
			PositionalComponent newComponent = new PositionalComponent(newEntity);
			newComponent.Position = Position;
			return newComponent;
		}
	}
}
