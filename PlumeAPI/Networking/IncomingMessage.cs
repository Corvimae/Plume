using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class IncomingMessage : Message {
		public IncomingMessage(NetIncomingMessage message) : base(message)  {}

		internal NetIncomingMessage GetMessage() {
			return (NetIncomingMessage) _message;
		}
	}
}
