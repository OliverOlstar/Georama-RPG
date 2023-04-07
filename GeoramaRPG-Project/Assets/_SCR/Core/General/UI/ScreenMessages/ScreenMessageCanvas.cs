using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    public class ScreenMessageCanvas : MonoBehaviour
    {
        [SerializeField]
        private ScreenMessageWidget largeMessage = null;
        [SerializeField]
        private ScreenMessageWidget smallMessage = null;

        public ScreenMessageWidget LargeMessage => largeMessage;
        public ScreenMessageWidget SmallMessage => smallMessage;
    }
}
