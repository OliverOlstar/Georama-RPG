
#if UNITY_EDITOR // This should really be in an editor folder but I need to use it for data validation
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Core
{
	public static class AssetDatabaseUtil
	{
		// Doesn't make sense to throw duplicate name warnings for these types, we know there will be dupes
		//private static readonly HashSet<System.Type> IGNORE_WARN_FOR_TYPES = new HashSet<System.Type>() { typeof(GameObject) };

		private class Cache
		{
			private System.Type m_Type = default;

			private string[] m_Paths = null;
			public string[] Paths => m_Paths;

			private bool m_Nested = false;

			private Dictionary<string, string> m_Names = null;
			private Dictionary<string, UnityEngine.Object> m_Assets = null;

			public Cache(System.Type searchType, bool canBeNested = true)
			{
				m_Type = searchType;
				string search = "t:" + m_Type.Name;
				string[] guids = AssetDatabase.FindAssets(search);

				// AssetDatabase.FindAssets behaves different for nested assets in 2020
				// In 2019 it would return the same GUID of the root asset for each nested asset
				// as of 2020 it now returns the root GUID only once
#if UNITY_2020_1_OR_NEWER
				m_Paths = new string[guids.Length];
				for (int i = 0; i < guids.Length; i++)
				{
					string guid = guids[i];
					string path = AssetDatabase.GUIDToAssetPath(guid);
					if (!m_Nested && canBeNested)
					{
						// If the main type at this path does not match assume this must be a nested asset
						System.Type mainType = AssetDatabase.GetMainAssetTypeAtPath(path);
						if (!mainType.IsAssignableFrom(searchType))
						{
							m_Nested = true;
						}
					}
					m_Paths[i] = path;
				}
#else
				// Note: Nested assets will return the GUID for the root asset many times
				HashSet<string> uniqueGuids = new HashSet<string>();
				foreach (string guid in guids)
				{
					if (!uniqueGuids.Contains(guid))
					{
						uniqueGuids.Add(guid);
					}
					else
					{
						m_Nested = true; // At lease one asset is nested
					}
				}
				m_Paths = new string[uniqueGuids.Count];
				int i = 0;
				foreach (string guid in uniqueGuids)
				{
					m_Paths[i] = AssetDatabase.GUIDToAssetPath(guid);
					i++;
				}
#endif
			}

			private void Init()
			{
				if (m_Assets != null)
				{
					return;
				}
				m_Names = new Dictionary<string, string>(m_Paths.Length);
				m_Assets = new Dictionary<string, Object>(m_Paths.Length);
				if (m_Nested)
				{
					foreach (string path in m_Paths)
					{
						// If some of the assets of this type are nested we need to do something more complicated
						UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
						foreach (UnityEngine.Object asset in assets)
						{
							if (asset == null)
							{
								continue; // This can happen in some cases I don't really know how
							}
							if (!m_Type.IsAssignableFrom(asset.GetType()))
							{
								continue;
							}
							if (AddName(asset.name, path))
							{
								m_Assets.Add(path + asset.name, asset);
							}
						}
					}
				}
				else
				{
					foreach (string path in m_Paths)
					{
						string name = Path.GetFileNameWithoutExtension(path);
						AddName(name, path);
					}
				}
			}

			private bool AddName(string name, string path)
			{
				if (m_Names.ContainsKey(name))
				{
					//if (!IGNORE_WARN_FOR_TYPES.Contains(m_Type.GetType()))
					//{
					//	Debug.LogWarning("Core.AssetDatabaseUtil.Cache.AddAsset() " + m_Type.Name + " has more than one asset with the same name " +
					//		name + " this could cause unexpected search results");
					//}
					return false;
				}
				if (name.EndsWith(" "))
				{
					Debug.LogWarning("Core.AssetDatabaseUtil.Cache.AddAsset() '" + name + "' ends with a space, this will probably cause problems");
				}
				m_Names.Add(name, path);
				return true;
			}

			public bool TryGet(string name, out string path)
			{
				Init();
				return m_Names.TryGetValue(name, out path);
			}

			public bool TryLoad(string name, out UnityEngine.Object asset)
			{
				Init();
				if (!TryGet(name, out string path))
				{
					asset = null;
					return false;
				}
				if (m_Assets.TryGetValue(path + name, out asset))
				{
					return asset != null;
				}
				if (m_Nested)
				{
					return false; // For nested assets everything has been loaded up front
				}
				// Non nested assets are lazy loaded
				asset = AssetDatabase.LoadAssetAtPath(path, m_Type);
				m_Assets.Add(path + asset.name, asset);
				return asset != null;
			}

			public void LoadAll<T>(List<T> assets) where T : UnityEngine.Object
			{
				Init();
				foreach (string name in m_Names.Keys)
				{
					if (TryLoad(name, out UnityEngine.Object obj) && obj is T asset)
					{
						assets.Add(asset);
					}
				}
			}
		}

		// Need to clear the cache if something has changed
		private class Postprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(
				string[] importedAssets,
				string[] deletedAssets,
				string[] movedAssets,
				string[] movedFromAssetPaths)
			{
				// TODO: Saving a sheet causes results to be cleared, that sucks. 
				// Could we detect the difference between a new asset and a reimported asset?
				s_SearchResults.Clear();
			}
		}

		private static Dictionary<System.Type, Cache> s_SearchResults = new Dictionary<System.Type, Cache>();

		private static Cache FindInternal(System.Type assetType, bool canBeNested = true)
		{
			if (!assetType.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				Debug.LogWarning("Core.AssetDatabaseUtil.FindInternal() " + assetType.Name + " should be a subclass of UnityEngine.Object");
			}
			if (!s_SearchResults.TryGetValue(assetType, out Cache results))
			{
				results = new Cache(assetType,canBeNested);
				s_SearchResults.Add(assetType, results);
			}
			return results;
		}

		public static string[] Find(System.Type assetType, bool canBeNested = true)
		{
			return FindInternal(assetType, canBeNested).Paths;
		}

		public static string Find(string assetName, System.Type assetType)
		{
			FindInternal(assetType).TryGet(assetName, out string path);
			return path;
		}

		public static bool Exists(string assetName, System.Type assetType)
		{
			return FindInternal(assetType).TryGet(assetName, out _);
		}

		public static bool Exists<T>(string assetName) where T : UnityEngine.Object
		{
			return FindInternal(typeof(T)).TryGet(assetName, out _);
		}

		public static T Load<T>(string assetName) where T : UnityEngine.Object
		{
			FindInternal(typeof(T)).TryLoad(assetName, out UnityEngine.Object asset);
			return asset as T;
		}
		public static T LoadAtPath<T>(string path) where T : UnityEngine.Object
		{
			return Load<T>(Path.GetFileNameWithoutExtension(path));
		}
		public static void LoadAll<T>(List<T> assets) where T : UnityEngine.Object
		{
			FindInternal(typeof(T)).LoadAll<T>(assets);
		}
		public static void LoadAll(System.Type type, List<UnityEngine.Object> assets)
		{
			FindInternal(type).LoadAll<UnityEngine.Object>(assets);
		}

		public static UnityEngine.Object Load(string assetName, System.Type assetType)
		{
			FindInternal(assetType).TryLoad(assetName, out UnityEngine.Object asset);
			return asset;
		}
		public static UnityEngine.Object LoadAtPath(string path, System.Type assetType)
		{
			return Load(Path.GetFileNameWithoutExtension(path), assetType);
		}
	}
}
#endif
