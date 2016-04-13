using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using PlumeAPI.Utilities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace PlumeServer {
	class ServerCore {
		long LastUpdate = 0;
		public static long Updates = 0;

		public void Begin() {
			Console.WriteLine("Server ready, waiting for new connections.");
			GameServices.StartTimer();

			Thread sendMessagesThread = new Thread(() => ServerMessageDispatch.Process());
			sendMessagesThread.Start();

			Thread processIncomingThread = new Thread(() => ProcessIncomingMessages());
			processIncomingThread.Start();

			long totalUpdateTime = 0;

			while(true) {
				long timeElapsed = GameServices.TimeElapsed();
				if(timeElapsed - LastUpdate >= (1000 / ServerConfiguration.TickRate)) {
					ServerMessageDispatch.IncrementTick();
					Update();
					totalUpdateTime += GameServices.TimeElapsed() - timeElapsed;
					if(Updates % 100 == 0) Console.WriteLine("Previous update took " + (GameServices.TimeElapsed() - timeElapsed) + "ms");

					LastUpdate = timeElapsed;
					Updates++;

					if(Updates % 100 == 0) Console.WriteLine((totalUpdateTime/ Updates) + "ms / update");
				}	
			}
		}

		public void ProcessIncomingMessages() {
			while(true) {
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
			}
		}
		public void Update() {
			foreach(EntityScope scope in ScopeController.GetAllScopes()) {
				scope.Update();
			}

			ModuleController.InvokeStartupMethod("Update");
		}
	}
}
