using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Camera
{
    public class EagleEyeCamera : MonoBehaviour
    {
		[SerializeField]
		private InputBridge_EagleEye input = null;
		[SerializeField, DisableInPlayMode]
		private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Camera);

		[Header("Follow")]
		public Transform cameraTransform = null; // Should be child
		[SerializeField]
		private Vector3 childOffset = new Vector3(0.0f, 2.0f, -5.0f);

		[Header("Move")]
		[SerializeField]
		private float moveSpeed = 1.0f;
		[SerializeField]
		private float moveDeltaSpeed = 1.0f;
		private Vector3 targetPosition;

		[Header("Look")]
		[SerializeField]
		private Transform lookTransform = null;
		private float RotateInput => input.Rotate.Input;
		[SerializeField]
		private float rotateSpeed = 1.0f;

		[Header("Zoom")]
		[SerializeField]
		private float zoomSpeed = 1.0f;
		[SerializeField]
		private Vector2 zoomDistanceClamp = new Vector2(1.0f, 5.0f);
		private float currZoom = 0.5f;

		[Header("Collision")]
		[SerializeField]
		private LayerMask collisionLayers = new LayerMask();
		[SerializeField]
		private float collisionRadius = 0.2f;
		[SerializeField]
		private float collisionZoomSpacing = 1.0f;


		private void Reset()
		{
			lookTransform = transform;
			if (transform.childCount > 0)
			{
				cameraTransform = transform.GetChild(0);
			}
		}

		private void Start()
		{
			targetPosition = lookTransform.position;
			currZoom = childOffset.magnitude;
			cameraTransform.localPosition = childOffset;
			cameraTransform.LookAt(transform.position);

			if (input != null)
			{
				input.MoveDelta.Value.onChanged.AddListener(OnMoveDelta);
				input.Zoom.onChanged.AddListener(OnZoom);
			}

			updateable.Register(Tick);
		}

		private void OnDestroy()
		{
			updateable.Deregister();
		}

		private void Tick(float pDeltaTime)
		{
			Move(input.Move.Input * moveSpeed * pDeltaTime);
			DoMoveUpdate(pDeltaTime);
			DoZoomUpdate(pDeltaTime);
			RotateCamera(RotateInput * rotateSpeed * pDeltaTime);
			DoCollision();
		}

		private void DoMoveUpdate(in float pDeltaTime)
		{
			lookTransform.position = Vector3.Lerp(lookTransform.position, targetPosition, pDeltaTime * 10.0f);
		}

		private void Move(Vector2 pInput)
		{
			if (pInput.sqrMagnitude > Util.NEARZERO)
			{
				targetPosition += pInput.x * Util.Horizontalize(cameraTransform.right);
				targetPosition += pInput.y * Util.Horizontalize(cameraTransform.forward);
			}
		}

		private void DoZoomUpdate(in float pDeltaTime)
		{
			cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, childOffset.normalized * currZoom, pDeltaTime * 13.0f);
		}

		private void RotateCamera(float pInput)
		{
			if (lookTransform == null || pInput == 0.0f)
			{
				return;
			}
			Vector3 euler = lookTransform.eulerAngles;
			euler.y -= pInput;
			lookTransform.rotation = Quaternion.Euler(euler);
		}

		private void DoCollision()
		{
			Vector3 direction = lookTransform.TransformDirection(childOffset.normalized);
			if (Physics.Linecast(lookTransform.position + (direction * zoomDistanceClamp.y), cameraTransform.position - (direction * collisionRadius), out RaycastHit hit, collisionLayers))
			{
				float magnitude = (zoomDistanceClamp.y - hit.distance) + collisionRadius;
				cameraTransform.localPosition = childOffset.normalized * magnitude;
				currZoom = magnitude + collisionZoomSpacing;
			}
		}

		#region Input
		public void OnMoveDelta(Vector2 pInput)
		{
			Move(pInput * moveDeltaSpeed * (1 + currZoom - zoomDistanceClamp.x));
		}

		public void OnZoom(float pInput)
		{
			currZoom += (pInput * zoomSpeed);
			currZoom = Mathf.Clamp(currZoom, zoomDistanceClamp.x, zoomDistanceClamp.y);
		}
		#endregion

		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying && cameraTransform != null)
			{
				cameraTransform.localPosition = childOffset;
				cameraTransform.LookAt(transform.position);
			}
		}

		private void OnDrawGizmos()
		{
			if (cameraTransform == null)
			{
				return;
			}
			Gizmos.color = Color.green;
			Gizmos.DrawLine(lookTransform.position, lookTransform.position + lookTransform.TransformDirection(childOffset) * (cameraTransform.localPosition.magnitude + collisionRadius));
		}
	}
}
