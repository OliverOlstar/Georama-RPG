using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
    public class DisableOnPause : MonoBehaviour
    {
        [SerializeField] private GameObject[] disableOnPause = new GameObject[1];
        [SerializeField] private GameObject[] enableOnPause = new GameObject[1];

        private void Start() 
        {
            PauseSystem.onPause += OnPause;
            PauseSystem.onUnpause += OnUnpause;
        }

        private void OnDestroy() 
        {
            PauseSystem.onPause -= OnPause;
            PauseSystem.onUnpause -= OnUnpause;
        }
        
        private void OnPause() => Toggle(true);
        private void OnUnpause() => Toggle(false);

        private void Toggle(bool pPause)
        {
            foreach (GameObject o in disableOnPause)
            {
                o.SetActive(!pPause);
            }
            
            foreach (GameObject o in enableOnPause)
            {
                o.SetActive(pPause);
            }
        }
    }
}