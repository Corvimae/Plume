using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreEngine.World;
using CoreEngine.Modularization;
using CoreEngine.Utilities;
using System.Diagnostics;
using System;
using CoreEngine.Scripting;
using CoreEngine.Entities;
using System.Collections.Generic;

namespace CoreEngine {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Core : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Map activeMap;

		public object AstUtils { get; private set; }

		public Core() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
		}

		protected override void Initialize() {
			base.Initialize();

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

			if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
				Camera.YGoal -= 10;
			} else if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
				Camera.YGoal += 10;
			}

			if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
				Camera.XGoal -= 10;
			} else if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
				Camera.XGoal += 10;
			}

			if(Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
				Camera.Scale -= 0.05f;
			} else if(Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
				Camera.Scale += 0.05f;
			}

			Camera.Update();

			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("update")) {
					entity.Update();
				}
			}

			base.Update(gameTime);
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

			foreach(List<DrawOperation> operationList in DrawQueue.ProcessAndReturnDrawQueue().Values) {
				foreach(DrawOperation operation in operationList) {
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
