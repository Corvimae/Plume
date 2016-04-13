using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities.Interfaces {
	public interface IRegisterableEntity {
		void Register();
		void RegisterClient();
		void RegisterServer();
	}
}
