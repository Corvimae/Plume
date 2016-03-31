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
using PlumeAPI.Attributes;
using PlumeAPI.World;
using PlumeAPI.Networking;

namespace PlumeAPI.Entities {
	public class BaseEntity : CoreObject, IDrawableEntity {

		[Syncable]
		public virtual Vector2 Position { get; set; }
		protected Vector2 DrawDimensions;

		public EntityScope Scope { get; set; }

		protected string Identifier;

		public Dictionary<int, Action> DrawActionRegistry = new Dictionary<int, Action>();

		public static Dictionary<string, bool> EntityProperties = new Dictionary<string, bool> {
			{ "draw", false },
			{ "update", false },
			{ "click", false }
		};

		public int DrawLayer = 0;

		IEnumerable<PropertyInfo> SyncableProperties;
		IEnumerable<PropertyInfo> InterpolatingProperties;

		public BaseEntity() {
			SyncableProperties = GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).
					Where(p => Attribute.IsDefined(p, typeof(SyncableAttribute))).OrderBy(p => p.Name);
			InterpolatingProperties = GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).
							Where(p => Attribute.IsDefined(p, typeof(InterpolateAttribute))).OrderBy(p => p.Name);
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
		protected void UpdateSyncableProperties() {
			ClientEntitySnapshot snapshotBeforePoint = EntityController.SnapshotBeforeMoment;
			ClientEntitySnapshot snapshotAfterPoint = EntityController.SnapshotAfterMoment;

			if(snapshotBeforePoint != null && snapshotAfterPoint != null) {
			Dictionary<PropertyInfo, object> beforeProperties, afterProperties;

				snapshotBeforePoint.Properties.TryGetValue(this.Id, out beforeProperties);
				snapshotAfterPoint.Properties.TryGetValue(this.Id, out afterProperties);

				if(beforeProperties != null) {
					foreach(PropertyInfo property in beforeProperties.Keys) {
						if(InterpolatingProperties.Contains(property)) {
							if(afterProperties != null) {
								object beforeValue, afterValue;
								beforeProperties.TryGetValue(property, out beforeValue);
								afterProperties.TryGetValue(property, out afterValue);
								if(afterValue != null) {
									double fractionBetweenPoints = ((EntityController.InterpolationPoint - snapshotBeforePoint.Received).TotalMilliseconds /
										(snapshotAfterPoint.Received - snapshotBeforePoint.Received).TotalMilliseconds) - 1;
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

		/*protected void Interpolate() {
			foreach(PropertyInfo property in GetInterpolatingProperties()) {
				//Find the previous two timestamps
				if(Snapshots.Count() >= 2) {
					IEnumerable<ClientEntitySnapshot> reversedSnapshots = Snapshots.Reverse<ClientEntitySnapshot>();
					IEnumerable<ClientEntitySnapshot> relevantSnapshots = reversedSnapshots.Where(x => x.Properties.Keys.Contains(property));
					ClientEntitySnapshot mostRecent = relevantSnapshots.FirstOrDefault();
					ClientEntitySnapshot secondMostRecent = relevantSnapshots.Skip(1).FirstOrDefault();
					DateTime now = DateTime.UtcNow;
					if(reversedSnapshots.First().Properties.ContainsKey(property)) {
						if(mostRecent != null && secondMostRecent != null) {
							double timeBetweenUpdates = (mostRecent.Received - secondMostRecent.Received).TotalMilliseconds;
							if(timeBetweenUpdates < Configuration.InterpolationLimit) {
								double timeSinceLastUpdate = (now - mostRecent.Received).TotalMilliseconds;
								if(EntityInterpolation.HasInterpolator(property.PropertyType)) {
									property.SetValue(this, EntityInterpolation.Interpolate(
										property.PropertyType,
										mostRecent.Properties[property],
										secondMostRecent.Properties[property],
										timeBetweenUpdates,
										timeSinceLastUpdate
									));
								} else {
									Console.WriteLine(property.PropertyType.FullName + " is not an interpolatable type.");
								}
							} else {
								property.SetValue(this, mostRecent.Properties[property]);
							}
						}
					} else {
						property.SetValue(this, mostRecent.Properties[property]);
					}
				}
			}
		}*/

		public virtual void Draw() { }

		public virtual void OnClick(EventData data) { }

		public virtual void PackageForInitialTransfer(OutgoingMessage message) {
			message.Write(EntityController.GetEntityIdByName(GetName()));
			message.Write(Id);
		}

		public IEnumerable<PropertyInfo> GetSyncableProperties() {
			return SyncableProperties;
		}

		public IEnumerable<PropertyInfo> GetInterpolatingProperties() {
			return InterpolatingProperties;
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
