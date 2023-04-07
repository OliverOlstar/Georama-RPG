using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OliverLoescher 
{
	public static class UnityEventsUtil
	{
		[System.Serializable] 
		public class TransformEvent : UnityEvent<Transform> { }

		[System.Serializable] 
		public class TransformIntEvent : UnityEvent<Transform, int> { }

		[System.Serializable] 
		public class RigidbodyEvent : UnityEvent<Rigidbody> { }

		[System.Serializable]
		public class BoolEvent : UnityEvent<bool> { }

		[System.Serializable]
		public class IntEvent : UnityEvent<int> { }

		[System.Serializable]
		public class FloatEvent : UnityEvent<float> { }

		[System.Serializable]
		public class DoubleFloatEvent : UnityEvent<float, float> { }

		[System.Serializable]
		public class Vector2Event : UnityEvent<Vector2> { }

		[System.Serializable]
		public class Vector3Event : UnityEvent<Vector3> { }
		
		[System.Serializable]
		public class StringEvent : UnityEvent<string> { }
		
		[System.Serializable]
		public class RaycastHitEvent : UnityEvent<RaycastHit> { }
	}
}