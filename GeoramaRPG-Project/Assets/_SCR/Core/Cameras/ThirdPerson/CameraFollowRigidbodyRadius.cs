using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
    public class CameraFollowRigidbodyRadius : MonoBehaviour
    {
        [SerializeField] private Rigidbody target = null;
		[SerializeField]
		private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Camera);

		[Header("Look")]
        [SerializeField] private float lookVelocity = 1.0f;
        [SerializeField] private Vector3 lookOffset = new Vector3();
        [SerializeField] private float lookDampening = 5.0f;

        [Header("Follow")]
        [SerializeField] private float followDistance = 9.0f;
        [SerializeField] private float followHeight = 2.0f;
        [SerializeField] private float followDampening = 1.0f;

		private void Start()
		{
			updateable.Register(Tick);
		}

		private void OnDestroy()
		{
			updateable.Deregister();
		}

		void Tick(float pDeltaTime)
        {
			Vector3 offset = Util.Horizontalize(transform.position - target.position) * followDistance; // x, z
            offset.y = followHeight; // y
            transform.position = Vector3.Lerp(transform.position, target.position + offset, followDampening * pDeltaTime);

            Vector3 lookAtTarget = target.transform.position + (target.velocity * lookVelocity) + lookOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtTarget - transform.position), pDeltaTime * lookDampening);
        }

        private void OnDrawGizmosSelected()
        {
			if (target == null)
			{
				return;
			}
            // transform.position = target.position + followOffset;
            transform.LookAt(target.transform.position + (target.velocity * lookVelocity) + lookOffset);
        }
    }
}