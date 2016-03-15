using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreEngine.World;
using CoreEngine.Modularization;
using CoreEngine.Utilities;
using System.Diagnostics;
using System;
using System.Linq;
using CoreEngine.Scripting;
using CoreEngine.Entities;
using System.Collections.Generic;
using CoreEngine.Events;

namespace CoreEngine {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Core : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Map activeMap;
		DrawQueue drawQueue = new DrawQueue();

		private KeyboardState previousKeyboardState = Keyboard.GetState();
		private MouseState previousMouseState = Mouse.GetState();

		public Core() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
		}

		protected override void Initialize() {
			base.Initialize();

			this.IsMouseVisible = true;

			GameServices.AddService<GraphicsDevice>(GraphicsDevice);
			string[] modules = new string[] { "DevModule", "Core" };
			foreach(string module in modules) ModuleController.RegisterModule(module);
			ModuleController.ResolveDependencies();
			ModuleController.ImportModules();

			activeMap = new Map(50, 50);
			Camera.Initialize();
			Camera.UseEasing = true;				
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GameServices.AddService<SpriteBatch>(spriteBatch);
			CoreFont.System = Content.Load<SpriteFont>("Fonts/System");
		}

		protected override void UnloadContent() {
		}

		protected override void Update(GameTime gameTime) {
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			//Handle Input

			KeyboardState keyboardState = Keyboard.GetState();
			MouseState mouseState = Mouse.GetState();
			if(keyboardState.IsKeyDown(Keys.Up)) {
				Camera.YGoal -= 10;
			} else if(keyboardState.IsKeyDown(Keys.Down)) {
				Camera.YGoal += 10;
			}

			if(keyboardState.IsKeyDown(Keys.Left)) {
				Camera.XGoal -= 10;
			} else if(keyboardState.IsKeyDown(Keys.Right)) {
				Camera.XGoal += 10;
			}

			if(keyboardState.IsKeyDown(Keys.OemMinus)) {
				Camera.Scale -= 0.05f;
			} else if(keyboardState.IsKeyDown(Keys.OemPlus)) {
				Camera.Scale += 0.05f;
			}

			if(IsKeyPressed(keyboardState, Keys.E)) {
				EventController.Fire("pause", new EventBundle(new Dictionary<object, object>()));
			}

			if(mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed ) {
				EventBundle bundle = new EventBundle(new Dictionary<object, object> {
					{ "x", mouseState.X },
					{ "y", mouseState.Y }
				});
				EventController.Fire("click", bundle);
				if(bundle.ContinuePropagating) {
					foreach(BaseEntity e in EntityController.GetAllEntities().Where(e => e.HasPropertyEnabled("click"))) {
						if(e.GetDrawBoundry().Contains((int) bundle.Content["x"], (int) bundle.Content["y"])) {
							e.OnClick(bundle);
							if(!bundle.ContinuePropagating) break;
						}
					}
				}
			}
				
			previousMouseState = mouseState; 
			previousKeyboardState = keyboardState;

			//End Input Section

			Camera.Update();

			foreach(CoreEngine.Modularization.Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("update", new object[] { });
			}

			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("update")) {
					entity.Update();
				}
			}

			base.Update(gameTime);
		}

		private bool IsKeyPressed(KeyboardState keyboardState, Keys key) {
			return keyboardState.IsKeyDown(key) && !previousKeyboardState.GetPressedKeys().Contains(key);
		}


		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Canvas.LoadCameraBoundsForFrame();

			DateTime start = DateTime.Now;
			Matrix transformationMatrix = Camera.GetTransformationMatrix();
			Matrix inverseMatrix = Matrix.Invert(transformationMatrix);
			spriteBatch.Begin(transformMatrix: transformationMatrix);
			foreach(CoreEngine.Modularization.Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("draw", new object[] { });
			}
				
			foreach(DrawLayer layer in drawQueue.ProcessAndReturnDrawQueue().Values) {
				foreach(BaseEntity entity in layer.BaseDrawEntities) {
					entity.Draw();
				}
				foreach(DrawQueueOperation operation in layer.DrawOperations) {
					if(operation.Delegate.IsCSharp) {
						operation.Delegate.Delegate.Invoke();
					} else {
						operation.Delegate.Delegate.Invoke(operation.Entity, null);
					}
				}
			}
			//UI Layer
			float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
			spriteBatch.DrawString(CoreFont.System,
				"FPS: " + Math.Round(frameRate) + " | Draw: " + Math.Round((DateTime.Now - start).TotalMilliseconds) + "ms",
				Vector2.Transform(new Vector2(5, 5), inverseMatrix), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
