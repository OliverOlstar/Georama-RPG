
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

namespace Core
{
	public static class EditorUtil
	{
		[MenuItem("Core/Search Scene By Layer")]
		static void SearchByLayer()
		{
			Dictionary<int, List<GameObject>> objectsByLayer = new Dictionary<int, List<GameObject>>();

			foreach (GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
			{
				if (!objectsByLayer.ContainsKey(gameObject.layer))
				{
					objectsByLayer.Add(gameObject.layer, new List<GameObject>());
				}
				objectsByLayer[gameObject.layer].Add(gameObject);
			}

			foreach (int layer in objectsByLayer.Keys)
			{
				string log = "layer: " + layer + "-" + LayerMask.LayerToName(layer);
				foreach (GameObject gameObject in objectsByLayer[layer])
				{
					log += "\n" + DebugUtil.GetScenePath(gameObject);
				}
				Debug.Log(log);
			}
		}

#if UNITY_EDITOR_WIN
		[MenuItem("Core/Time Manager/Pause &-")] // alt + -
#else
		[MenuItem("Core/Time Manager/Mac Pause _F1")] // alt + -
#endif
		static void EditorPause()
		{
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

#if UNITY_EDITOR_WIN
		[MenuItem("Core/Time Manager/Step &=")] // alt + =
#else
		[MenuItem("Core/Time Manager/Mac Step _F2")]
#endif
		static void EditorTimeStep()
		{
			EditorApplication.Step();
		}
		
#if UNITY_EDITOR_WIN
		[MenuItem("Core/Time Manager/Increment &]")]
#else
		[MenuItem("Core/Time Manager/Mac Increment _F4")]
#endif
		static void EditorTimeInc()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				GameObject obj = new GameObject("TimeScaleManager");
				tem = obj.AddComponent<TimeScaleManager>();
			}
			tem.EditorInc();
		}
		
#if UNITY_EDITOR_WIN
		[MenuItem("Core/Time Manager/Decrement &[")] // alt + [
#else
		[MenuItem("Core/Time Manager/Mac Decrement _F3")]
#endif
		static void EditorTimeDec()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				GameObject obj = new GameObject("TimeScaleManager");
				tem = obj.AddComponent<TimeScaleManager>();
			}
			tem.EditorDec();
		}

		//	This makes it easy to create, name and place unique new ScriptableObject asset files.
		public static T CreateAsset<T>() where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace(Path.GetFileName (AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
			
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
			
			return asset;
		}

		public delegate void ApplyOrRevert(GameObject _goCurrentGo, Object _ObjPrefabParent);
		[MenuItem ("Core/Selection/Apply all selected prefabs")]
		static void ApplyPrefabs()
		{
			SearchPrefabConnections(ApplyToSelectedPrefabs);
		}

		[MenuItem ("Core/Selection/Revert all selected prefabs")]
		static void ResetPrefabs()
		{
			SearchPrefabConnections(RevertToSelectedPrefabs);
		}

		//Look for connections
		static void SearchPrefabConnections(ApplyOrRevert _applyOrRevert)
		{
			GameObject[] tSelection = Selection.gameObjects;

			if (tSelection.Length > 0)
			{
				GameObject goPrefabRoot;
				GameObject goCur;
				bool bTopHierarchyFound;
				int iCount=0;
				PrefabInstanceStatus prefabType;
				bool bCanApply;
				//Iterate through all the selected gameobjects
				foreach(GameObject go in tSelection)
				{
					prefabType = PrefabUtility.GetPrefabInstanceStatus(go);
					//Is the selected gameobject a prefab?
					if(prefabType == PrefabInstanceStatus.Connected || prefabType == PrefabInstanceStatus.Disconnected)
					{
						//Prefab Root;
						goPrefabRoot = ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go)).transform.root.gameObject;
						goCur = go;
						bTopHierarchyFound = false;
						bCanApply = true;
						//We go up in the hierarchy to apply the root of the go to the prefab
						while(goCur.transform.parent != null && !bTopHierarchyFound)
						{  
							//Are we still in the same prefab?
							if(goPrefabRoot == ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(goCur.transform.parent.gameObject)).transform.root.gameObject)
							{
								goCur = goCur.transform.parent.gameObject;
							}
							else
							{
								//The gameobject parent is another prefab, we stop here
								bTopHierarchyFound = true;
								if(goPrefabRoot !=  ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(goCur)))
								{
									//Gameobject is part of another prefab
									bCanApply = false;
								}
							}
						}

						if(_applyOrRevert != null && bCanApply)
						{
							iCount++;
							_applyOrRevert(goCur, PrefabUtility.GetCorrespondingObjectFromSource(goCur));
						}
					}
				}
				Debug.Log(iCount + " prefab" + (iCount>1 ? "s" : "") + " updated");
			}
		}

		//Apply      
		static void ApplyToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			PrefabUtility.ReplacePrefab(_goCurrentGo, _ObjPrefabParent, ReplacePrefabOptions.ConnectToPrefab);
