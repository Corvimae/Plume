using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Utilities {
	public static class ClientConfiguration {

		public static int InterpolationLimit = 100;
		public static int InterpolationDelay = 100;
		public static bool ClientPredictionSmoothing = false;

		public static int TickRate = 60;
	}
}
