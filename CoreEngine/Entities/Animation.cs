using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CoreEngine.Scripting;

namespace CoreEngine.Entities {
	public class Animation {
		public Texture2D SpriteSheet;
		public int CellDuration = 1;
		public int TotalCells = 1;
		public Vector2 CellDimensions;

		public bool FlipHorizontal = false;
		public bool FlipVertical = false;
	
		public bool Paused = false;

		private int FramesPassed = 0;
		private Vector2 Origin;

		public Animation(Texture2D spriteSheet, Vector2 dimensions, int cellDuration, int totalCells) {
			this.SpriteSheet = spriteSheet;
			this.CellDimensions = dimensions;
			this.CellDuration = cellDuration;
			this.TotalCells = totalCells;
			this.Origin = new Vector2(CellDimensions.X / 2.0f, CellDimensions.Y); 
		}

		public void Draw(Vector2 position, CoreColor color) {
			Rectangle clip = new Rectangle((int) (Math.Floor((double) FramesPassed / CellDuration) * CellDimensions.X), 0,
																		 (int) CellDimensions.X, (int)CellDimensions.Y);
			
			SpriteEffects spriteEffects;
			if(FlipHorizontal && FlipVertical) {
				spriteEffects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
			} else if(FlipHorizontal) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			} else if(FlipVertical) {
				spriteEffects = SpriteEffects.FlipVertically;
			} else {
				spriteEffects = SpriteEffects.None;
			}

			Canvas.DrawTexture(SpriteSheet, position, clip, color, Origin, spriteEffects);

			if(!Paused) {
				FramesPassed += 1;

				//Account for 0-index
				if(FramesPassed > (TotalCells - 1) * CellDuration) {
					FramesPassed -= (TotalCells - 1) * CellDuration;
				}
			}
		}

		public void Reset() {
			FramesPassed = 0;
		}


		public void Pause() {
			Paused = true;
		}

		public void Resume() {
			Paused = false;
		}

		public Animation Clone() {
			Animation newAnimation =  new Animation(SpriteSheet, CellDimensions, CellDuration, TotalCells);
			return newAnimation;
		}
	}
}

