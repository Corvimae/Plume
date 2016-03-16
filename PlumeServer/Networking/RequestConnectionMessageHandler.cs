using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.Networking;

namespace PlumeServer.Networking {
	class RequestConnectionMessageHandler : MessageHandler {
		public override void Handle(NetIncomingMessage message) {
			base.Handle(message);
			string username = message.ReadString();
			Console.WriteLine("Connection requested from " + username + " (" + message.SenderConnection.RemoteEndPoint.Address + ")");
			PlumeServerClient newClient = new PlumeServerClient(username, message.SenderConnection);
			MessageDispatch.Clients.Add(newClient);
			newClient.Connection.Approve();
		}
	}
}
