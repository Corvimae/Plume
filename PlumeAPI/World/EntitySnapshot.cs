using Lidgren.Network;
using Microsoft.Xna.Framework;
using PlumeAPI.Attributes;
using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class EntitySnapshot {
		public int Id;
		Dictionary<string, object> SyncableProperties = new Dictionary<string, object>();
		static Dictionary<Type, Action<object, NetOutgoingMessage>> MessageWriters = new Dictionary<Type, Action<object, NetOutgoingMessage>>();
		static Dictionary<Type, Func<NetIncomingMessage, object>> MessageReaders = new Dictionary<Type, Func<NetIncomingMessage, object>>();
		public EntitySnapshot(BaseEntity entity) {
			Id = entity.Id;

			EntitySnapshot previousEntitySnapshot = null;
			ScopeSnapshot previousScopeSnapshot = entity.Scope.PreviousSnapshot;
			if(previousScopeSnapshot != null) {
				previousEntitySnapshot = previousScopeSnapshot.GetSnapshotForEntity(Id);
			}

			IEnumerable<PropertyInfo> properties = entity.GetSyncableProperties();
			foreach(PropertyInfo property in properties) {
				object value = property.GetValue(entity);
				if(previousEntitySnapshot == null || previousEntitySnapshot.SyncableProperties[property.Name] != value) {
					SyncableProperties.Add(property.Name, value);
				}
			}
		}

		public void AddToMessage(NetOutgoingMessage message) {
			message.Write(Id);
			foreach(KeyValuePair<string, object> property in SyncableProperties) {
				try {
					MessageWriters[property.Value.GetType()].Invoke(property.Value, message);
				} catch(KeyNotFoundException) {
					Console.WriteLine("Unable to parse syncable property " + property.Key + ": no valid message writer for type " + property.Value.GetType().FullName + ".");
				}
			}
		}

		public static void RegisterTypeHandler(Type type, Func<NetIncomingMessage, object> incoming, Action<object, NetOutgoingMessage> outgoing) {
			MessageReaders.Add(type, incoming);
			MessageWriters.Add(type, outgoing);
		}

		public static object ReadType(Type type, NetIncomingMessage message) {
			try {
				return MessageReaders[type].Invoke(message);
			} catch(KeyNotFoundException) {
				Console.WriteLine("Unable to parse syncable property " + type + ": no valid message reader for type " + type + ".");
				return null;
			}
		}

		static EntitySnapshot() {
			RegisterTypeHandler(
				typeof(System.Byte),
				(message) => { return message.ReadByte(); },
				(obj, message) => { message.Write((byte)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Int16),
				(message) => { return message.ReadInt16(); },
				(obj, message) => { message.Write((short)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Int32),
				(message) => { return message.ReadInt32(); },
				(obj, message) => { message.Write((int)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Int64),
				(message) => { return message.ReadInt64(); },
				(obj, message) => { message.Write((long)obj); }
			);

			RegisterTypeHandler(
				typeof(System.String),
				(message) => { return message.ReadString(); },
				(obj, message) => { message.Write((string)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Single),
				(message) => { return message.ReadFloat(); },
				(obj, message) => { message.Write((float)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Double),
				(message) => { return message.ReadDouble(); },
				(obj, message) => { message.Write((double)obj); }
			);

			RegisterTypeHandler(
				typeof(System.Boolean),
				(message) => {return message.ReadBoolean(); },
				(obj, message) => { message.Write((bool)obj); }
			);

			RegisterTypeHandler(
				typeof(Microsoft.Xna.Framework.Vector2),
				(message) => { return new Vector2(message.ReadFloat(), message.ReadFloat()); },
				(obj, message) => {
					Vector2 vec2 = (Vector2)obj;
					message.Write(vec2.X);
					message.Write(vec2.Y);
				}
			);
		}
	}

}
