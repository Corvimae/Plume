using PlumeAPI.Entities.Interfaces;
using PlumeAPI.Events;
using PlumeAPI.Modularization;
using PlumeAPI.Graphics;
using PlumeAPI.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lidgren.Network;
using PlumeAPI.World;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;

namespace PlumeAPI.Entities {
	public class BaseEntity : CoreObject, IDrawableEntity {

		public virtual Vector2 Position { get; set; }
		protected Vector2 DrawDimensions;

		public delegate MemberType MemberGetDelegate<MemberType>(BaseEntity obj);
		public delegate MemberType MemberSetDelegate<MemberType>(BaseEntity obj);

		public EntityScope Scope { get; set; }

		protected string Identifier;

		public Dictionary<int, Action> DrawActionRegistry = new Dictionary<int, Action>();

		public static Dictionary<string, bool> EntityProperties = new Dictionary<string, bool> {
			{ "draw", false },
			{ "update", false },
			{ "click", false }
		};

		public int DrawLayer = 0;


		Dictionary<PropertyInfo, EntityPropertyData> NetworkedProperties = new Dictionary<PropertyInfo, EntityPropertyData>();
		Dictionary<EntityPropertyType, EntityPropertyData[]> NetworkedPropertiesByType = new Dictionary<EntityPropertyType, EntityPropertyData[]>() {
			{ EntityPropertyType.Syncable, new EntityPropertyData[0] },
			{ EntityPropertyType.Interpolatable, new EntityPropertyData[0] },
			{ EntityPropertyType.ClientControlledValue, new EntityPropertyData[0] }
		};


		public BaseEntity() { }


		protected void RegisterProperty(EntityPropertyType type, string name, Func<object> getter, Action<object> setter) {
			PropertyInfo info = GetType().GetProperty(name);
			if(info != null) {
				if(NetworkedProperties.ContainsKey(info)) {
					NetworkedProperties[info] = new EntityPropertyData(info, type, getter, setter);
				} else {
					NetworkedProperties.Add(info, new EntityPropertyData(info, type, getter, setter));
				}
				
				Array propertyTypes = Enum.GetValues(typeof(EntityPropertyType));

				Dictionary<EntityPropertyType, List<EntityPropertyData>> temporaryTypeList = new Dictionary<EntityPropertyType, List<EntityPropertyData>>();
				NetworkedPropertiesByType.Clear();

				foreach(EntityPropertyType propertyType in propertyTypes) {
					temporaryTypeList.Add(propertyType, new List<EntityPropertyData>());
				}

				foreach(EntityPropertyData data in NetworkedProperties.Values) {
					foreach(EntityPropertyType propertyType in propertyTypes) {
						if(data.HasPropertyType(propertyType)) {
							temporaryTypeList[propertyType].Add(data);
						}
					}
				}

				foreach(KeyValuePair<EntityPropertyType, List<EntityPropertyData>> pair in temporaryTypeList) {
					NetworkedPropertiesByType.Add(pair.Key, pair.Value.OrderBy(x => x.Info.Name).ToArray());
				}
			} else {
				Console.WriteLine("Unable to find property with name " + name + ".");
			}
		}

		protected virtual void Create(params object[] arguments) { }

		public virtual void Destroy() { }

		public bool HasPropertyEnabled(string setting) {
			return EntityProperties[setting];
		}

		protected void SetEntityProperties(Dictionary<string, bool> properties) {
			foreach(KeyValuePair<string, bool> property in properties) {
				if(EntityProperties.ContainsKey(property.Key)) {
					EntityProperties[property.Key] = (bool)property.Value;
				} else {
					throw new InvalidEntityPropertyException(property.Key, GetName());
				}
			}
		}

		protected void SetEntityProperty(string property, bool value) {
			if(EntityProperties.ContainsKey(property)) {
				EntityProperties[property] = (bool)value;
			} else {
				throw new InvalidEntityPropertyException(property, GetName());
			}
		}

		public int GetDrawLayer() {
			return DrawLayer;
		}
		public Vector2 GetPosition() {
			return Position;
		}
		public Vector2 GetDimensions() {
			return DrawDimensions;
		}

		public void SetPosition(float x, float y) {
			Position = new Vector2(x, y);
		}

		public void SetDrawDimensions(int width, int height) {
			DrawDimensions = new Vector2(width, height);
		}

		public Dictionary<int, Action> GetDrawActionRegistry() {
			return DrawActionRegistry;
		}

