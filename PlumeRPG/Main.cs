using PlumeAPI.Modularization;
using System.Diagnostics;
using PlumeAPI.Networking;
using PlumeRPG.Entities;
using PlumeAPI.Entities;
using PlumeAPI.World;
using PlumeAPI.Commands.Builtin;

namespace PlumeRPG {
	class Main : PlumeModule {
		public Main() {
			Debug.WriteLine("PlumeCore launched");
		}

		public override void Register() {
			base.Register();
			MovementCommands.Register();
		}

		public override void AfterLoad() {
			Debug.WriteLine("PlumeCore loaded!");
		}

		public override void UserFullyLoaded(Client user) {
			Player player = (Player) ModuleController.CreateEntityByReferencer("PlumeRPG.Entities.Player", user);
			ScopeController.RegisterEntity(user.Scope, player);
			user["PlayerEntity"] = player;
		}
	}
}