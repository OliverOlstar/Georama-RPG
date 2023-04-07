using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	public class SmoothLocalOffset : MonoBehaviour
	{
		public Transform myTransform = null;
		public Vector3 offset = Vector3.zero;
		private Vector3 initalOffset = Vector3.zero;
		[SerializeField] private float dampening = 5.0f;

		private void Reset()
		{
			myTransform = transform;
			offset = myTransform.localPosition;
		}

		private void Start()
		{
			initalOffset = offset;
		}

		private void Update()
		{
			myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, offset, dampening * Time.deltaTime);
		}

		public void SetOffset(Vector3 pOffset) => offset = pOffset;
		public void SetOffsetY(float pHeight) => offset = new Vector3(0.0f, pHeight, 0.0f);
		public void ModifyOffsetY(float pHeight) => offset.y += pHeight;
		public void ModifyInitialOffsetY(float pHeight) => offset.y = initalOffset.y + pHeight;
		public void ResetOffset() => offset = initalOffset;
	}
}
