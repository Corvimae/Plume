using Lidgren.Network;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public static class ClientMessageDispatch {
		static NetClient Client;
		static NetPeerConfiguration Configuration;
		public static float Ping;
		public static void Connect(string ip, int port) {
			Configuration = new NetPeerConfiguration("PlumeServer");
			Configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
			Client = new NetClient(Configuration);
			Client.Start();
			OutgoingMessage connectionRequest = CreateMessage(new RequestConnectionMessageHandler("AcceptableIce"));
			try {
				NetConnection connection = Client.Connect(ip, port, connectionRequest.GetMessage());
			} catch(NetException) {
				Debug.WriteLine("Unable to resolve host.");
			}
		}
		public static void Send(MessageHandler handler) {
			Client.SendMessage(CreateMessage(handler).GetMessage(), NetDeliveryMethod.ReliableOrdered);
		}
		public static OutgoingMessage CreateMessage(MessageHandler handler) {
			return handler.CreateMessage(Client);
		}

		public static void Disconnect(string message) {
			Client.Disconnect(message);
		}
		public static void Handle(int type, IncomingMessage message) {
			if(Client.ConnectionStatus == NetConnectionStatus.Connected) {
				if(MessageController.MessageTypes.ContainsKey(type)) {
					MessageController.MessageTypes[type].Handle(message);
				} else {
					throw new InvalidMessageTypeException();
				}
			}
		}
		public static void ProcessIncomingMessages() {
			NetIncomingMessage message;
			while((message = Client.ReadMessage()) != null) {
				if(message.MessageType == NetIncomingMessageType.ConnectionLatencyUpdated) {
					Ping = message.ReadFloat();
				} else if(message.MessageType == NetIncomingMessageType.StatusChanged) {
					if(message.SenderConnection.Status == NetConnectionStatus.Connected) {
						Debug.WriteLine("Connection approved by server.");
					} else if(message.SenderConnection.Status == NetConnectionStatus.Disconnected || message.SenderConnection.Status == NetConnectionStatus.Disconnecting) {
						Debug.WriteLine("Disconnected from server.");
					}
				} else if(message.MessageType == NetIncomingMessageType.Data) {
					int id = message.ReadInt32();
					Handle(id, new IncomingMessage(message));
				}
			}
		}
	}
}
