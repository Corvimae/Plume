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
using IronRuby.Builtins;

namespace CoreEngine {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Core : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Map activeMap;

		public Core() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
		}

		protected override void Initialize() {
			base.Initialize();

			GameServices.AddService<GraphicsDevice>(GraphicsDevice);

			ModuleController.RegisterModule("DevModule");
	
			activeMap = new Map(50, 50);
			Camera.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GameServices.AddService<SpriteBatch>(spriteBatch);
			CoreFont.System = Content.Load<SpriteFont>("Fonts/System");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() {
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
				Camera.SetYPosition(Camera.GetYPosition() - 5);
			} else if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
				Camera.SetYPosition(Camera.GetYPosition() + 5);
			}

			if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
				Camera.SetXPosition(Camera.GetXPosition() - 5);
			} else if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
				Camera.SetXPosition(Camera.GetXPosition() + 5);
			}

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
			foreach(Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("draw", new object[] { });
			}

			foreach(List<DrawOperation> operationList in DrawQueue.ProcessAndReturnDrawQueue().Values) {
				foreach(DrawOperation operation in operationList) {
					if(operation.Identifier.GetType().Name == "RubySymbol") {
						CoreScript script = ModuleController.FindEntityRecordByReferencer(operation.Entity.GetReferencer());
						script.InvokeMethod(operation.Entity, (string)operation.Identifier.String, new object[] { });
					} else {
						TypeReflector.InvokeMethod(operation.Entity, operation.Identifier, null);
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
