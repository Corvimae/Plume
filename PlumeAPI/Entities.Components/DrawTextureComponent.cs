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

		Dictionary<string, object> definition;
		public List<EntityTexture> Textures = new List<EntityTexture>();
		public DrawTextureComponent(Dictionary<string, object> textures, int priority, BaseEntity entity) : base(priority, entity) {
			definition = textures;
			foreach(KeyValuePair<string, object> item in textures) {
				Textures.Add(new EntityTexture(item.Key, (ComponentDefinition)item.Value));
			}
		}

		public override void Call(EventData eventData) {
			Vector2 position = Entity.GetComponent<PositionalComponent>().Position;
			foreach(EntityTexture texture in Textures) {
				Canvas.DrawTexture(texture.Texture, position.X + texture.Position.X, position.Y + texture.Position.Y);
			}
		}

		public override EntityComponent Clone(BaseEntity newEntity) {
			return new DrawTextureComponent(definition, Priority, newEntity);
		}
	}

	public class EntityTexture {
		public Vector2 Position { get; set; }
		public Texture2D Texture { get; set; }
		public EntityTexture(string texture, Dictionary<string, object> options) {
			Texture = TextureController.GetTexture(texture);
			if(options.ContainsKey("position")) {
				Dictionary<string, object> positionObject = (ComponentDefinition) options["position"];
				Position = new Vector2(Convert.ToSingle(positionObject["x"]), Convert.ToSingle(positionObject["y"]));
			} else {
				Position = new Vector2(0, 0);
			}
		}
	}
}
