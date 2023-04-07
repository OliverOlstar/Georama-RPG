using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	namespace UI
	{
		public class WorldUIManager : MonoBehaviour
		{
			static WorldUIManager sSingleton = null;

			Dictionary<int, WorldUIBehaviour> mWorldUIElements = new Dictionary<int, WorldUIBehaviour>(100);

			public static WorldUIManager Get() { return sSingleton; }

			public static void CheckIn(WorldUIBehaviour element)
			{
				WorldUIManager worldUIManager = WorldUIManager.Get();
				if (worldUIManager == null)
				{
					return;
				}
				worldUIManager.mWorldUIElements.Add(element.GetRect().GetInstanceID(), element);
				element.GetRect().SetParent(worldUIManager.transform, true);
			}

			public static void CheckOut(WorldUIBehaviour element)
			{
				WorldUIManager worldUIManager = WorldUIManager.Get();
				if (worldUIManager == null)
				{
					return;
				}
				worldUIManager.mWorldUIElements.Remove(element.GetRect().GetInstanceID());
			}

			void Awake()
			{
				sSingleton = this;
			}

			bool Sort()
			{
				for (int i = 1; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					WorldUIBehaviour ui = mWorldUIElements[child.GetInstanceID()];
					int swapIndex = -1;
					for (int j = i - 1; j >= 0; j--)
					{
						Transform child2 = transform.GetChild(j);
						WorldUIBehaviour ui2 = mWorldUIElements[child2.GetInstanceID()];
						if (ui.GetSortPriority() > ui2.GetSortPriority())
						{
							swapIndex = j;
						}
						else
						{
							break;
						}
					}

					if (swapIndex >= 0)
					{
						//Debug.LogWarning("Moved " + child.GetInstanceID() + " up " + (i - swapIndex) + " spots");
						child.SetSiblingIndex(swapIndex);
						return true;
					}
				}
				return false;
			}

			void LateUpdate()
			{
				if (Camera.main == null)
				{
					return;
				}

				foreach (WorldUIBehaviour element in mWorldUIElements.Values)
				{
					element.ScaleToCamera();
				}

				while (Sort());
			}
		} 
	}
}
