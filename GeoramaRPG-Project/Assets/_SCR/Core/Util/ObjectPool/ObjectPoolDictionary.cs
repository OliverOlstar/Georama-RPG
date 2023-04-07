using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
	public class ObjectPoolDictionary : MonoBehaviour
	{
		[System.Serializable]
		public class PoolValues
		{
			[AssetsOnly]
			public GameObject prefab = null;
			public string key => prefab.name;

			[Tooltip("What returns when all items are already checked out")]
			public PoolReturnType returnType = PoolReturnType.Expand;
			[HideInPlayMode]
			public int startingCopies = 5;
		}

		private class Pool
		{
			private PoolValues values = null;
			private List<PoolItem> itemsIn = new List<PoolItem>();
			private List<PoolItem> itemsOut = new List<PoolItem>();

			private Transform transform = null;

			public Pool(PoolValues pValues, Transform pTransform)
			{
				values = pValues;
				transform = pTransform;

				while (itemsIn.Count < values.startingCopies)
				{
					itemsIn.Add(InstiateNewObject());
				}
			}

			private PoolItem InstiateNewObject()
			{
				PoolItem item = new PoolItem();

				item.gameObject = Instantiate(values.prefab, transform);
				item.parent = transform;

				item.element = item.gameObject.GetComponentInChildren<PoolElement>();
				if (item.element != null)
				{
					item.gameObject = item.element.gameObject;
					item.element.Init(values.prefab.name, item.parent);
					item.gameObject.name = values.prefab.name;
				}

				item.gameObject.SetActive(false);
				return item;
			}

			public GameObject CheckOutObject(bool pEnable = false)
			{
				PoolItem item;
				// Grab Next
				if (itemsIn.Count > 0)
				{
					item = itemsIn[0];
					itemsOut.Add(item);
					itemsIn.RemoveAt(0);
				}
				// Expand
				else if (values.returnType == PoolReturnType.Expand)
				{
					item = InstiateNewObject();
				}
				// Grab first out
				else if (values.returnType == PoolReturnType.Loop)
				{
					if (itemsOut.Count > 0)
					{
						// Return first to pool, then take it back out
						item = itemsOut[0];
						if (item.element != null)
							item.element.ReturnToPool();
						itemsIn.Remove(item);
					}
					else
					{
						// If none out create new
						item = InstiateNewObject();
					}
					itemsOut.Add(item);
				}
				else // (returnType == PoolReturnType.Null)
				{
					return null;
				}

				item.gameObject.SetActive(pEnable);
				if (item.element != null)
					item.element.OnExitPool();
				return item.gameObject;
			}

			public void CheckInObject(PoolElement pElement, bool pDisable = true)
			{
				pElement.gameObject.transform.SetParent(pElement.parent, true);
				pElement.gameObject.transform.localScale = Util.Inverse(pElement.parent.lossyScale);
				if (pElement.gameObject.activeSelf)
					pElement.gameObject.SetActive(!pDisable);

				PoolItem item = new PoolItem();
				item.gameObject = pElement.gameObject;
				item.element = pElement;
				item.parent = pElement.parent;

				itemsOut.Remove(item);
				itemsIn.Add(item);
			}

			public void ObjectDestroyed(GameObject pObject, PoolElement pElement)
			{
				PoolItem item = new PoolItem();
				item.gameObject = pObject;
				item.element = pElement;

				if (!itemsIn.Remove(item))
					itemsOut.Remove(item);
			}

			public void OnDestroy()
			{

			}
		}

		public struct PoolItem
		{
			public Transform parent;
			public GameObject gameObject;
			public PoolElement element;
		}

		public enum PoolReturnType
		{
			Expand,
			Loop,
			Null
		}

		#region Singleton
		public static ObjectPoolDictionary _Instance = null;
		public static ObjectPoolDictionary Instance
		{
			get
			{
				if (_Instance != null)
				{
					return _Instance;
				}
				GameObject gameObject = new GameObject("ObjectPoolDictionary Container");
				_Instance = gameObject.AddComponent<ObjectPoolDictionary>();
				return _Instance;
			}
		}

		#endregion

		private static Dictionary<string, Pool> dictionary = new Dictionary<string, Pool>();

		public static GameObject Get(GameObject pObject)
		{
			if (pObject == null)
			{
				return null;
			}
			if (dictionary.ContainsKey(pObject.name) == false)
			{
				PoolValues values = new PoolValues();
				values.prefab = pObject;
				return Instance.CreatePool(values).CheckOutObject();
			}
			return dictionary[pObject.name].CheckOutObject();
		}
		public static GameObject Get(PoolElement pElement)
		{
			if (pElement == null)
			{
				return null;
			}
			if (!dictionary.ContainsKey(pElement.PoolKey))
			{
				PoolValues values = new PoolValues();
				values.prefab = pElement.gameObject;
				return Instance.CreatePool(values).CheckOutObject();
			}
			return dictionary[pElement.PoolKey].CheckOutObject();
		}
		public static GameObject Get(PoolValues pValues)
		{
			if (pValues.prefab == null)
			{
				return null;
			}
			if (dictionary.ContainsKey(pValues.key) == false)
			{
				return Instance.CreatePool(pValues).CheckOutObject();
			}
			return dictionary[pValues.key].CheckOutObject();
		}
		
		public static GameObject Play(GameObject pObject, Vector3 pPosition, Quaternion pRotation, Transform pParent = null) => InternalPlay(Get(pObject), pPosition, pRotation, pParent);
		public static GameObject Play(PoolElement pElement, Vector3 pPosition, Quaternion pRotation, Transform pParent = null) => InternalPlay(Get(pElement), pPosition, pRotation, pParent);
		public static GameObject Play(PoolValues pValues, Vector3 pPosition, Quaternion pRotation, Transform pParent = null) => InternalPlay(Get(pValues), pPosition, pRotation, pParent);
		private static GameObject InternalPlay(GameObject pPoolObject, Vector3 pPosition, Quaternion pRotation, Transform pParent = null)
		{
			pPoolObject.transform.SetParent(pParent);
			pPoolObject.transform.position = pPosition;
			pPoolObject.transform.rotation = pRotation;
			pPoolObject.SetActive(true);
			return pPoolObject;
		}

		public static void Return(PoolElement pElement, bool pDisable = true)
		{
			dictionary[pElement.PoolKey].CheckInObject(pElement, pDisable);
		}

		public static void ObjectDestroyed(GameObject pObject, PoolElement pElement)
		{
			if (dictionary.ContainsKey(pObject.name))
			{
				dictionary[pObject.name].ObjectDestroyed(pObject, pElement);
			}
		}

		private Pool CreatePool(PoolValues pValues)
		{
			Transform poolT = new GameObject("Pool " + pValues.key).transform;
			poolT.SetParent(transform);
			Pool pool = new Pool(pValues, poolT);

			dictionary.Add(pValues.key, pool);
			return pool;
		}

		private void OnDestroy()
		{
			foreach (Pool p in dictionary.Values)
			{
				p.OnDestroy();
			}
			dictionary.Clear();
		}
	}
}