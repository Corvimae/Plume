using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using CoreEngine.Entities;
using CoreEngine.Modularization;

namespace CoreEngine {
	public class CoreObjectData {
		public Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();
		public Dictionary<string, Animation> AnimationRegistry = new Dictionary<string, Animation>();

		public Module Module;
		public virtual CoreObjectData CreateImpartialClone() {
			CoreObjectData data = new CoreObjectData();
			data.Module = Module;
			data.TextureRegistry = ShallowCopyDictionary<string, Texture2D>(TextureRegistry);
			data.AnimationRegistry = ShallowCopyDictionary<string, Animation>(AnimationRegistry);
			return data;
		}

		protected Dictionary<K, V> ShallowCopyDictionary<K, V>(Dictionary<K, V> dict) {
			return dict.ToDictionary(x => x.Key, x => x.Value);
		}
	}
}

