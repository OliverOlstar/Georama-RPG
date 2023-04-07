using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	[RequireComponent(typeof(UnityEngine.Camera))]
    public class MainCamera : MonoBehaviour
    {
		public static MainCamera Instance = null;
		public static UnityEngine.Camera Camera = null;
		private UnityEngine.Camera localCamera = null;

		private void Awake()
		{
			if (Instance != null)
			{
				Util.DevException($"[MainCamera] Second MainCamera, this should never happen. Please ensure cleaning up the old one first if a new one is intended. Destroying self ({gameObject.name}).", transform.parent);
				Destroy(gameObject);
				return;
			}
			Instance = this;
			localCamera = GetComponent<UnityEngine.Camera>();
			OnEnable();
		}

		private void OnEnable()
		{
			Camera = localCamera;
		}

		private void OnDisable()
		{
			Camera = null;
		}
	}
}
