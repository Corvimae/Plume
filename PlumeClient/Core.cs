using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlumeAPI.World;
using PlumeAPI.Modularization;
using PlumeAPI.Utilities;
using System.Diagnostics;
using System;
using System.Linq;
using PlumeAPI.Graphics;
using PlumeAPI.Entities;
using System.Collections.Generic;
using PlumeAPI.Events;
using System.Reflection;
using PlumeClient.Utilities;
using PlumeAPI.Networking;
using PlumeAPI.Commands;

namespace PlumeClient {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Core : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		static EntityScope ActiveScope;
		static DrawQueue drawQueue;

		Process serverWindow; //DEBUG

		private KeyboardState previousKeyboardState = Keyboard.GetState();
		private MouseState previousMouseState = Mouse.GetState();

		private double UpdateTime = 0;

		public Core() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 840;

			this.Exiting += HandleGameExiting;

		}

		protected override void Initialize() {
			base.Initialize();

			/* DEBUG */
#if WINDOWS
			serverWindow = Process.Start(ModuleController.ModuleDirectory + "/../PlumeServer/bin/Debug/PlumeServer.exe");
#endif
			/* END DEBUG */

			AppDomain.CurrentDomain.AssemblyResolve += ResolveDuplicateAssembly;

			this.IsMouseVisible = true;
			this.IsFixedTimeStep = true;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / ClientConfiguration.TickRate);

			GameServices.AddService<GraphicsDevice>(GraphicsDevice);
			GameServices.AddService<Core>(this);

			string[] modules = new string[] { "DevModule", "PlumeRPG" };
			foreach(string module in modules) ModuleController.RegisterModule(module);
			ModuleController.ResolveDependencies();
			ModuleController.ImportModules();

			drawQueue = new DrawQueue(ActiveScope);
			Camera.Initialize();
			Camera.UseEasing = true;

			GameServices.StartTimer();

			ClientMessageDispatch.Connect("localhost", 25656);
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GameServices.AddService<SpriteBatch>(spriteBatch);
			FontRepository.System = Content.Load<SpriteFont>("Fonts/System");
		}

		protected override void UnloadContent() {
		}

		public static void SetScope(EntityScope scope) {
			ActiveScope = scope;
			drawQueue = new DrawQueue(ActiveScope);
		}

		protected override void Update(GameTime gameTime) {
			long startTime = GameServices.TimeElapsed();
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			EntityController.Tick += 1;

			EntityController.SetSnapshotsForMoment();

			ClientMessageDispatch.ProcessIncomingMessages();

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

			if(keyboardState.IsKeyDown(Keys.W) && !previousKeyboardState.IsKeyDown(Keys.W)) {
				CommandController.ParseCommand("+north");
			} else if(!keyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyDown(Keys.W)) {
				CommandController.ParseCommand("-north");
			}

			if(keyboardState.IsKeyDown(Keys.A) && !previousKeyboardState.IsKeyDown(Keys.A)) {
				CommandController.ParseCommand("+west");
			} else if(!keyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyDown(Keys.A)) {
				CommandController.ParseCommand("-west");
			}

			if(keyboardState.IsKeyDown(Keys.S) && !previousKeyboardState.IsKeyDown(Keys.S)) {
				CommandController.ParseCommand("+south");
			} else if(!keyboardState.IsKeyDown(Keys.S) && previousKeyboardState.IsKeyDown(Keys.S)) {
				CommandController.ParseCommand("-south");
			}

			if(keyboardState.IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D)) {
				CommandController.ParseCommand("+east");
			} else if(!keyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyDown(Keys.D)) {
				CommandController.ParseCommand("-east");
			}

			if(IsKeyPressed(keyboardState, Keys.E)) {
				CommandController.ParseCommand("lerp_delay 500");
			}

			if(IsKeyPressed(keyboardState, Keys.R)) {
				CommandController.ParseCommand("lerp_delay 100");
			}

			if(mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed) {
				Vector2 localMousePoint = Camera.ConvertViewportToCamera(new Vector2(mouseState.X, mouseState.Y));
				EventData eventData = new EventData(new Dictionary<string, object> {
					{ "position", localMousePoint },
				});
				EventController.Fire("click", eventData);
				if(eventData.ContinuePropagating && ActiveScope != null) {
					foreach(BaseEntity e in ActiveScope.GetEntities().Where(e => e.HasPropertyEnabled("click"))) {
						if(e.GetDrawBoundry().Contains((Vector2)eventData["position"])) {
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

			ModuleController.InvokeStartupMethod("Update");

			if(ActiveScope != null) {
				foreach(BaseEntity entity in ActiveScope.GetEntities()) {
					if(entity.HasPropertyEnabled("update")) {
						entity.Update();
					}
				}
			}

			base.Update(gameTime);
			UpdateTime = GameServices.TimeElapsed() - startTime;
		}

		private bool IsKeyPressed(KeyboardState keyboardState, Keys key) {
			return keyboardState.IsKeyDown(key) && !previousKeyboardState.GetPressedKeys().Contains(key);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Canvas.LoadCameraBoundsForFrame();

			long start = GameServices.TimeElapsed();
			Matrix transformationMatrix = Camera.GetTransformationMatrix();
			Matrix inverseMatrix = Matrix.Invert(transformationMatrix);
			spriteBatch.Begin(transformMatrix: transformationMatrix);
			foreach(PlumeAPI.Modularization.Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod("Draw", new object[] { });
			}

			if(ActiveScope != null) {
				foreach(List<Action> layer in drawQueue.ProcessAndReturnDrawQueue().Values) {
					foreach(Action operation in layer) {
						operation.Invoke();
					}
				}
			}

			//UI Layer
			float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
			spriteBatch.DrawString(FontRepository.System,
				"FPS: " + Math.Round(frameRate) + " | Draw: " + (GameServices.TimeElapsed() - start) + "ms | Update: " + Math.Round(UpdateTime) + 
				"ms | Ping: " + Math.Round(ClientMessageDispatch.Ping*1000) + "ms",
				Camera.ConvertViewportToCamera(new Vector2(5, 5)), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void HandleGameExiting(object sender, EventArgs e) {
			/* DEBUG */
#if WINDOWS
			if(!serverWindow.HasExited) serverWindow.Kill();
#endif
			/* END DEBUG */
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
