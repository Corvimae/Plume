using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities.Components {
	public class DrawComponent : EventTriggeredComponent {
		public DrawComponent(int priority, BaseEntity entity) : base("draw", priority, entity) {}

		public override EntityComponent Clone(BaseEntity newEntity) {
			return new DrawComponent(Priority, newEntity);
		}
	}
}
