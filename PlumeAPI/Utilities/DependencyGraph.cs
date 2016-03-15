using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PlumeAPI.Utilities {
	class DependencyGraph<T> {
		List<DependencyNode<T>> Nodes = new List<DependencyNode<T>>();

		public DependencyGraph() { }

		public DependencyNode<T> AddNode(T item) {
			if(!HasNode(item)) {
				DependencyNode<T> node = new DependencyNode<T>(item);
				Nodes.Add(node);
				return node;
			} else {
				return Nodes.First(t => t.Item.Equals(item));
			}
		}

		public DependencyNode<T> AddNode(T item, Func<T, T, bool> equalityMethod) {
			if(!HasNode(item, equalityMethod)) {
				DependencyNode<T> node = new DependencyNode<T>(item);
				Nodes.Add(node);
				return node;
			} else {
				return Nodes.First(t => equalityMethod.Invoke(t.Item, item));
			}
		}

		public void RemoveNode(DependencyNode<T> node) {
			if(HasNode(node)) {
				foreach(DependencyNode<T> existingNode in Nodes) {
					existingNode.IncomingEdges.RemoveAll(t => t == node);
					existingNode.OutgoingEdges.RemoveAll(t => t == node);
				}
				Nodes.Remove(node);
			}
		}

		public bool HasNode(DependencyNode<T> node) {
			return Nodes.Contains(node);
		}

		public bool HasNode(T item) {
			return Nodes.Any(t => t.Item.Equals(item));
		}

		public bool HasNode(T item, Func<T, T, bool> lambda) {
			return Nodes.Any(t => lambda.Invoke(t.Item, item));
		}

		public void AddDependency(DependencyNode<T> parent, DependencyNode<T> child) {
			parent.OutgoingEdges.Add(child);
			child.IncomingEdges.Add(parent);
		}

		public void RemoveDependency(DependencyNode<T> parent, DependencyNode<T> child) {
			parent.OutgoingEdges.Remove(child);
			parent.IncomingEdges.Remove(parent);
		}


		private void VisitNode(DependencyNode<T> node, ref List<DependencyNode<T>> temporarilyMarked, ref List<DependencyNode<T>> permanentlyMarked, ref List<DependencyNode<T>> order) {
			if(temporarilyMarked.Contains(node)) {
				throw new CircularDependencyException<T>(node);
			} else {
				temporarilyMarked.Add(node);
				foreach(DependencyNode<T> child in node.OutgoingEdges) {
					if(!permanentlyMarked.Contains(child)) {
						VisitNode(child, ref temporarilyMarked, ref permanentlyMarked, ref order);
					}
				}
				permanentlyMarked.Add(node);
				temporarilyMarked.Remove(node);
				order.Insert(0, node);
			}
		}

		public List<DependencyNode<T>> GetProcessingOrder() {
			List<DependencyNode<T>> sortedList = new List<DependencyNode<T>>();
			//Select all starting nodes (those that have no dependencies)
			List<DependencyNode<T>> temporarilyMarked = new List<DependencyNode<T>>();
			List<DependencyNode<T>> permanentlyMarked = new List<DependencyNode<T>>();
			while(permanentlyMarked.Count() < Nodes.Count()) {
				DependencyNode<T> arbitraryUnmarkedNode = Nodes.Except(permanentlyMarked).First();
				VisitNode(arbitraryUnmarkedNode, ref temporarilyMarked, ref permanentlyMarked, ref sortedList);
			}
			return sortedList;
		}
	}

	class DependencyNode<T> {
		public T Item;
		public List<DependencyNode<T>> IncomingEdges = new List<DependencyNode<T>>();
		public List<DependencyNode<T>> OutgoingEdges = new List<DependencyNode<T>>();
		public DependencyNode(T item) {
			Item = item;
		}

		public bool DirectDepedantOf(DependencyNode<T> node) {
			return IncomingEdges.Contains(node);
		}

		public bool DirectDependencyOf(DependencyNode<T> node) {
			return OutgoingEdges.Contains(node);
		}

		public static bool operator ==(DependencyNode<T> a, DependencyNode<T> b) {
			return a.Item.Equals(b.Item);
		}
		public static bool operator !=(DependencyNode<T> a, DependencyNode<T> b) {
			return !a.Item.Equals(b.Item);
		}

		public override bool Equals(object o) {
			return this.Item.Equals(((DependencyNode<T>) o).Item);
		}

		public override int GetHashCode() {
			return Item.GetHashCode();
		}
		public override string ToString() {
			return Item.ToString();
		}
	}

	class CircularDependencyException<T> : Exception {
		public DependencyNode<T> Node;
		public CircularDependencyException(DependencyNode<T> node) {
			Node = node;
		}

		public override string ToString() {
			return "Circular dependency on node " + Node.ToString();
		}
	}

	class AllNodesDependentException : Exception { }
}
