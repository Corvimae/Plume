using CoreEngine.Attributes;
using CoreEngine.Modularization;
using System.Diagnostics;

namespace PlumeCore {
	[PlumeStartup]
	class Main : PlumeModule {
		public Main() {
			Debug.WriteLine("PlumeCore launched");
		}

		public override void AfterLoad() {
			Debug.WriteLine("PlumeCore loaded!");
		}
	}
}