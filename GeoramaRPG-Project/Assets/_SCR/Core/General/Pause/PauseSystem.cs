using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OliverLoescher
{
    public static class PauseSystem
    {
        public delegate void PauseEvent();

        public static PauseEvent onPause;
        public static PauseEvent onUnpause;
        public static bool isPaused { get; private set; } = false;

        public static void Pause(bool pPause)
        {
            if (isPaused != pPause)
            {
                isPaused = pPause;
                if (isPaused)
                {
                    onPause?.Invoke();
                }
                else
                {
                    onUnpause?.Invoke();
                }
            }
        }
    }
}