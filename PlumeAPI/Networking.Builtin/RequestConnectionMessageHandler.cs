using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.Networking;
using PlumeAPI.Modularization;
using PlumeAPI.World;

namespace PlumeAPI.Networking.Builtin {
	class RequestConnectionMessageHandler : MessageHandler {
		string Username;

		public RequestConnectionMessageHandler(string username) {
			this.Username = username;
		}

		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			message.Write(Username);
			return message;
		}

		public override void Handle(IncomingMessage message) {
			int id = message.ReadInt32();
			string username = message.ReadString();
			Console.WriteLine("Connection requested from " + username + " (" + message.GetMessage().SenderConnection.RemoteEndPoint.Address + ")");
			Client newClient = new Client(username, message.GetMessage().SenderConnection);
			ServerMessageDispatch.Clients.Add(newClient);
			newClient.Connection.Approve();
		}
	}
}
