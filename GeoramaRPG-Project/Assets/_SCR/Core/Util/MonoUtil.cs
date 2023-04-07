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
			public const int Camera = 1000;
			public const int Last = int.MaxValue;
		}

		public struct Updateable
		{
			private Action<float> action;
			[SerializeField, DisableInPlayMode]
			private UpdateType type;
			[SerializeField, DisableInPlayMode]
			private float priority;

			public Action<float> Action => action;
			public UpdateType Type => type;
			public float Priority => priority;

			public Updateable(UpdateType pType, float pPriority)
			{
				action = null;
				type = pType;
				priority = pPriority;
			}

			public void Register(Action<float> pAction)
			{
				if (pAction == null)
				{
					LogExeception("Register() was passed a null action");
					return;
				}
				if (action != null)
				{
					if (action.Method == pAction.Method)
					{
						LogWarning("Register() was passed the same method which was already registered, returning.");
						return;
					}
					Deregister(); // Remove old action before registering the new one
				}
				action = pAction;
				RegisterUpdate(this);
			}

			public void Deregister()
			{
				DeregisterUpdate(this);
				action = null;
			}

			public override string ToString()
			{
				return $"Updateable(Action: {action.Target} - {action.Method.Name}, Type {type}, Priority {priority})";
			}
		}

		private static List<Updateable> updatables = new List<Updateable>();
		private static List<Updateable> earlyUpdatables = new List<Updateable>();
		private static List<Updateable> lateUpdatables = new List<Updateable>();
		private static List<Updateable> fixedUpdatables = new List<Updateable>();

		private static void RegisterUpdate(in Updateable pUpdatable)
		{
			TryCreate();

			ref List<Updateable> items = ref GetUpdatables(pUpdatable.Type);
			int index;
			for (index = 0; index < items.Count; index++)
			{
				if (items[index].Priority <= pUpdatable.Priority)
				{
					break;
				}
			}
			items.Insert(index, pUpdatable);
		}

		private static void DeregisterUpdate(in Updateable pUpdatable)
		{
			if (!GetUpdatables(pUpdatable.Type).Remove(pUpdatable))
			{
				LogError($"DeregisterUpdate() Failed to remove {pUpdatable}.");
			}
		}

		private static ref List<Updateable> GetUpdatables(UpdateType pType)
		{
			switch (pType)
			{
				case UpdateType.Early:
					return ref earlyUpdatables;
				case UpdateType.Late:
					return ref lateUpdatables;
				case UpdateType.Fixed:
					return ref fixedUpdatables;
				default:
					return ref updatables;
			}
		}

		private void Update()
		{
			foreach (Updateable updatable in earlyUpdatables)
			{
				updatable.Action.Invoke(Time.deltaTime);
			}
			foreach (Updateable updatable in updatables)
			{
				updatable.Action.Invoke(Time.deltaTime);
			}
		}

		private void LateUpdate()
		{
			foreach (Updateable updatable in lateUpdatables)
			{
				updatable.Action.Invoke(Time.deltaTime);
			}
		}

		private void FixedUpdate()
		{
			foreach (Updateable updatable in fixedUpdatables)
			{
				updatable.Action.Invoke(Time.fixedDeltaTime);
			}
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
