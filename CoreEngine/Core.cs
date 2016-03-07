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

namespace CoreEngine {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Core : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Camera viewport;
		Map activeMap;

		public Core() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
		}

		protected override void Initialize() {
			base.Initialize();

			GameServices.AddService<GraphicsDevice>(GraphicsDevice);

			ModuleControl.RegisterModule("DevModule");

			activeMap = new Map(50, 50);
			viewport = new Camera(0, 0);
			GameServices.AddService<Camera>(viewport);

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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}

			if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
				viewport.SetYPosition(viewport.GetYPosition() - 5);
			} else if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
				viewport.SetYPosition(viewport.GetYPosition() + 5);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
				viewport.SetXPosition(viewport.GetXPosition() - 5);
			} else if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
				viewport.SetXPosition(viewport.GetXPosition() + 5);
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(-viewport.X, -viewport.Y, 0));
			activeMap.draw(spriteBatch);

			float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
			spriteBatch.End();

			//UI Layer
			spriteBatch.Begin();
			spriteBatch.DrawString(CoreFont.System, "FPS: " + Math.Round(frameRate), new Vector2(5, 5), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
