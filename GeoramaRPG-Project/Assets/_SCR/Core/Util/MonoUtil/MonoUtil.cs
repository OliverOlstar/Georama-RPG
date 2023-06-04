using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	/// <summary>
	/// Public access point for any class to access MonoBehaviour functions
	/// </summary>
    public class MonoUtil : MonoBehaviourSingleton<MonoUtil>
    {
		private void Awake()
		{
			DontDestroyOnLoad(this);
		}
		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		#region Updatables
		public enum UpdateType
		{
			Default = 0,
			Early,
			Late,
			Fixed
		}

		public static class Priorities
		{
			public const int First = int.MinValue;
			public const int Input = -2000;
			public const int UI = -1000;
			public const int Default = 0;
			public const int CharacterController = 400;
			public const int ModelController = 500;
			public const int Animation = 700;
			public const int Camera = 1000;
			public const int Last = int.MaxValue;
		}

		private static UpdateableCollection updatables = new UpdateableCollection();
		private static UpdateableCollection earlyUpdatables = new UpdateableCollection();
		private static UpdateableCollection lateUpdatables = new UpdateableCollection();
		private static UpdateableCollection fixedUpdatables = new UpdateableCollection();

		public static void RegisterUpdate(Updateable pUpdatable)
		{
			TryCreate();
			GetUpdatables(pUpdatable.Type).Add(pUpdatable);
		}

		public static void DeregisterUpdate(Updateable pUpdatable)
		{
			if (!GetUpdatables(pUpdatable.Type).Remove(pUpdatable))
			{
				LogError($"DeregisterUpdate() Failed to remove {pUpdatable}.");
			}
		}

		private static UpdateableCollection GetUpdatables(UpdateType pType)
		{
			switch (pType)
			{
				case UpdateType.Early:
					return earlyUpdatables;
				case UpdateType.Late:
					return lateUpdatables;
				case UpdateType.Fixed:
					return fixedUpdatables;
				case UpdateType.Default:
					return updatables;
				default:
					throw new NotImplementedException();
			}
		}

		private void Update()
		{
			earlyUpdatables.Update(Time.deltaTime);
			updatables.Update(Time.deltaTime);
		}

		private void LateUpdate()
		{
			lateUpdatables.Update(Time.deltaTime);
		}

		private void FixedUpdate()
		{
			fixedUpdatables.Update(Time.fixedDeltaTime);
		}
		#endregion Updatables

		#region Coroutines
		public static Coroutine Start(in IEnumerator pEnumerator)
		{
			return Instance.StartCoroutine(pEnumerator);
		}
		public static void Stop(ref Coroutine pCoroutine)
		{
			if (pCoroutine == null)
			{
				return;
			}
			Instance.StopCoroutine(pCoroutine);
			pCoroutine = null;
		}
		#endregion Coroutines
	}
}
