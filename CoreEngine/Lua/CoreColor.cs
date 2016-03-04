using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Lua {
	[MoonSharpUserData]
	class CoreColor {

		public static CoreColor White = new CoreColor(255, 255, 255, 255);
		public static CoreColor Black = new CoreColor(0, 0, 0, 255);
		int r, g, b, a;

		public CoreColor(int r, int g, int b, int a) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Color ToXNAColor() {
			return new Color(r, g, b, a);
		}
	}
}
