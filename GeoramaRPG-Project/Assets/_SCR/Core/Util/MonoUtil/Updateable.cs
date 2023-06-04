using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace OliverLoescher
{
	[System.Serializable]
	public class Updateable
	{
		private Action<float> action;
		[SerializeField, DisableInPlayMode]
		private MonoUtil.UpdateType type;
		[SerializeField, DisableInPlayMode]
		private float priority;

		public Action<float> Action => action;
		public MonoUtil.UpdateType Type => type;
		public float Priority => priority;

		public Updateable(MonoUtil.UpdateType pType, float pPriority)
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
			MonoUtil.RegisterUpdate(this);
		}

		public void Deregister()
		{
			MonoUtil.DeregisterUpdate(this);
			action = null;
		}

		public override string ToString()
		{
			return $"Updateable(Action: {action.Target} - {action.Method.Name}, Type {type}, Priority {priority})";
		}
		
        protected static void Log(string pMessage) => Debug.Log($"[MonoUtil] {pMessage}", MonoUtil.Instance);
        protected static void LogWarning(string pMessage) => Debug.LogWarning($"[MonoUtil] {pMessage}", MonoUtil.Instance);
        protected static void LogError(string pMessage) => Debug.LogError($"[MonoUtil] {pMessage}", MonoUtil.Instance);
        protected static void LogExeception(string pMessage) => Util.DevException($"[MonoUtil] {pMessage}", MonoUtil.Instance);
	}
}
