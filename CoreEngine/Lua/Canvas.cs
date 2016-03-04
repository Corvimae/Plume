using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Lua {
	[MoonSharpUserData]
	class Canvas {
		static SpriteBatch spriteBatch = GameServices.GetService<SpriteBatch>();
		static GraphicsDevice graphicsDevice = GameServices.GetService<GraphicsDevice>();

		static Texture2D Pixel;

		public static void Initialize() {
			Pixel = new Texture2D(graphicsDevice, 1, 1);
			Pixel.SetData(new Color[] { Color.White });
		}


		public static void DrawSprite(Texture2D sprite, int x, int y) {
			spriteBatch.Draw(sprite, new Vector2(x, y), Color.White);
		}

		public static void DrawRect(int x, int y, int width, int height, Color color) {
			spriteBatch.Draw(Pixel, new Rectangle(x, y, width, 1), color);
			spriteBatch.Draw(Pixel, new Rectangle(x, y, 1, height), color);
			spriteBatch.Draw(Pixel, new Rectangle(x + width, y, 1, height), color);
			spriteBatch.Draw(Pixel, new Rectangle(x, y + height, width, 1), color);

		}

		public static void DrawFilledRect(int x, int y, int width, int height, Color color) {
			spriteBatch.Draw(Pixel, new Rectangle(x, y, width, height), color);
		}


	}
}
