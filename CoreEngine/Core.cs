using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreEngine.World;
using CoreEngine.Modularization;
using CoreEngine.Utilities;
using System.Diagnostics;
using System;
using System.Linq;
using CoreEngine.Graphics;
using CoreEngine.Entities;
using System.Collections.Generic;
using CoreEngine.Events;
using System.Reflection;

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
			AppDomain.CurrentDomain.AssemblyResolve += ResolveDuplicateAssembly;

			this.IsMouseVisible = true;

			GameServices.AddService<GraphicsDevice>(GraphicsDevice);
			string[] modules = new string[] { "DevModule", "PlumeCore" };
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
			FontRepository.System = Content.Load<SpriteFont>("Fonts/System");
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
				EventController.Fire("pause", new EventData(new Dictionary<string, object>()));
			}

			if(mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed ) {
				Vector2 localMousePoint = Camera.ConvertViewportToCamera(new Vector2(mouseState.X, mouseState.Y));
				EventData eventData = new EventData(new Dictionary<string, object> {
					{ "position", localMousePoint },
				});
				EventController.Fire("click", eventData);
				if(eventData.ContinuePropagating) {
					foreach(BaseEntity e in EntityController.GetAllEntities().Where(e => e.HasPropertyEnabled("click"))) {
						if(e.GetDrawBoundry().Contains((Vector2) eventData["position"])) {
							e.OnClick(eventData);
							if(!eventData.ContinuePropagating) break;
						}
					}
				}
			}
				
			previousMouseState = mouseState; 
			previousKeyboardState = keyboardState;

			//End Input Section

			Camera.Update();

			foreach(Modularization.Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("Update", new object[] { });
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
			foreach(Modularization.Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("Draw", new object[] { });
			}
				
			foreach(List<Action> layer in drawQueue.ProcessAndReturnDrawQueue().Values) {
				foreach(Action operation in layer) {
					operation.Invoke();
				}
			}

			//UI Layer
			float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
			spriteBatch.DrawString(FontRepository.System,
				"FPS: " + Math.Round(frameRate) + " | Draw: " + Math.Round((DateTime.Now - start).TotalMilliseconds) + "ms",
				Camera.ConvertViewportToCamera(new Vector2(5, 5)), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		static Assembly ResolveDuplicateAssembly(object source, ResolveEventArgs e) {
			if(!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().FullName == e.Name)) {
				return Assembly.Load(e.Name);
			} else {
				Debug.WriteLine("Duplicate assembly " + e.Name + " skipped.");
				return AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().FullName == e.Name);
			}
		}
	}
}
