using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.World {
	public class Camera {
		public float X;
		public float Y;
		public float Width;
		public float Height;
		public float XGoal;
		public float YGoal;
		public float Speed = 1.0f;

		public Camera(float x, float y) {
			this.X = x;
			this.Y = y;
			this.XGoal = x;
			this.YGoal = y;
			GraphicsDevice graphicsDevice = GameServices.GetService<GraphicsDevice>();
			this.Width = graphicsDevice.Viewport.Width;
			this.Height = graphicsDevice.Viewport.Height;
		}

		public float GetXPosition() {
			return X;
		}

		public float GetYPosition() {
			return Y;
		}

		public float GetXGoal() {
			return XGoal;
		}

		public float GetYGoal() {
			return YGoal;
		}

		public float GetSpeed() {
			return Speed;
		}

		public void SetGoal(float x, float y) {
			XGoal = x;
			YGoal = y;
		}

		public void SetGoal(Vector2 position) {
			XGoal = position.X;
			YGoal = position.Y;
		}

		public void SetXGoal(float x) {
			XGoal = x;
		}

		public void SetYGoal(float y) {
			YGoal = Y;
		}

		public void SetPosition(float x, float y) {
			this.X = x;
			this.Y = y;
		}

		public void SetPosition(Vector2 position) {
			X = position.X;
			Y = position.Y;
		}

		public void SetXPosition(float x) {
			this.X = x;
		}

		public void SetYPosition(float y) {
			this.Y = y;
		}

		public void SetCameraSpeed(float speed) {
			this.Speed = speed;
		}

		public Vector2 GetStartPosition() {
			return new Vector2(X, Y);
		}
		
		public Vector2 GetEndPosition() {
			return new Vector2(X + Width, Y + Height);
		}

		public void Update() {
			//Trend towards our position
			X += (XGoal - X) / (1 / Speed);
			X += (YGoal - Y) / (1 / Speed);
		}
	}
}
