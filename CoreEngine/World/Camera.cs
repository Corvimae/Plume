using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreEngine.World {
	public static class Camera {
		public static float X { get; set; }
		public static float Y { get; set; }
		public static float Scale { get; set; }
		public static float XGoal { get; set; }
		public static float YGoal { get; set; }
		public static float ScaleGoal { get; set; }
		public static float Speed { get; set; }

		private static float MaxSpeed = 7.5f;

		public static bool UseEasing = true;

	public static void Initialize() {
			X = 0;
			Y = 0;
			Scale = 1.0f;
			XGoal = X;
			YGoal = Y;
			ScaleGoal = 1.0f;
			Speed = 0.1f;
		}	

		public static void SetGoal(float x, float y) {
			XGoal = x;
			YGoal = y;
		}

		public static void SetGoal(float x, float y, float scale) {
			XGoal = x;
			YGoal = y;
			ScaleGoal = scale;
		}

		public static void SetGoal(Vector2 position) {
			XGoal = position.X;
			YGoal = position.Y;
		}

		public static void ForcePosition(float x, float y) {
			X = x;
			Y = y;
			XGoal = X;
			YGoal = Y;
		}

		public static void ForcePosition(Vector2 position) {
			X = position.X;
			Y = position.Y;
			XGoal = X;
			YGoal = Y;
		}

		public static Rectangle GetBounds() {
			GraphicsDevice graphicsDevice = GameServices.GetService<GraphicsDevice>();
			return new Rectangle((int)X, (int)Y, (int)(graphicsDevice.Viewport.Width * (1 / Scale)), (int)(graphicsDevice.Viewport.Height * (1 / Scale)));
		}

		public static bool IsOnCamera(Rectangle boundingBox) {
			return GetBounds().Intersects(boundingBox);
		}

		public static Matrix GetTransformationMatrix() {
			return Matrix.CreateTranslation(-X, -Y, 0) * Matrix.CreateScale(Scale, Scale, 1.0f);
		}

		public static void Update() {
			if(UseEasing) {
				//Trend towards our position
				float dX = (XGoal - X) / (1 / Speed);
				float dY = (YGoal - Y) / (1 / Speed);
				if(Math.Abs(dX) > MaxSpeed) {
					dX = dX < 0 ? -1 * MaxSpeed : MaxSpeed;
				}

				if(Math.Abs(dY) > MaxSpeed) {
					dY = dY < 0 ? -1 * MaxSpeed : MaxSpeed;
				}

				X += dX;
				if(Math.Abs(XGoal - X) < 1) X = XGoal;

				Y += dY;
				if(Math.Abs(YGoal - Y) < 1) Y = YGoal;

				Scale += (ScaleGoal - Scale) / (1 / Speed);
				if(Math.Abs(ScaleGoal - Scale) < 1) Scale = ScaleGoal;
			} else {
				X = XGoal;
				Y = YGoal;
				Scale = ScaleGoal;
			}

		}
	}
}
