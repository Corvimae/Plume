using CoreEngine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreEngine.Entities{
	public class EntityData : CoreObjectData  {

		public string Name;
		public string EntityType;

		public Dictionary<int, DynamicDelegate> DrawActionRegistry = new Dictionary<int, DynamicDelegate>();

		public Dictionary<string, bool> EntityProperties = new Dictionary<string, bool> {
			{ "draw", false },
			{ "update", false }
		};


		public int DrawLayer = 0;

		public string GetReferencer() {
			return Module.Definition.ModuleInfo.FullName + "." + EntityType + "." + Name;
		}

		public EntityData CreateImpartialClone() {
			EntityData data = new EntityData();
			data.EntityProperties = ShallowCopyDictionary<string, bool>(EntityProperties);
			data.DrawLayer = DrawLayer;
			data.TextureRegistry = ShallowCopyDictionary<string, Texture2D>(TextureRegistry);
			data.AnimationRegistry = ShallowCopyDictionary<string, Animation>(AnimationRegistry);

			data.DrawActionRegistry = ShallowCopyDictionary<int, DynamicDelegate>(DrawActionRegistry);
			return data;

		}
	}
}
