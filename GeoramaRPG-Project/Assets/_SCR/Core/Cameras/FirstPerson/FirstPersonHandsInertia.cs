using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Camera
{
    public class FirstPersonHandsInertia : MonoBehaviour
    {
        private Vector3 lastRotation;

        [Header("Tilt")]
        [SerializeField] private Vector2 tiltMagnitude = Vector2.one;
        [SerializeField, Min(0)] private Vector2 tiltMax = new Vector2(8.0f, 6.0f);
        [SerializeField, Min(0)] private Vector2 tiltDampening = Vector2.one;

        [Header("Movement")]
        [SerializeField] private Rigidbody rigid = null;
        [SerializeField,Range(0, 0.01f)] private float targetMagnitude = 5.0f;
        [SerializeField, Min(0)] private float maxMagnitude = 1.0f;
        [SerializeField] private float moveDampening = 1.0f;
        [SerializeField] private Vector3 moveRelOffset = Vector3.zero;

        private Vector3 initalRelOffset;
        private float moveValue = 0.0f;

        [Space] // Bounce
        [SerializeField] private OnGround grounded = null;
        [SerializeField] private AnimationCurve bounceCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [SerializeField] private float bounceFrequncy = 0.1f;

        private float bounceProgress = 0.0f;
        private bool doBounce = true;

        private void Start() 
        {
            lastRotation = transform.parent.eulerAngles;
            initalRelOffset = transform.localPosition;

            if (grounded != null)
            {
                grounded.OnEnter.AddListener( delegate{ doBounce = true;} );
                grounded.OnExit.AddListener( delegate{ doBounce = false;} );
            }
        }

        private void LateUpdate() 
        {
            Vector3 motion = transform.parent.eulerAngles - lastRotation;
            lastRotation = transform.parent.eulerAngles;

            Vector3 rot = transform.localEulerAngles;
            rot.y = Calculate(Util.SafeAngle(rot.y), motion.y * tiltMagnitude.x, tiltMax.x, tiltDampening.x);
            rot.x = Calculate(Util.SafeAngle(rot.x), motion.x * tiltMagnitude.y, tiltMax.y, tiltDampening.y);
            transform.localRotation = Quaternion.Euler(rot);

            float v = Mathf.Min(maxMagnitude, rigid.velocity.sqrMagnitude) * targetMagnitude;

            Vector3 bounceOffset = Vector3.zero;
            if (doBounce == true)
            {
                bounceProgress += Time.deltaTime * bounceFrequncy * v;
                bounceOffset = Vector3.up * bounceCurve.Evaluate(bounceProgress);
            }

            moveValue = Mathf.Lerp(moveValue, Mathf.Clamp01(v), Time.deltaTime * moveDampening);
            transform.localPosition = Vector3.Lerp(initalRelOffset, moveRelOffset + bounceOffset, moveValue);
        
        }

        private float Calculate(float pValue, float pTarget, float pMax, float pDampening)
        {
            pTarget = Mathf.Clamp(pTarget, -pMax, pMax);
            pValue = Mathf.Lerp(pValue, pTarget, Time.deltaTime * tiltDampening.x);
            return pValue;
        }

        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.parent.position + moveRelOffset);
            Gizmos.DrawCube(transform.parent.position + moveRelOffset, Vector3.one * 0.1f);
        }
    }
}