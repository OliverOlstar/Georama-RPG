using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Camera
{
    public class SpectatorCamera : MonoBehaviour
    {
#region Singleton
        public static SpectatorCamera Instance = null;
        private void Awake() 
        {
            if (Instance != null)
            {
                Debug.LogError("[SpectatorCamera] Multiple Instances, destroying other", this);
                Destroy(Instance);
            }
            Instance = this;
        }

        private void OnDestroy() 
        {
            Instance = null;
        }
#endregion

        private enum Mode
        {
            None,
            FirstPerson,
            ThirdPerson,
            Freefly
        }

        //[SerializeField] private InputBridge_Spectator inputBridge = null;
        [SerializeField] private FollowTarget firstPersonCamera = null;
        [SerializeField] private ThirdPersonCamera thirdPersonCamera = null;
        [SerializeField] private FreeflyCamera freeflyCamera = null;
        private Transform cameraTransform;
        
        [HideInInspector] public List<SpectatorTarget> targets = new List<SpectatorTarget>();
        
        private int targetIndex = 0;
        private float canInputTime = 0.0f;

        private Mode mode = Mode.None;

        private void Start() 
        {
            //inputBridge.onLook.AddListener(OnLook);
            //inputBridge.onLookDelta.AddListener(OnLookDelta);
            //inputBridge.onMove.AddListener(OnMove);
            //inputBridge.onMoveVertical.AddListener(OnMoveVertical);
            //inputBridge.onZoom.AddListener(OnZoom);
            //inputBridge.onSprint.AddListener(OnSprint);
            //inputBridge.onMode.AddListener(OnMode);
            //inputBridge.onTarget.AddListener(OnTarget);

            cameraTransform = UnityEngine.Camera.main.transform;

            OnEnable();
        }

        private void OnEnable() 
        {
            if (cameraTransform != null) // Don't run before Start()
            {
                freeflyCamera.gameObject.SetActive(false);
                firstPersonCamera.gameObject.SetActive(false);
                thirdPersonCamera.gameObject.SetActive(false);

                mode = Mode.None;
                SwitchMode(Mode.Freefly);
            }
        }

        private void SwitchMode()
        {
            if (mode == Mode.ThirdPerson || (targets.Count == 0 && mode == Mode.Freefly))
            {
                // To Freefly from Third
                SwitchMode(Mode.Freefly);
            }
            else if (mode == Mode.Freefly)
            {
                // To FirstPerson from Free
                SwitchMode(Mode.FirstPerson);
            }
            else if (mode == Mode.FirstPerson)
            {
                // To ThirdPerson from First
                SwitchMode(Mode.ThirdPerson);
            }
        }

        private void SwitchMode(Mode pMode)
        {
            if (mode == pMode)
                return;
            mode = pMode;
            
            if (mode == Mode.Freefly)
            {
                freeflyCamera.gameObject.SetActive(true);
                thirdPersonCamera.gameObject.SetActive(false);

                if (cameraTransform != null)
                {
                    freeflyCamera.gameObject.transform.position = cameraTransform.position;
                    freeflyCamera.gameObject.transform.rotation = cameraTransform.rotation;
                }

                //inputBridge.ClearInputs();
            }
            else if (mode == Mode.FirstPerson)
            {
                canInputTime = Time.time + 0.4f;

                firstPersonCamera.gameObject.SetActive(true);
                freeflyCamera.gameObject.SetActive(false);
                
                targets[targetIndex].Toggle(true);

                firstPersonCamera.posTarget = targets[targetIndex].firstPersonTarget;
                firstPersonCamera.rotTarget = targets[targetIndex].firstPersonTarget;
            }
            else if (mode == Mode.ThirdPerson)
            {
                canInputTime = Time.time + 0.4f;

                thirdPersonCamera.gameObject.SetActive(true);
                firstPersonCamera.gameObject.SetActive(false);
                
                targets[targetIndex].Toggle(false);

                thirdPersonCamera.transform.rotation = Quaternion.Euler(30.0f, targets[targetIndex].thirdPersonTarget.eulerAngles.y, 0.0f);
                thirdPersonCamera.followTransform = targets[targetIndex].thirdPersonTarget;

                //inputBridge.ClearInputs();
            }
        }

        private void SwitchTarget()
        {
            // TODO on if a target stop exisiting, switch to someone else
                
            targets[targetIndex].Toggle(false);

            targetIndex++;
            if (targetIndex == targets.Count)
                targetIndex = 0;

            if (mode == Mode.Freefly)
            {
                SwitchMode();
            }

            if (mode == Mode.FirstPerson)
            {
                firstPersonCamera.posTarget = targets[targetIndex].firstPersonTarget;
                firstPersonCamera.rotTarget = targets[targetIndex].firstPersonTarget;
                
                targets[targetIndex].Toggle(true);
            }
            else if (mode == Mode.ThirdPerson)
            {
                thirdPersonCamera.followTransform = targets[targetIndex].thirdPersonTarget;
            }
        }

        public void OnTargetLost(SpectatorTarget pTarget)
        {
            int index = targets.IndexOf(pTarget);
            if (targetIndex == index && mode != Mode.Freefly)
            {
                SwitchMode(Mode.Freefly);
            }
            
            if (targetIndex >= index && targetIndex > 0)
            {
                targetIndex--;
            }
        }

#region Input
        protected virtual void OnLook(Vector2 pInput) 
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.ThirdPerson)
            {
                thirdPersonCamera.OnLook(pInput);
            }
            else if (mode == Mode.Freefly)
            {
                freeflyCamera.OnLook(pInput);
            }
        }
        protected virtual void OnLookDelta(Vector2 pInput) 
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.ThirdPerson)
            {
                thirdPersonCamera.OnLookDelta(pInput);
            }
            else if (mode == Mode.Freefly)
            {
                freeflyCamera.OnLookDelta(pInput);
            }
        }

        protected virtual void OnMove(Vector2 pInput) 
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.Freefly)
            {
                freeflyCamera.OnMoveHorizontal(pInput);
            }
        }
        protected virtual void OnMoveVertical(float pInput) 
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.Freefly)
            {
                freeflyCamera.OnMoveVertical(pInput);
            }
        }

        protected virtual void OnZoom(float pInput) 
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.ThirdPerson)
            {
                thirdPersonCamera.OnZoom(pInput);
            }
        }

        protected virtual void OnSprint(bool pInput)
        {
            if (Time.time < canInputTime)
                return;

            if (mode == Mode.Freefly)
            {
                freeflyCamera.OnSprint(pInput);
            }
        }
        
        protected virtual void OnMode()
        {
            SwitchMode();
        }
        
        protected virtual void OnTarget()
        {
            if (Time.time < canInputTime)
                return;

            if (targets.Count > 2)
                SwitchTarget();
        }
    }
#endregion
}