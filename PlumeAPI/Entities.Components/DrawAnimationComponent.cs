using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;

namespace PlumeAPI.Entities.Components {
	public class DrawAnimationComponent : DrawComponent {
		string AnimationName { get; set; }
		public Animation Animation { get; set; }
		public DrawAnimationComponent(string animation, int priority, BaseEntity entity) : base(priority, entity) {
			AnimationName = animation;
			Animation = TextureController.GetAnimationInstance(animation);
		}

		public override void Call(EventData eventData) {
			Vector2 position = Entity.GetComponent<PositionalComponent>().Position;
			Canvas.DrawAnimation(Animation, position.X, position.Y, Color.White);
		}

		public override EntityComponent Clone(BaseEntity newEntity) {
			return new DrawAnimationComponent(AnimationName, Priority, newEntity);
		}
	}
}
