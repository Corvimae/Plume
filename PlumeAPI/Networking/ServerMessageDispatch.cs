using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Networking;
using PlumeAPI.World;
using PlumeAPI.Networking.Builtin;

namespace PlumeAPI.Networking {
	public static class ServerMessageDispatch {

		public static NetServer Server;
		public static List<Client> Clients = new List<Client>();

		static long Tick;

		public static void Send(MessageHandler handler, Client recipient) {
			NetOutgoingMessage message = handler.CreateMessage(Server).GetMessage();
			Server.SendMessage(message, recipient.Connection, NetDeliveryMethod.ReliableOrdered, 0);
		}

		public static void Broadcast(MessageHandler handler) {
			if(Server.Connections.Count() > 0) {
				NetOutgoingMessage message = handler.CreateMessage(Server).GetMessage();
				Server.SendMessage(message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}

		public static void SendToScope(MessageHandler handler, EntityScope scope) {
			NetOutgoingMessage message = handler.CreateMessage(Server).GetMessage();
			List<NetConnection> connections = GetClientsInScope(scope).Select(x => x.Connection).ToList<NetConnection>();
			if(connections.Count() > 0) {
				Server.SendMessage(message, connections, NetDeliveryMethod.ReliableOrdered, 0);
			}
		}

		public static void Handle(int type, IncomingMessage message) {
			if(MessageController.MessageTypes.ContainsKey(type)) {
				MessageController.MessageTypes[type].Handle(message);
			} else {
				throw new InvalidMessageTypeException();
			}
		}

		public static Client GetSender(IncomingMessage message) {
			try {
				return (Client) Clients.First(x => message.GetMessage().SenderConnection == x.Connection);
			} catch (Exception) {
				Console.WriteLine("Could not find sender! Perhaps they've disconnected?");
				return null;
			}
		}

		public static void IncrementTick() {
			Tick += 1;
		}

		public static long GetTick() {
			return Tick;
		}

		public static List<Client> GetClientsInScope(EntityScope scope) {
			return Clients.Where(x => x.Scope == scope).ToList<Client>();
		}
	}
}
