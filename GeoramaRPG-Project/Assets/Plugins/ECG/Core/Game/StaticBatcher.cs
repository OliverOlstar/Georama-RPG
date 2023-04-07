using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	public static class StaticBatcher
	{
		public static void SetUpStaticBatching2(GameObject gameObject)
		{
			MeshRenderer[] renderers = RecursiveBatchingSearch2(gameObject.transform);

			// Assume disabled objects can just be deleted
			foreach (MeshRenderer renderer in renderers)
			{
				if (renderer == null)
				{
					continue;
				}
				if (!renderer.gameObject.activeInHierarchy)
				{
//					Debug.LogWarning("GameUtil.SetUpStaticBatching2() Destroying disabled object "
//								   + DebugUtil.GetScenePath(renderer.gameObject));
					UnityEngine.Object.DestroyImmediate(renderer.gameObject);
				}
				else if (!renderer.enabled)
				{
//					Debug.LogWarning("GameUtil.SetUpStaticBatching2() Destroying disabled renderer "
//								   + DebugUtil.GetScenePath(renderer.gameObject));
					UnityEngine.Object.DestroyImmediate(renderer);
				}
			}

			int count = 0;
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] == null)
				{
					continue;
				}
				Material batchMat = renderers[i].sharedMaterial;
				if (batchMat == null)
				{
					continue;
				}
				List<GameObject> batch = new List<GameObject>(renderers.Length - i);
				for (int j = i; j < renderers.Length; j++)
				{
					if (renderers[j] == null)
					{
						continue;
					}

					bool found = false;
					foreach (Material mat in renderers[j].sharedMaterials)
					{
						if (mat != null && mat.GetInstanceID() == batchMat.GetInstanceID())
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						continue;
					}
					batch.Add(renderers[j].gameObject);
					renderers[j] = null;
				}
				count++;
				//Debug.Log("Batch " + count + " " + batchMat.name + " " + batch.Count);
				StaticBatchingUtility.Combine(batch.ToArray(), gameObject);
				if (batch.Count > 0)
				{
					MeshFilter meshFilter = batch[0].GetComponentInChildren<MeshFilter>();
					if (meshFilter != null && meshFilter.sharedMesh != null)
					{
						meshFilter.sharedMesh.name = "Batch_" + count + "_" + batchMat.name;
					}
				}
			}
			//Debug.Log(count + " batches");
		}

		public static MeshRenderer[] RecursiveBatchingSearch2(Transform t)
		{
			// CHECK THIS TRANSFORM'S NAME
			if (t.name.ToLower().Contains("dynamic"))
			{
				return new MeshRenderer[] { };
			}

			// This transform's name allows batching - add stuff to the result list
			List<MeshRenderer> result = new List<MeshRenderer>();

			// ADD RENDERER, IF APPLICABLE
			MeshRenderer renderer = t.GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				result.Add(renderer);
			}

			// DO RECURSION ON ANY CHILDREN
			if (t.childCount > 0)
			{
				foreach (Transform child in t)
				{
					result.AddRange(RecursiveBatchingSearch2(child));
				}
			}

			return result.ToArray();
		}
	}
}
