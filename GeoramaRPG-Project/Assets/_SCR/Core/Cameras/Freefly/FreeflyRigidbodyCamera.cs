using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Camera
{
    public class FreeflyRigidbodyCamera : FreeflyCamera
    {
        [SerializeField] private Rigidbody moveRigidbody = null;

        private void Start() 
        {
            moveTransform = null;
        }

        private void OnEnable() 
        {
            moveRigidbody.velocity = Vector3.zero;
        }

        protected override void DoMove(Vector2 pMovement, float pUp, float pMult)
        {
            Vector3 move = (pMovement.y * transform.forward) + (pMovement.x * transform.right) + (pUp * transform.up);
            moveRigidbody.velocity = move.normalized * pMult;
        }
    }
}