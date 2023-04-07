using System;
using System.Collections.Generic;
using OliverLoescher;

namespace Core
{
	public class CollectionPool<TCollection, T> where TCollection : class, ICollection<T>, new()
	{
		private static Stack<TCollection> s_Pool;
		private static HashSet<TCollection> s_Set;

		static CollectionPool()
		{
			s_Pool = new Stack<TCollection>();
			s_Set = new HashSet<TCollection>();
		}

		public static TCollection Request()
		{
			if (s_Pool.Count > 0)
			{
				TCollection collection = s_Pool.Pop();
				s_Set.Remove(collection);
				if (collection.Count > 0)
				{
					Util.DevException($"CollectionPool.Request() {typeof(TCollection).Name} {collection.GetHashCode()} is not empty, " +
						$"this means someone has been modifying the collection after calling Return()");
					collection.Clear();
				}
				//UnityEngine.Debug.Log($"{typeof(TCollection).Name}.Request() Pooled " + collection.GetHashCode());
				return collection;
			}
			else
			{
				TCollection collection = new TCollection();
				//UnityEngine.Debug.Log($"{typeof(TCollection).Name}.Request() New " + collection.GetHashCode());
				return collection;
			}
		}

		public static TCollection Request(TCollection copyCollection)
		{
			TCollection collection = Request();
			foreach (T value in copyCollection)
			{
				collection.Add(value);
			}
			return collection;
		}

		public static void Return(TCollection collection)
		{
			collection = collection ?? throw new ArgumentNullException("collection");
			collection.Clear();
			//UnityEngine.Debug.Log($"{typeof(TCollection).Name}.Return() " + collection.GetHashCode());
			if (s_Set.Contains(collection))
			{
				Util.DevException($"CollectionPool.Return() {typeof(TCollection).Name} {collection.GetHashCode()} has already been returned. " +
					"Requests and Returns must be 1 to 1 or pool instances can get leaked");
				return;
			}
			s_Pool.Push(collection);
			s_Set.Add(collection);
		}

		protected CollectionPool()
		{
		}
	}
}
