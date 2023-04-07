using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    [RequireComponent(typeof(CharacterController))]
    public class RootMotionCharacterReciever : MonoBehaviour
    {
        private RootMotionCharacter parent;

        public void Init(RootMotionCharacter pParent)
        {
            parent = pParent;
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            parent.OnControllerColliderHit(hit);
        }
    }
}
