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
using System.Timers;

namespace PlumeServer {
	class ServerCore {
		DateTime LastUpdate = DateTime.Now;

		public void Begin() {
			Console.WriteLine("Server ready, waiting for new connections.");
			while(true) {
				DateTime time = DateTime.Now;
				if((time - LastUpdate).TotalMilliseconds >= Configuration.TickRate) {
					ServerMessageDispatch.IncrementTick();
					Update();
					LastUpdate = time;
				}	
			}
		}

		public void Update() {
			NetIncomingMessage rawMessage;
			if((rawMessage = ServerMessageDispatch.Server.ReadMessage()) != null) {
				IncomingMessage message = new IncomingMessage(rawMessage);
				if(rawMessage.MessageType == NetIncomingMessageType.ConnectionApproval) {
					ServerMessageDispatch.Handle(MessageController.GetMessageTypeId("PlumeAPI.Networking.Builtin.RequestConnectionMessageHandler"), message);
				} else if(rawMessage.MessageType == NetIncomingMessageType.Data) {
					ServerMessageDispatch.Handle(message.ReadInt32(), message);
				} else if(rawMessage.MessageType == NetIncomingMessageType.StatusChanged) {
					if(rawMessage.SenderConnection.Status == NetConnectionStatus.Connected) {
						Client client = ServerMessageDispatch.GetSender(message);
						client.Message(new SendModuleRequirementsMessageHandler());
						client.Message(new SyncMessageTypesMessageHandler());
						ModuleController.InvokeStartupMethod("UserConnected", client);
						client.SendInitialConnectionData();
					} else if(rawMessage.SenderConnection.Status == NetConnectionStatus.Disconnected || rawMessage.SenderConnection.Status == NetConnectionStatus.Disconnecting) {
						Client sender = ServerMessageDispatch.GetSender(message);
						ModuleController.InvokeStartupMethod("UserDisconnected", sender);
						MessageController.Clients.Remove(sender);
						Console.WriteLine(sender.Name + " has disconnected.");
					}
				}
			}

			foreach(EntityScope scope in ScopeController.GetAllScopes()) {
				scope.Update();
			}
		}
	}
}