		protected void DrawOnLayer(int level, string method) {
			if(!DrawActionRegistry.ContainsKey(level)) {
				DrawActionRegistry.Add(level, (Action)GetDelegate(method));
			}
		}

		protected void UnregisterDrawOnLayer(int level) {
			if(DrawActionRegistry.ContainsKey(level)) {
				DrawActionRegistry.Remove(level);
			}
		}

		public Rectangle GetDrawBoundry() {
			return new Rectangle((int)Position.X, (int)Position.Y, (int)DrawDimensions.X, (int)DrawDimensions.Y);
		}


		public override void UpdateClient() {
			UpdateSyncableProperties();
		}

		public virtual void UpdateClientControlledValues() { }

		protected virtual bool IsClientControlled() {
			return false;
		}

		protected void UpdateSyncableProperties() {
			Interpolate();
			if(IsClientControlled()) {
				UpdateClientControlledValues();
				ClientMessageDispatch.Send(new SendClientEntityStateMessageHandler(this));
			}
		}

		public void Interpolate() {
			ClientEntitySnapshot snapshotBeforePoint = EntityController.SnapshotBeforeMoment;
			ClientEntitySnapshot snapshotAfterPoint = EntityController.SnapshotAfterMoment;

			if(snapshotBeforePoint != null && snapshotAfterPoint != null) {
				Dictionary<PropertyInfo, object> beforeProperties, afterProperties;

				snapshotBeforePoint.Properties.TryGetValue(this.Id, out beforeProperties);
				snapshotAfterPoint.Properties.TryGetValue(this.Id, out afterProperties);


				if(beforeProperties != null) {
					foreach(PropertyInfo property in beforeProperties.Keys) {
						EntityPropertyData propertyData = NetworkedProperties[property];
						if(!(IsClientControlled() && propertyData.HasPropertyType(EntityPropertyType.ClientControlledValue))) {
							if(propertyData.HasPropertyType(EntityPropertyType.Interpolatable)) {
								if(afterProperties != null) {
									object beforeValue, afterValue;
									beforeProperties.TryGetValue(property, out beforeValue);
									afterProperties.TryGetValue(property, out afterValue);
									if(afterValue != null) {
										double fractionBetweenPoints = Math.Min(1.0, ((double) (EntityController.InterpolationPoint - snapshotBeforePoint.Received) /
											(double) (snapshotAfterPoint.Received - snapshotBeforePoint.Received)));
										Console.WriteLine(beforeValue + "," + afterValue + "," + fractionBetweenPoints + "," + (EntityController.InterpolationPoint - snapshotBeforePoint.Received) + "," + (snapshotAfterPoint.Received - snapshotBeforePoint.Received));
										if(EntityInterpolation.HasInterpolator(property.PropertyType)) {
											property.SetValue(this, EntityInterpolation.Interpolate(
												property.PropertyType,
												beforeValue,
												afterValue,
												fractionBetweenPoints
											));
										} else {
											Console.WriteLine(property.PropertyType.FullName + " is not an interpolatable type.");
										}
									} else {
										property.SetValue(this, beforeValue);
									}
								}
							} else if(beforeProperties.ContainsKey(property)) {
								property.SetValue(this, beforeProperties[property]);
							}
						}
					}
				}
			}
		}

		public virtual void Draw() { }

		public virtual void OnClick(EventData data) { }

		public virtual void PackageForInitialTransfer(OutgoingMessage message) {
			message.Write(EntityController.GetEntityIdByName(GetName()));
			message.Write(Id);
		}

		public EntityPropertyData[] GetSyncableProperties() {
			return NetworkedPropertiesByType[EntityPropertyType.Syncable];
		}

		public EntityPropertyData[] GetInterpolatingProperties() {
			return NetworkedPropertiesByType[EntityPropertyType.Interpolatable];
		}

		public EntityPropertyData[] GetClientControlledValues() {
			return NetworkedPropertiesByType[EntityPropertyType.ClientControlledValue];
		}

		public static object[] UnpackageFromInitialTransfer(IncomingMessage message) {
			return new object[] { message.ReadInt32() };
		}
	}



	class InvalidEntityPropertyException : Exception {
		string Key;
		string Referencer;
		public InvalidEntityPropertyException(string key, string Referencer) {
			this.Key = key;
			this.Referencer = Referencer;
		}

		public override string ToString() {
			return Referencer + " attemped to set invalid entity property " + Key + ".";
		}
	}
}
