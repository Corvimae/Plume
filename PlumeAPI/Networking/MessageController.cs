using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public static class MessageController {
		public static Dictionary<string, MessageHandler> MessageTypes = new Dictionary<string, MessageHandler>();
		public static List<Client> Clients = new List<Client>();
		public static void RegisterIncomingMessageType(string type, MessageHandler handlerInstance) {
			MessageTypes.Add(type, handlerInstance);
		}
	}

	public class InvalidMessageTypeException : Exception { };
}
