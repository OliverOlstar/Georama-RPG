using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher.Camera
{
    public class FreeflyCamera : MonoBehaviour
    {
        [Header("Look")]
        [SerializeField] protected Transform lookTransform = null;
        [SerializeField] private float sensitivityDelta = 1.0f;
        [SerializeField] private float sensitivityUpdate = 1.0f;
        [SerializeField, MinMaxSlider(-90, 90, true)] private Vector2 cameraYClamp = new Vector2(-40, 50);

        [Header("Movement")]
        [SerializeField] protected Transform moveTransform = null;
        [SerializeField] private float moveSpeed = 1.0f;
        [SerializeField] private float sprintSpeed = 2.0f;

        // Inputs
        private Vector2 moveInputHorizontal = new Vector2();
        private float moveInputVertical = 0.0f;
        private Vector2 lookInput = new Vector2();
        private bool sprintInput = false;
        
        private void FixedUpdate() 
        {
            if (moveInputHorizontal != Vector2.zero || moveInputVertical != 0.0f)
            {
                DoMove(moveInputHorizontal, moveInputVertical, (sprintInput ? sprintSpeed : moveSpeed) * Time.fixedDeltaTime);
            }
            
            if (lookInput != Vector2.zero)
            {
                DoRotateCamera(lookInput * sensitivityUpdate * Time.fixedDeltaTime);
            }
        }

        protected virtual void DoMove(Vector2 pMovement, float pUp, float pMult)
        {
            Vector3 move = (pMovement.y * transform.forward) + (pMovement.x * transform.right) + (pUp * transform.up);
            moveTransform.position += move.normalized * pMult;
        }

        protected virtual void DoRotateCamera(Vector2 pInput)
        {
            Vector3 euler = lookTransform.eulerAngles;
            euler.x = Mathf.Clamp(Util.SafeAngle(euler.x - pInput.y), cameraYClamp.x, cameraYClamp.y);
            euler.y = euler.y + pInput.x;
            euler.z = 0.0f;
            lookTransform.rotation = Quaternion.Euler(euler);
        }

#region Inputs
        public void OnMoveHorizontal(Vector2 pInput)
        {
            moveInputHorizontal = pInput;
        }

        public void OnMoveVertical(float pInput)
        {
            moveInputVertical = pInput;
        }

        public void OnLook(Vector2 pInput)
        {
            lookInput = pInput;
        }

        public void OnLookDelta(Vector2 pInput)
        {
            DoRotateCamera(pInput * sensitivityDelta);
        }

        public void OnSprint(bool pInput)
        {
            sprintInput = pInput;
        }
#endregion
    }
}
