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

		public override NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			message.Write(Username);
			return message;
		}

		public override void Handle(NetIncomingMessage message) {
			base.Handle(message);
			int id = message.ReadInt32();
			string username = message.ReadString();
			Console.WriteLine("Connection requested from " + username + " (" + message.SenderConnection.RemoteEndPoint.Address + ")");
			Client newClient = new Client(username, message.SenderConnection);
			ServerMessageDispatch.Clients.Add(newClient);
			newClient.Connection.Approve();
		}
	}
}
