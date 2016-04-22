using PlumeAPI.Events;
using PlumeAPI.Modularization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities.Components {
	public class EntityComponent {
		public BaseEntity Entity { get; set; }

		public EntityComponent(BaseEntity entity) {
			Entity = entity;
		}

		public virtual void Setup() {}

		public virtual void Call(EventData eventData) { }
	

		public virtual EntityComponent Clone(BaseEntity newEntity) {
			return new EntityComponent(newEntity);
		}
	}

}
