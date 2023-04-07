using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
    public class CameraFollowRigidbody : MonoBehaviour
    {
        [SerializeField] private Rigidbody target = null;

        [Header("Look")]
        [SerializeField] private float lookVelocity = 1.0f;
        [SerializeField] private Vector3 lookOffset = new Vector3();
        [SerializeField] private float lookDampening = 5.0f;

        [Header("Follow")]
        [SerializeField] private Vector3 followOffset = new Vector3();
        [SerializeField] private float followDampening = 1.0f;

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + followOffset, followDampening * Time.deltaTime);

            Vector3 lookAtTarget = target.transform.position + (target.velocity * lookVelocity) + lookOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtTarget - transform.position), Time.deltaTime * lookDampening);
        }

        private void OnDrawGizmosSelected() 
        {
            transform.position = target.position + followOffset;
            transform.LookAt(target.transform.position + (target.velocity * lookVelocity) + lookOffset);
        }
    }
}