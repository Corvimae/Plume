using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities {
	public static class EntityInterpolation {
		static Dictionary<Type, Func<object, object, double, object>> interpolationHandlers = new Dictionary<Type, Func<object, object, double, object>>();

		public static void RegisterInterpolationHandler(Type type, Func<object, object, double, object> handler) {
			interpolationHandlers.Add(type, handler);
		}

		public static bool HasInterpolator(Type key) {
			return interpolationHandlers.ContainsKey(key);
		}

		public static object Interpolate(Type type, object from, object to, double percentage) {
			return interpolationHandlers[type].Invoke(from, to, percentage);
		}

		public static double InterpolateValue(double from, double to, double percentage) {
			return from + ((to - from) * percentage);
		}

		static EntityInterpolation() {
			RegisterInterpolationHandler(
				typeof(System.Byte),
				(from, to, percentage) => { return (byte)InterpolateValue((byte) from, (byte) to, percentage);  }
			);

			RegisterInterpolationHandler(
				typeof(Microsoft.Xna.Framework.Vector2),
				(from, to, percentage) => {
					Vector2 fromVector = (Vector2) from;
					Vector2 toVector = (Vector2) to;
					return new Vector2((float) InterpolateValue(fromVector.X, toVector.X, percentage), (float) InterpolateValue(fromVector.Y, toVector.Y, percentage));
				});
		}
	}
}
