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
		static Queue<PendingOutgoingMessage> PendingMessages = new Queue<PendingOutgoingMessage>();
	
		static long Tick;

		public static void Send(MessageHandler handler, Client recipient) {
			PendingMessages.Enqueue(new PendingOutgoingMessage(handler.CreateMessage(Server), recipient.Connection));
		}

		public static void Broadcast(MessageHandler handler) {
			if(Server.Connections.Count() > 0) {
				PendingMessages.Enqueue(new PendingOutgoingMessage(handler.CreateMessage(Server), Server.Connections));
			}
		}

		public static void SendToScope(MessageHandler handler, EntityScope scope) {
			List<NetConnection> connections = GetClientsInScope(scope).Select(x => x.Connection).ToList<NetConnection>();
			if(connections.Count() > 0) {
				PendingMessages.Enqueue(new PendingOutgoingMessage(handler.CreateMessage(Server), connections));
			}
		}

		public static void Handle(int type, IncomingMessage message) {
			if(MessageController.MessageTypes.ContainsKey(type)) {
				MessageController.MessageTypes[type].Handle(message);
			} else {
				throw new InvalidMessageTypeException();
			}
		}

		public static void Process() {
			while(true) {
				while(PendingMessages.Count > 0) {
					PendingOutgoingMessage message = PendingMessages.Dequeue();
					Server.SendMessage(message.Message.GetMessage(), message.Recipients, NetDeliveryMethod.ReliableOrdered, 0);
				}
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
