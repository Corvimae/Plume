using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class OutgoingMessage : Message {
		public OutgoingMessage(NetOutgoingMessage message) : base(message)  {}

		internal NetOutgoingMessage GetMessage() {
			return (NetOutgoingMessage) _message;
		}
	}
}
