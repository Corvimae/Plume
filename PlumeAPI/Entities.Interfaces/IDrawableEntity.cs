using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlumeAPI.Entities.Interfaces {
	interface IDrawableEntity {

		void Draw();
		Rectangle GetDrawBoundry();
	}
}
