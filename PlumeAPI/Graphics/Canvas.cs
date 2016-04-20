using PlumeAPI.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlumeAPI.World;
using PlumeAPI.Entities;

namespace PlumeAPI.Graphics {
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

		public static bool IsInViewport(Rectangle boundry) {
			return CameraBounds.Intersects(boundry);
		}

		public static void DrawTexture(Texture2D sprite, float x, float y) {
			if(IsInViewport(new Rectangle((int) x, (int) y, sprite.Width, sprite.Height))) {
				spriteBatch.Draw(sprite, new Vector2(x, y), Color.White);
			}
		}
		public static void DrawTexture(Texture2D sprite, float x, float y, Color color) {
			if(IsInViewport(new Rectangle((int) x, (int) y, sprite.Width, sprite.Height))) {
				spriteBatch.Draw(sprite, new Vector2(x, y), color);
			}
		}
		public static void DrawTexture(Texture2D sprite, Vector2 position, Rectangle clip, Color color, Vector2 origin, SpriteEffects effect) {
			spriteBatch.Draw(sprite, position, clip, color, 0.0f, origin, 1.0f, effect, 0.0f);
		}

		public static void DrawAnimation(Animation animation, float x, float y, Color color) {
			animation.Draw(new Vector2(x, y), color);
		}

		public static void DrawString(SpriteFont font, string text, float x, float y, Color color) {
			Vector2 dimensions = font.MeasureString(text);
			if(CameraBounds.Intersects(new Rectangle((int)x, (int)y, (int)dimensions.X, (int)dimensions.Y))) {
				spriteBatch.DrawString(font, text, new Vector2(x, y), color);
			}
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

		public static Vector2 GetStringWidth(SpriteFont font, string text) {
			return font.MeasureString(text);
		}

		public static Vector2 CreateVector2(float x, float y) {
			return new Vector2(x, y);
		}
	}
}
