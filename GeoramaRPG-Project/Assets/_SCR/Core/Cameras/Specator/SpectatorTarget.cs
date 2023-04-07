using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Camera
{
    public class SpectatorTarget : MonoBehaviour
    {
        public Transform thirdPersonTarget = null;
        public Transform firstPersonTarget = null;

        [SerializeField] private GameObject[] disableOnSpectate = new GameObject[0];
        [SerializeField] private GameObject[] enableOnSpectate = new GameObject[0];

        private void Start() 
        {
            OnEnable(); // Calls after SpectatorCamera Singleton Awake();
        }

        private void OnEnable() 
        {
            SpectatorCamera cam = SpectatorCamera.Instance;
            if (cam != null)
            {
                cam.targets.Add(this);
            }
        }

        private void OnDisable()
        {
            SpectatorCamera cam = SpectatorCamera.Instance;
            if (cam != null)
            {
                if (cam.isActiveAndEnabled)
                    cam.OnTargetLost(this);
                cam.targets.Remove(this);
            }
        }

        public void Toggle(bool pFirstPerson)
        {
            foreach (GameObject o in disableOnSpectate)
            {
                o.SetActive(!pFirstPerson);
            }
            
            foreach (GameObject o in enableOnSpectate)
            {
                o.SetActive(pFirstPerson);
            }
        }
    }
}