#pragma warning restore CS0618 // Type or member is obsolete
		}

		//Revert
		static void RevertToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent)
		{
			PrefabUtility.RevertPrefabInstance(_goCurrentGo, InteractionMode.UserAction);
		}
		
		[MenuItem("Core/Profiling/Find Materials")]
		static void CountMaterials()
		{
			Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
			List<Material> materials = new List<Material>(renderers.Length);
			List<int> counts = new List<int>(renderers.Length);
			foreach (Renderer r in renderers)
			{
				//Debug.Log(r.name);
				foreach (Material m in r.sharedMaterials)
				{
					if (m == null)
					{
						continue;
					}
					bool unique = true;
					for (int i = 0; i < materials.Count; i++)
					{
						if (m.GetInstanceID() == materials[i].GetInstanceID())
						{
							unique = false;
							counts[i]++;
							break;
						}
					}
					if (unique)
					{
						materials.Add(m);
						counts.Add(1);
					}
				}
			}
			
			for (int i = 0; i < materials.Count; i++)
			{
				Debug.Log(AssetDatabase.GetAssetPath(materials[i].GetInstanceID()) + " references: " + counts[i]);
			}
			Debug.Log("Found " + materials.Count + " unique materials.");
		}
		
		[MenuItem("Core/Profiling/Find Meshes")]
		static void CountMeshes2()
		{
			MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>();
			List<Mesh> meshes = new List<Mesh>(filters.Length);
			foreach (MeshFilter f in filters)
			{
				//Debug.Log(r.name);
				if (f.sharedMesh == null)
				{
					continue;
				}
				bool unique = true;
				for (int i = 0; i < meshes.Count; i++)
				{
					if (f.sharedMesh.GetInstanceID() == meshes[i].GetInstanceID())
					{
						unique = false;
						break;
					}
				}
				if (unique)
				{
					meshes.Add(f.sharedMesh);
				}
			}
			
			meshes = meshes.OrderBy(o=>o.triangles.Length).ToList();
			for (int i = 0; i < meshes.Count; i++)
			{
				Debug.Log(AssetDatabase.GetAssetPath(meshes[i].GetInstanceID()) + " " + meshes[i].name + " tris: " + (meshes[i].triangles.Length / 3));
			}
			Debug.Log("Found " + meshes.Count + " unique meshes.");
		}
		
		[MenuItem("Core/Profiling/Triangle Counts Per Mesh")]
		static void CountMeshes()
		{
			MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>();
			List<Mesh> meshes = new List<Mesh>(filters.Length);
			List<int> counts = new List<int>(filters.Length);
			foreach (MeshFilter f in filters)
			{
				//Debug.Log(r.name);
				if (f.sharedMesh == null)
				{
					continue;
				}
				bool unique = true;
				for (int i = 0; i < meshes.Count; i++)
				{
					if (f.sharedMesh.GetInstanceID() == meshes[i].GetInstanceID())
					{
						unique = false;
						counts[i]++;
						break;
					}
				}
				if (unique)
				{
					meshes.Add(f.sharedMesh);
					counts.Add(1);
				}
			}
			
			int average = 0;
			for (int i = 0; i < meshes.Count; i++)
			{
				average += counts[i] * meshes[i].triangles.Length;
			}
			average /= counts.Count;
			
			for (int i = 0; i < meshes.Count; i++)
			{
				int tris = counts[i] * meshes[i].triangles.Length;
				if (tris < average)
				{
					continue;
				}
				Debug.Log(
					AssetDatabase.GetAssetPath(meshes[i].GetInstanceID()) + " " + 
					meshes[i].name + " placed: " + 
					counts[i] + " total tris: " +
					(tris / 3));
			}
			Debug.Log("Found " + meshes.Count + " unique meshes. Average total tris: " + (average / 3));
		}

		public static ulong EstimateTextureSize(Texture2D tex)
		{
			return EstimateTextureSize(tex, AssetDatabase.GetAssetPath(tex));
		}
		public static ulong EstimateTextureSize(string path)
		{
			Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			if (tex == null)
			{
				return 0;
			}
			return EstimateTextureSize(tex, path);
		}
		public static ulong EstimateTextureSize(Texture2D tex, string path)
		{
			int pixels = Mathf.RoundToInt(tex.width * tex.height);
			TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			bool squaredPOT = tex.width == tex.height && Mathf.IsPowerOfTwo(tex.width);
			bool atlassed = !string.IsNullOrEmpty(texImporter.spritePackingTag);
			bool compressed = (atlassed || squaredPOT) &&
				texImporter.textureCompression != TextureImporterCompression.Uncompressed;
			bool hasAlpha = texImporter.DoesSourceTextureHaveAlpha() &&
				texImporter.alphaSource == TextureImporterAlphaSource.FromInput;
			int bitsPerPixel = compressed ?
				(hasAlpha ? 8 : 4) :
				(hasAlpha ? 32 : 24);
			ulong size = (ulong)(pixels * bitsPerPixel) / 8;
			return size;
		}
	}
}
