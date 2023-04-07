using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

namespace OliverLoescher.Camera
{
    public class FirstPersonCameraInertia : MonoBehaviour
    {
        [SerializeField] private new CinemachineVirtualCamera camera = null;
        [SerializeField] private Rigidbody rigid = null;
        private Vector3 lastVelocity;
        private Vector3 lastPosition;

        [Header("Tilt")]
        [SerializeField, Range(0, 4999)] private float tiltMagnitude = 1.0f;
        [SerializeField, Range(0, 45)] private float tiltMax = 12.5f;
        [SerializeField, Range(0, 50)] private float tiltDampening = 10.0f;

        [Header("Spring")]
        [SerializeField, Range(0, 50)] private float sprintSpring = 45.0f;
        [SerializeField, Range(0, 50)] private float springDamper = 10.0f;
        private float springVel = 0;
        private float springY = 0;

        [Header("FOV")]
        [SerializeField, MinMaxSlider(0, 180, true)] private Vector2 fovMinMax = new Vector2(70.0f, 110.0f);
        [SerializeField, MinMaxSlider(0, 20, true)] private Vector2 fovVelocity = new Vector2(0.0f, 10.0f);
        [SerializeField, Min(0)] private float fovDampening = 10.0f;

        private void Start()
        {
            lastPosition = transform.position;
            if (camera != null)
			{
                camera.m_Lens.FieldOfView = fovMinMax.x;
            }
        }

		private void LateUpdate()
        {
            Vector3 velocity = rigid.velocity - lastVelocity;
            lastVelocity = rigid.velocity;

            DoSpring(velocity, Time.deltaTime);
        }

		private void FixedUpdate() 
        {
            Vector3 motion = transform.parent.position - lastPosition;
            Vector3 relMotion = transform.InverseTransformDirection(motion);
            lastPosition = transform.parent.position;

            DoTilt(relMotion, Time.fixedDeltaTime);
            DoFOV(Time.fixedDeltaTime);
        }

		private void DoTilt(Vector3 pRelMotion, float pDeltaTime)
        {
            Vector3 rot = transform.localEulerAngles;
            rot.z = pRelMotion.x * -tiltMagnitude;
			rot.z = Mathf.Clamp(rot.z, -tiltMax, tiltMax);
			rot.z = Mathf.Lerp(Util.SafeAngle(transform.localEulerAngles.z), rot.z, pDeltaTime * tiltDampening);
            transform.localRotation = Quaternion.Euler(rot);
        }

        private void DoSpring(Vector3 pVelocity, float pDeltaTime)
        {
            springVel += -pVelocity.y;
            springVel += sprintSpring * -transform.localPosition.y * pDeltaTime;
            springVel += springDamper * -springVel * pDeltaTime;

            springY += springVel * pDeltaTime;
            transform.localPosition = transform.parent.InverseTransformVector(springY * Vector3.up);
        }

        private void DoFOV(float pDeltaTime)
		{
            if (camera == null)
			{
                return;
			}
            float fov01 = Util.SmoothStep(fovVelocity, Util.Horizontalize(rigid.velocity).magnitude);
            camera.m_Lens.FieldOfView = Mathf.Lerp(camera.m_Lens.FieldOfView, Mathf.Lerp(fovMinMax.x, fovMinMax.y, fov01), pDeltaTime * fovDampening);
        }
    }
}