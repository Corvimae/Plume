using PlumeAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities.Components {
	public class EventTriggeredComponent : EntityComponent {
		public string Event { get; set; } = "";
		public int Priority { get; set; } = 0;
		public EventRegistration Registration { get; set; } = null;

		public EventTriggeredComponent(string evt, int priority, BaseEntity entity) : base(entity) {
			Event = evt;
			Priority = priority;
			if(!entity.Prototypal) {
				Registration = EventController.CallOnEvent(Event, priority, Call);
			}
		}
		public override EntityComponent Clone(BaseEntity newEntity) {
			return new EventTriggeredComponent(Event, Priority, newEntity);
		}
	}
}
