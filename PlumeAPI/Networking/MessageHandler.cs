using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class MessageHandler {
		public OutgoingMessage CreateMessage(NetPeer peer) {
			OutgoingMessage message = new OutgoingMessage(peer.CreateMessage());
			message.Write(MessageController.GetMessageTypeId(GetType().FullName));
			message = PackageMessage(message);
			return message;
		}

		public virtual OutgoingMessage PackageMessage(OutgoingMessage message) {
			return message;
		}

		public virtual void Handle(IncomingMessage message) {
		}
	}
}
