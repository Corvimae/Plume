using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Diagnostics;
using PlumeAPI.Entities;
using PlumeAPI.Attributes;
using PlumeAPI.Graphics;

namespace PlumeRPG.Entities {
	public class MapTile : BaseEntity {
		public Vector2 Coordinates;
		public Texture2D Texture;

		[RunOnLoad(new string[] { "MapTile" })]
		public new static void Register() {
			RegisterTexture("texture", Name.Split('.').Last() + ".png");
			Debug.WriteLine("Calling registration function for " + MethodBase.GetCurrentMethod().DeclaringType.Name + "(as " + Name + ")");
			SetEntityProperty("draw", true);
			SetEntityProperty("click", true);
		}

		public MapTile(int x, int y) : base() {
			Coordinates = new Vector2(x, y);
			SetPosition(x * 32, y * 32);
			SetDrawDimensions(32, 32);
			Texture = GetTexture("texture");
		}

		public override void Draw() {
			Canvas.DrawTexture(Texture, Position.X, Position.Y, Color.White);
		}
	}
}