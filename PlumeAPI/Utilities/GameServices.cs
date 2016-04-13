using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PlumeAPI.Utilities {
	public static class GameServices {
		private static GameServiceContainer container;

		private static Stopwatch gameTimer = new Stopwatch();

		public static void StartTimer() {
			gameTimer.Start();
		}

		public static long TimeElapsed() {
			return gameTimer.ElapsedMilliseconds;
		}

		public static GameServiceContainer Instance {
			get {
				if (container == null) {
					container = new GameServiceContainer();
				}
				return container;
			}
		}

		public static T GetService<T>() {
			return (T)Instance.GetService(typeof(T));
		}

		public static void AddService<T>(T service) {
			Instance.AddService(typeof(T), service);
		}

		public static void RemoveService<T>() {
			Instance.RemoveService(typeof(T));
		}
	}
}
