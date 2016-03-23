using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class MessageHandler {
		public string Name;
		public NetOutgoingMessage CreateMessage(NetPeer peer) {
			NetOutgoingMessage message = peer.CreateMessage();
			message.Write(MessageController.GetMessageTypeId(Name));
			message = PackageMessage(message);
			return message;
		}

		public virtual NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			return message;
		}

		public virtual void Handle(NetIncomingMessage message) {
		}
	}
}
