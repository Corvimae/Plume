using Lidgren.Network;
using PlumeServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeServer {
	class PlumeServer {
		static NetPeerConfiguration Configuration;
		static ServerCore Core;

		static void Main(string[] args) {
			Configuration = new NetPeerConfiguration("PlumeServer");
			Configuration.Port = 25656;
			Configuration.MaximumConnections = 4;
			Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

			MessageDispatch.Server = new NetServer(Configuration);
			MessageDispatch.Server.Start();
			Console.WriteLine(Configuration.AppIdentifier + " server started on port " + Configuration.Port + ".");
			Core = new ServerCore();
			Core.Begin();
		}
	}
}