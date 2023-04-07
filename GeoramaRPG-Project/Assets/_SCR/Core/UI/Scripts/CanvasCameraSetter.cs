using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.UI
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasCameraSetter : MonoBehaviour
	{
		[SerializeField, Min(0.0f)]
		private float planeDistance = 0.101f;

		private void Start()
		{
			if (!TryGetComponent(out Canvas canvas))
            {
				Debug.LogError($"There is no canvas with this {GetType().Name}. This should never happen.", this);
				return;
            }
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = Camera.main;
			canvas.planeDistance = planeDistance;
			Destroy(this);
		}
	}
}