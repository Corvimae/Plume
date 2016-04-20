using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeAPI.Modularization;

namespace PlumeAPI.Entities.Components {
	public class DrawTextureComponent : DrawComponent {

		public string TextureName { get; set; }
		public Texture2D Texture { get; set; }
		public DrawTextureComponent(string texture, int priority, BaseEntity entity) : base(priority, entity) {
			TextureName = texture;
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				Texture = TextureController.GetTexture(texture);
			}
		}

		public override void CallClient(EventData eventData) {
			Vector2 position = Entity.GetComponent<PositionalComponent>().Position;
			Canvas.DrawTexture(Texture, position.X, position.Y);
		}

		public override EntityComponent Clone(BaseEntity newEntity) {
			return new DrawTextureComponent(TextureName, Priority, newEntity);
		}
	}
}
