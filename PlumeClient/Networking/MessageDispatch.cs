using Lidgren.Network;
using PlumeAPI.Networking;
using PlumeClient.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeClient.Networking {
	public static class MessageDispatch {
		static NetClient Client;
		static NetPeerConfiguration Configuration;

		static MessageDispatch() {
		}

		public static void Connect(string ip, int port) {
			Configuration = new NetPeerConfiguration("PlumeServer");
			Client = new NetClient(Configuration);
			Client.Start();
			NetOutgoingMessage connectionRequest = CreateMessage(new RequestConnectionMessageHandler("AcceptableIce"));
			try {
				NetConnection connection = Client.Connect(ip, port, connectionRequest);
			} catch (NetException) {
				Debug.WriteLine("Unable to resolve host.");
			}
		}
		public static void Send(MessageHandler handler) {
			Client.SendMessage(CreateMessage(handler), NetDeliveryMethod.ReliableOrdered);
		}
		public static NetOutgoingMessage CreateMessage(MessageHandler handler) {
			return handler.CreateMessage(Client);
		}
		public static void Handle(string type, NetIncomingMessage message) {
			if(MessageController.MessageTypes.ContainsKey(type)) {
				MessageController.MessageTypes[type].Handle(message);
			} else {
				throw new InvalidMessageTypeException();
			}
		}
		public static void ProcessIncomingMessages() {
			NetIncomingMessage message;
			while((message = Client.ReadMessage()) != null) {
				if(message.MessageType == NetIncomingMessageType.StatusChanged) {
					if(message.SenderConnection.Status == NetConnectionStatus.Connected) {
						Debug.WriteLine("Connection approved by server.");
					}
				}else if(message.MessageType == NetIncomingMessageType.Data) {
					MessageDispatch.Handle(message.ReadString(), message);
				}
			}
		}
	}
}
