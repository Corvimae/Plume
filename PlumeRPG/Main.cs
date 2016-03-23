using PlumeAPI.Modularization;
using System.Diagnostics;

namespace PlumeRPG {
	class Main : PlumeModule {
		public Main() {
			Debug.WriteLine("PlumeCore launched");
		}

		public override void AfterLoad() {
			Debug.WriteLine("PlumeCore loaded!");
		}
	}
}