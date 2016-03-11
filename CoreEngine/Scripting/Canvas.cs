using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby.Runtime;
using CoreEngine.World;

namespace CoreEngine.Scripting {
	public class Canvas {
		static SpriteBatch spriteBatch = GameServices.GetService<SpriteBatch>();
		static GraphicsDevice graphicsDevice = GameServices.GetService<GraphicsDevice>();

		static Texture2D Pixel;
		static Rectangle CameraBounds;

		static Canvas() {
			Pixel = new Texture2D(graphicsDevice, 1, 1);
			Pixel.SetData(new Color[] { Color.White });
		}

		public static void LoadCameraBoundsForFrame() {
			CameraBounds = Camera.GetBounds();
		}

		private static bool IsInViewport(Vector2 point) {
			return CameraBounds.Contains(point);
		}

		public static void DrawTexture(Texture2D sprite, float x, float y) {
			spriteBatch.Draw(sprite, new Vector2(x, y), Color.White);
		}
		public static void DrawTexture(Texture2D sprite, float x, float y, CoreColor color) {
			spriteBatch.Draw(sprite, new Vector2(x, y), color.ToXNAColor());
		}

		public static void DrawString(SpriteFont font, string text, float x, float y, CoreColor color) {
			Vector2 dimensions = font.MeasureString(text);
			if(CameraBounds.Intersects(new Rectangle((int)x, (int)y, (int)dimensions.X, (int)dimensions.Y))) {
				spriteBatch.DrawString(font, text, new Vector2(x, y), color.ToXNAColor());
			}
		}

		public static void DrawRect(int x, int y, int width, int height, CoreColor coreColor) {
			Color color = coreColor.ToXNAColor();
			spriteBatch.Draw(Pixel, new Rectangle(x, y, width, 1), color);
			spriteBatch.Draw(Pixel, new Rectangle(x, y, 1, height), color);
			spriteBatch.Draw(Pixel, new Rectangle(x + width, y, 1, height), color);
			spriteBatch.Draw(Pixel, new Rectangle(x, y + height, width, 1), color);
		}

		public static void DrawFilledRect(int x, int y, int width, int height, CoreColor coreColor) {
			spriteBatch.Draw(Pixel, new Rectangle(x, y, width, height), coreColor.ToXNAColor());
		}

		public static Vector2 GetStringWidth(SpriteFont font, string text) {
			return font.MeasureString(text);
		}

		public static Vector2 CreateVector2(float x, float y) {
			return new Vector2(x, y);
		}
	}
}
