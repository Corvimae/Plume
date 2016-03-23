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

		static int nextHighestId = 0;
		static MessageController() {
			MessageController.RegisterMessageType("SyncMessageTypes", new SyncMessageTypesMessageHandler());
			MessageController.RegisterMessageType("UpdateEntity", new UpdateEntityMessageHandler(null));
			MessageController.RegisterMessageType("SendScope", new SendScopeMessageHandler(null));
			MessageController.RegisterMessageType("RequestConnection", new RequestConnectionMessageHandler(null));
		}
		public static void RegisterMessageType(string type, MessageHandler handlerInstance) {
			if(!MessageTypeIds.ContainsKey(type)) {
				int id = RegisterMessageTypeId(type);
				MessageTypes.Add(id, handlerInstance);
			}
		}

		public static int RegisterMessageTypeId(string type) {
			int id = nextHighestId++;
			MessageTypeIds.Add(type, id);
			return id;
		}

		public static int GetMessageTypeId(string name) {
			return MessageTypeIds[name];
		}

		public static int GetMessageTypeId(object handler) {
			return MessageTypeIds[handler.GetType().Name];
		}

		public static int GetMessageTypeId(Type handler) {
			return MessageTypeIds[handler.Name];
		}
	}

	public class InvalidMessageTypeException : Exception { };
}
