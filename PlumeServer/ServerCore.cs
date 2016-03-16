using Lidgren.Network;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeServer.Networking;
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
			if((message = MessageDispatch.Server.ReadMessage()) != null) {
				Console.WriteLine(message.MessageType);
				if(message.MessageType == NetIncomingMessageType.ConnectionApproval) {
					MessageDispatch.Handle("RequestConnection", message);
				} else if(message.MessageType == NetIncomingMessageType.Data) {
					MessageDispatch.Handle(message.ReadString(), message);
				} else if(message.MessageType == NetIncomingMessageType.StatusChanged) {
					if(message.SenderConnection.Status == NetConnectionStatus.Disconnected || message.SenderConnection.Status == NetConnectionStatus.Disconnecting) {
						PlumeServerClient sender = (PlumeServerClient)MessageDispatch.GetSender(message);
						ModuleController.InvokeStartupMethod("UserDisconnected", sender);
						MessageController.Clients.Remove(sender);
						Console.WriteLine(sender.Name + " has disconnected.");
					}
				}
			}
		}
	}
}
