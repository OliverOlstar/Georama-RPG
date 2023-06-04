using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher 
{
    public class ThirdPersonTargetingCamera : MonoBehaviour
    {
        [SerializeField] 
		private InputBridge_Camera input = null;
		[SerializeField]
		private Updateable updateable = new Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Camera);

		[Header("Follow")]
        public Transform followTransform = null;
		public Transform target = null;
        [SerializeField]
		private Vector3 offset = new Vector3(0.0f, 0.5f, 0.0f);
        public Transform cameraTransform = null; // Should be child
        [SerializeField]
		private Vector3 childOffset = new Vector3(0.0f, 2.0f, -5.0f);
        
        [Header("Look")]
        [SerializeField]
		private Transform lookTransform = null;
        [SerializeField, MinMaxSlider(-90, 90, true)] 
		private Vector2 lookYClamp = new Vector2(-40, 50);
        [SerializeField]
		private float sensitivityDelta = 1.0f;
        [SerializeField]
		private float sensitivityUpdate = 1.0f;
        private Vector2 lookInput = new Vector2();

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
			updateable.Register(Tick);
		}

		private void OnDestroy()
		{
			updateable.Deregister();
		}

		private void OnEnable()
		{
            if (input != null)
			{
				input.Look.onChanged.AddListener(OnLook);
				input.LookDelta.onChanged.AddListener(OnLookDelta);
				input.Zoom.onChanged.AddListener(OnZoom);
			}
			
			if (target == null || followTransform == null)
			{
				return;
			}
			Vector3 targetPos = Vector3.Lerp(target.position, followTransform.position, m_Lerp);
			m_PreviousRotation = (m_Lerp > 0.5f ? target.position : followTransform.position) - targetPos;
		}

		private void OnDisable()
		{
            if (input != null)
			{
				input.Look.onChanged.RemoveListener(OnLook);
				input.LookDelta.onChanged.RemoveListener(OnLookDelta);
				input.Zoom.onChanged.RemoveListener(OnZoom);
			}
		}

		[SerializeField]
		private float m_MinDistance = 4.0f;
		[SerializeField, Range(Util.NEARZERO, 1.0f - Util.NEARZERO)]
		private float m_Lerp = 0.5f;

		private Vector3 m_PreviousRotation = Vector3.forward;

		private void Tick(float pDeltaTime) 
        {
			if (target == null || followTransform == null)
			{
				return;
			}

			Vector3 targetPos = Vector3.Lerp(target.position, followTransform.position, m_Lerp);
            transform.position = targetPos + offset;
			
			Vector3 direction = (m_Lerp > 0.5f ? target.position : followTransform.position) - targetPos;
			float distance = direction.magnitude * 2;
			distance = Mathf.Max(distance + 1.0f, m_MinDistance);
            cameraTransform.localPosition = childOffset * distance;
            
            if (lookInput != Vector2.zero)
            {
                RotateCamera(lookInput * sensitivityUpdate * pDeltaTime);
            }
			
            if (Physics.Raycast(transform.position, transform.TransformDirection(childOffset.normalized), out RaycastHit hit, cameraTransform.localPosition.magnitude + collisionRadius, collisionLayers))
            {
                cameraTransform.localPosition = childOffset.normalized * (hit.distance - collisionRadius);
            }

            if (lookTransform == null)
			{
                return;
			}
			if (distance > m_MinDistance)
			{
				lookTransform.rotation *= Quaternion.FromToRotation(Util.Horizontal(m_PreviousRotation), Util.Horizontal(direction));
			}
			m_PreviousRotation = direction;
            Vector3 euler = lookTransform.eulerAngles;
            euler.z = 0.0f;
            lookTransform.rotation = Quaternion.Euler(euler);

			// DoZoomUpdate(pDeltaTime);
            // DoCollision();
        }

        private void RotateCamera(Vector2 pInput)
        {
            if (lookTransform == null)
			{
                return;
			}
            Vector3 euler = lookTransform.eulerAngles;
            euler.x = Mathf.Clamp(Util.SafeAngle(euler.x - pInput.y), lookYClamp.x, lookYClamp.y);
            euler.y = euler.y + pInput.x;
            euler.z = 0.0f;
            lookTransform.rotation = Quaternion.Euler(euler);
        }

#region Input
        public void OnLook(Vector2 pInput)
        {
            lookInput = pInput;
        }

        public void OnLookDelta(Vector2 pInput)
        {
            RotateCamera(pInput * sensitivityDelta);
        }

        public void OnZoom(float pInput)
        {
            // DoZoom(pInput);
        }
#endregion
    }
}