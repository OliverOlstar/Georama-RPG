using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher 
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] 
		private InputBridge_Camera input = null;
		[SerializeField]
		private Updateable updateable = new Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Camera);

		[Header("Follow")]
        public Transform followTransform = null;
        [SerializeField]
		private Vector3 offset = new Vector3(0.0f, 0.5f, 0.0f);
        public Transform cameraTransform = null; // Should be child
        [SerializeField]
		private Vector3 childOffset = new Vector3(0.0f, 2.0f, -5.0f);
		[SerializeField, Min(0.0f)]
		private float m_FollowYDampening = 5.0f;
        
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
            DoFollow(0.0f, 0.0f);

            currZoom = childOffset.magnitude;
            cameraTransform.localPosition = childOffset;

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

		private void Tick(float pDeltaTime) 
        {
            DoFollow(pDeltaTime, m_FollowYDampening);
            
            if (lookInput != Vector2.zero)
            {
                RotateCamera(lookInput * sensitivityUpdate * pDeltaTime);
            }

            DoZoomUpdate(pDeltaTime);
            DoCollision();
        }

        private void DoFollow(in float pDeltaTime, in float pYDampening)
        {
            if (followTransform == null)
            {
				return;
            }
			Vector3 position = followTransform.position + offset;
			if (pYDampening > 0.0f)
			{
				position.y = Mathf.Lerp(transform.position.y, position.y, pYDampening * pDeltaTime);
			}
			transform.position = position;
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

        private void DoZoom(float pInput)
        {
            currZoom += (pInput * zoomSpeed);
            currZoom = Mathf.Clamp(currZoom, zoomDistanceClamp.x, zoomDistanceClamp.y);
        }

        private void DoZoomUpdate(in float pDeltaTime)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, childOffset.normalized * currZoom, pDeltaTime * 15.0f);
        }

        private void DoCollision()
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(childOffset.normalized), out RaycastHit hit, cameraTransform.localPosition.magnitude + collisionRadius, collisionLayers))
            {
                cameraTransform.localPosition = childOffset.normalized * (hit.distance - collisionRadius);
            }
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
            DoZoom(pInput);
        }
#endregion

        private void OnDrawGizmosSelected() 
        {
            if (Application.isPlaying == false)
            {
                if (cameraTransform == null)
                    return;
                DoFollow(0.0f, 0.0f);
                cameraTransform.localPosition = childOffset;
            }
        }

        private void OnDrawGizmos()
        {
            if (cameraTransform == null)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(childOffset) * (cameraTransform.localPosition.magnitude + collisionRadius));
        }
    }
}