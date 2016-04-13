﻿using PlumeAPI.Graphics;
using Microsoft.Xna.Framework;
using PlumeRPG.Entities;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;
using PlumeAPI.Entities;

namespace DevModule.Tiles {
	public class Stone : MapTile {
		Random random;
		public Stone() : base() {
		}

		public int Bazoople { get; set; }

		public Stone(int x, int y) : base(x, y) {
			DrawOnLayer(2, "SecondDraw");
			SetEntityProperty("update", true);
			Bazoople = 0;
			random = new Random(("" + Position.X + Position.Y).GetHashCode());
			RegisterProperty(EntityPropertyType.Syncable, "Bazoople", () => { return Bazoople; }, (value) => { Bazoople = (int)value; });
		}

		public override void UpdateServer() {
			if(random.Next(0,100) > 80) Bazoople++;
		}

		public override void Draw() {
			base.Draw();
			Canvas.DrawString(FontRepository.System, "" + Bazoople, (int) Position.X, (int) Position.Y, Color.White);
		}
		public void SecondDraw() {
			Canvas.DrawFilledRect((int) Position.X, (int) Position.Y, 4, 4, Color.Black);
		}
	}
}
