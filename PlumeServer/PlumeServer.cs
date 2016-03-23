using Lidgren.Network;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.World;
using PlumeServer.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlumeServer {
	class PlumeServer {
		static NetPeerConfiguration Configuration;
		static ServerCore Core;

		static void Main(string[] args) {
			ModuleController.SetEnvironment(PlumeEnvironment.Server);
			AppDomain.CurrentDomain.AssemblyResolve += ResolveDuplicateAssembly;

			string[] modules = new string[] { "DevModule", "PlumeRPG" };
			foreach(string module in modules) ModuleController.RegisterModule(module);
			ModuleController.ResolveDependencies();
			ModuleController.ImportModules();

			Map testMap = new Map("MyMap", 50, 50);

			Configuration = new NetPeerConfiguration("PlumeServer");
			Configuration.Port = 25656;
			Configuration.MaximumConnections = 4;
			Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

			ServerMessageDispatch.Server = new NetServer(Configuration);
			ServerMessageDispatch.Server.Start();
			Console.WriteLine(Configuration.AppIdentifier + " server started on port " + Configuration.Port + ".");
			Core = new ServerCore();
			Core.Begin();
		}

		static Assembly ResolveDuplicateAssembly(object source, ResolveEventArgs e) {
			if(!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().FullName == e.Name)) {
				return Assembly.Load(e.Name);
			} else {
				Console.WriteLine("Duplicate assembly " + e.Name + " skipped.");
				return AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().FullName == e.Name);
			}
		}
	}
}