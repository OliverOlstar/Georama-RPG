using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	public class PoolElement : MonoBehaviour
	{
		private string poolKey = string.Empty;
		public string PoolKey => string.IsNullOrEmpty(poolKey) ? gameObject.name : poolKey;
		public Transform parent { get; private set; } = null;

		public virtual void Init(string pPoolKey, Transform pParent)
		{
			poolKey = pPoolKey;
			parent = pParent;
		}

		public virtual void ReturnToPool()
		{
			ObjectPoolDictionary.Return(this);
		}

		public virtual void OnExitPool()
		{

		}

		private void OnDestroy()
		{
			ObjectPoolDictionary.ObjectDestroyed(gameObject, this);
		}
	}
}