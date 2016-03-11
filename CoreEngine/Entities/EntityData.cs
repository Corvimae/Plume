using CoreEngine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreEngine.Entities {
	public class EntityData {

		public string Name;
		public string EntityType;
		public DirectoryInfo SourceDirectory;

		public Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();
		public Dictionary<int, DynamicDelegate> DrawActionRegistry = new Dictionary<int, DynamicDelegate>();

		public Dictionary<string, bool> EntityProperties = new Dictionary<string, bool> {
			{ "draw", false },
			{ "update", false }
		};


		public int DrawLayer = 0;

		public string GetReferencer() {
			return EntityType + "." + Name;
		}

		public EntityData CreateImpartialClone() {
			EntityData data = new EntityData();
			data.EntityProperties = ShallowCopyDictionary<string, bool>(EntityProperties);
			data.DrawLayer = DrawLayer;
			data.TextureRegistry = ShallowCopyDictionary<string, Texture2D>(TextureRegistry);
			data.DrawActionRegistry = ShallowCopyDictionary<int, DynamicDelegate>(DrawActionRegistry);
			return data;

		}

		private Dictionary<K, V> ShallowCopyDictionary<K, V>(Dictionary<K, V> dict) {
			return dict.ToDictionary(x => x.Key, x => x.Value);
		}
	}
}
