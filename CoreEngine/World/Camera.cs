using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.World {
	public static class Camera {
		public static float X;
		public static float Y;
		public static float Scale;
		public static float XGoal;
		public static float YGoal;
		public static float Speed = 1.0f;

		public static void Initialize() {
			X = 0;
			Y = 0;
			XGoal = X;
			YGoal = Y;
			Scale = 1.0f;
		}

		public static float GetXPosition() {
			return X;
		}

		public static float GetYPosition() {
			return Y;
		}

		public static float GetXGoal() {
			return XGoal;
		}

		public static float GetYGoal() {
			return YGoal;
		}

		public static float GetSpeed() {
			return Speed;
		}

		public static float GetScale() {
			return Scale;
		}

		public static void SetScale(float scale) {
			Scale = scale;
		}

		public static void SetGoal(float x, float y) {
			XGoal = x;
			YGoal = y;
		}

		public static void SetGoal(Vector2 position) {
			XGoal = position.X;
			YGoal = position.Y;
		}

		public static void SetXGoal(float x) {
			XGoal = x;
		}

		public static void SetYGoal(float y) {
			YGoal = Y;
		}

		public static void SetPosition(float x, float y) {
			X = x;
			Y = y;
		}

		public static void SetPosition(Vector2 position) {
			X = position.X;
			Y = position.Y;
		}

		public static void SetXPosition(float x) {
			X = x;
		}

		public static void SetYPosition(float y) {
			Y = y;
		}

		public static void SetCameraSpeed(float speed) {
			Speed = speed;
		}
		
		public static Rectangle GetBounds() {
			GraphicsDevice graphicsDevice = GameServices.GetService<GraphicsDevice>();
			return new Rectangle((int) X, (int) Y, (int) (graphicsDevice.Viewport.Width * (1 / Scale)), (int)(graphicsDevice.Viewport.Height * (1 / Scale)));
		}

		public static bool IsOnCamera(Rectangle boundingBox) {
			return GetBounds().Intersects(boundingBox);
		}

		public static Matrix GetTransformationMatrix() {
			return Matrix.CreateTranslation(-X, -Y, 0) * Matrix.CreateScale(Scale, Scale, 1.0f);
		}

		public static void Update() {
			//Trend towards our position
			X += (XGoal - X) / (1 / Speed);
			X += (YGoal - Y) / (1 / Speed);
		}
	}
}
