using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    public class ScreenMessage : Singleton<ScreenMessage>, ISingleton
    {
        public class Message
        {
            public string message { get; private set; }
            public float seconds { get; private set; }

            public Message(string pMessage, float pSeconds)
            {
                message = pMessage;
                seconds = pSeconds;
            }
        }

        private const string CANVASPREFABPATH = "ScreenMessages";

        private static ScreenMessageCanvas canvas = null;
        private static ResourceRequest loadCanvas = null;

        void ISingleton.OnAccessed()
        {
            if (canvas == null && loadCanvas == null)
            {
                MonoUtil.Start(Initalize());
            }
        }

        private static IEnumerator Initalize()
        {
            Log($"Loading Asset: {CANVASPREFABPATH}");
            loadCanvas = Resources.LoadAsync(CANVASPREFABPATH);
            while (!loadCanvas.isDone)
            {
                yield return null;
            }
            canvas = (MonoUtil.Instantiate(loadCanvas.asset) as GameObject).GetComponent<ScreenMessageCanvas>();
            loadCanvas = null;
        }

        private IEnumerator LargeMessageInternal(string pMessage, float pSeconds)
        {
            while (canvas == null)
            {
                yield return null;
            }
            canvas.LargeMessage.QueueMessage(new Message(pMessage, pSeconds));
        }
        public IEnumerator SmallMessageInternal(string pMessage, float pSeconds)
        {
            while (canvas == null)
            {
                yield return null;
            }
            canvas.SmallMessage.QueueMessage(new Message(pMessage, pSeconds));
        }

        public static void LargeMessage(string pMessage, float pSeconds) => MonoUtil.Start(Instance.LargeMessageInternal(pMessage, pSeconds));
        public static void SmallMessage(string pMessage, float pSeconds) => MonoUtil.Start(Instance.SmallMessageInternal(pMessage, pSeconds));
    }
}
