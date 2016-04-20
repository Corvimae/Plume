using Microsoft.Xna.Framework;
using PlumeAPI.Entities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeRPG.Entities {
	[Flags]
	public enum ActorMotion {
		None = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8
	}
}
