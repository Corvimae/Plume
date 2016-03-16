using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.Networking;

namespace PlumeClient.Networking {
	class RequestConnectionMessageHandler : MessageHandler {
		string Username;

		public RequestConnectionMessageHandler(string username) {
			this.Name = "RequestConnection";
			this.Username = username;
		}

		public override NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			message.Write(Username);
			return message;
		}

		public override void Handle(NetIncomingMessage message) {
			throw new NotImplementedException();
		}

	}
}
