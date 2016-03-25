using Lidgren.Network;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public static class MessageController {
		public static Dictionary<string, int> MessageTypeIds = new Dictionary<string, int>();
		public static Dictionary<int, MessageHandler> MessageTypes = new Dictionary<int, MessageHandler>();
		public static List<Client> Clients = new List<Client>();

		static int NextHighestId = 0;
		static MessageController() {
			MessageController.RegisterMessageType(new SyncMessageTypesMessageHandler());
			MessageController.RegisterMessageType(new SyncEntityIdsMessageHandler());
			MessageController.RegisterMessageType(new UpdateEntityMessageHandler(null));
			MessageController.RegisterMessageType(new SendScopeMessageHandler(null));
			MessageController.RegisterMessageType(new RequestConnectionMessageHandler(null));
			MessageController.RegisterMessageType(new SendScopeSnapshotMessageHandler(null));
		}
		public static void RegisterMessageType(MessageHandler handlerInstance) {
			string type = handlerInstance.GetType().FullName;
			if(!MessageTypeIds.ContainsKey(type)) {
				int id = RegisterMessageTypeId(type);
				MessageTypes.Add(id, handlerInstance);
			}
		}

		static int RegisterMessageTypeId(string type) {
			int id = NextHighestId++;
			MessageTypeIds.Add(type, id);
			return id;
		}

		public static int GetMessageTypeId(string name) {
			try {
				return MessageTypeIds[name];
			} catch (Exception) {
				Console.WriteLine("Unable to find message type of name " + name + ".");
				return -1;
			}
		}

		public static int GetMessageTypeId(object handler) {
			return MessageTypeIds[handler.GetType().Name];
		}

		public static int GetMessageTypeId(Type handler) {
			return MessageTypeIds[handler.Name];
		}
	}

	public class InvalidMessageTypeException : Exception { };

	public class MessageHandlerUnnamedException : Exception {
		Type Type;
		public MessageHandlerUnnamedException(Type type) {
			Type = type;
		}
	}
}
