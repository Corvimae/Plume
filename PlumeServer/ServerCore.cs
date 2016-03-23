using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeServer {
	class ServerCore {
		TimeSpan RefreshRate = new TimeSpan(0, 0, 0, 0, 16);

		public void Begin() {
			DateTime time = DateTime.Now;
			Console.WriteLine("Server started, waiting for new connections.");
			while(true) {
				Update();
			}
		}

		public void Update() {
			NetIncomingMessage message;
			if((message = ServerMessageDispatch.Server.ReadMessage()) != null) {
				if(message.MessageType == NetIncomingMessageType.ConnectionApproval) {
					ServerMessageDispatch.Handle(MessageController.GetMessageTypeId("RequestConnection"), message);
				} else if(message.MessageType == NetIncomingMessageType.Data) {
					ServerMessageDispatch.Handle(message.ReadInt32(), message);
				} else if(message.MessageType == NetIncomingMessageType.StatusChanged) {
					if(message.SenderConnection.Status == NetConnectionStatus.Connected) {
						Client client = ServerMessageDispatch.GetSender(message);
						client.Message(new SyncMessageTypesMessageHandler());
						ModuleController.InvokeStartupMethod("UserConnected", client);
						client.SendInitialConnectionData();
					} else if(message.SenderConnection.Status == NetConnectionStatus.Disconnected || message.SenderConnection.Status == NetConnectionStatus.Disconnecting) {
						Client sender = ServerMessageDispatch.GetSender(message);
						ModuleController.InvokeStartupMethod("UserDisconnected", sender);
						MessageController.Clients.Remove(sender);
						Console.WriteLine(sender.Name + " has disconnected.");
					}
				}
			}

			foreach(BaseEntity entity in ScopeController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("update")) entity.Update();
			}
		}
	}
}
