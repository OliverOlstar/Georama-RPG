using OliverLoescher;
using UnityEngine;

namespace OliverLoescher
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>, new()
    {
        private static T _Instance = null;
        private static ISingleton _InstanceInterface = null;
        public static T Instance
        {
            get
            {
				// Create
				TryCreate();

				// Access
				if (_InstanceInterface != null)
                {
                    _InstanceInterface.OnAccessed();
                }
                return _Instance;
            }
        }

		protected static void TryCreate()
		{
			if (!Util.IsApplicationQuitting && _Instance == null)
			{
				_Instance = new GameObject().AddComponent<T>();
				_Instance.gameObject.name = _Instance.GetType().Name;
				DontDestroyOnLoad(_Instance.gameObject);
				_InstanceInterface = (_Instance is ISingleton i) ? i : null;
			}
		}

		private static GameObject LogContext => _Instance != null ? _Instance.gameObject : null;
        protected static void Log(string pMessage) => Debug.Log($"[{typeof(T).Name}] {pMessage}", LogContext);
        protected static void LogWarning(string pMessage) => Debug.LogWarning($"[{typeof(T).Name}] {pMessage}", LogContext);
        protected static void LogError(string pMessage) => Debug.LogError($"[{typeof(T).Name}] {pMessage}", LogContext);
        protected static void LogExeception(string pMessage) => Util.DevException($"[{typeof(T).Name}] {pMessage}", LogContext);
    }
}