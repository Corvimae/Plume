using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Networking;

namespace PlumeServer.Networking {
	static class MessageDispatch {

		public static NetServer Server;
		public static List<Client> Clients = new List<Client>();
		static MessageDispatch() {
			MessageController.RegisterIncomingMessageType("RequestConnection", new RequestConnectionMessageHandler());
		}
		public static void Send(MessageHandler handler, Client recipient) {
			NetOutgoingMessage message = handler.CreateMessage(Server);
			Server.SendMessage(message, recipient.Connection, NetDeliveryMethod.ReliableOrdered, 0);
		}

		public static void Broadcast(MessageHandler handler) {
			NetOutgoingMessage message = handler.CreateMessage(Server);
			Server.SendMessage(message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
		}

		public static void Handle(string type, NetIncomingMessage message) {
			if(MessageController.MessageTypes.ContainsKey(type)) {
				MessageController.MessageTypes[type].Handle(message);
			} else {
				throw new InvalidMessageTypeException();
			}
		}

		public static PlumeServerClient GetSender(NetIncomingMessage message) {
			try {
				return (PlumeServerClient) MessageDispatch.Clients.First(x => message.SenderConnection == x.Connection);
			} catch (Exception) {
				Console.WriteLine("Could not find sender! Perhaps they've disconnected?");
				return null;
			}
		}
	}
}
