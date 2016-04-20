using PlumeAPI.Events;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities.Components {
	public class EntityComponent {
		public Action<EventData> Call;
		public BaseEntity Entity { get; set; }


		public EntityComponent(BaseEntity entity) {
			Entity = entity;
			Call = (x) => {
				if(ModuleController.Environment == PlumeEnvironment.Client) {
					CallClient(x);
				} else {
					CallServer(x);
				}
			};
		}

		public virtual void Setup() {}

		public virtual void CallClient(EventData eventData) { }
		public virtual void CallServer(EventData eventData) { }

		public virtual void PackageForInitialTransfer(OutgoingMessage message) { }
		public virtual void UnpackageFromInitialTransfer(IncomingMessage message) { }

		public virtual EntityComponent Clone(BaseEntity newEntity) {
			return new EntityComponent(newEntity);
		}

	}
}
