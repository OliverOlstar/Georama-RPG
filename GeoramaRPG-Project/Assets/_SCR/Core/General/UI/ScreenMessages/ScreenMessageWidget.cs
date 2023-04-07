using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace OliverLoescher
{
    public class ScreenMessageWidget : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text = null;

        private Queue<ScreenMessage.Message> messages = new Queue<ScreenMessage.Message>();
        private ScreenMessage.Message activeMessage = null;

        private void OnEnable()
        {
            text.text = string.Empty;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void QueueMessage(ScreenMessage.Message pMessage)
        {
            if (activeMessage != null)
            {
                messages.Enqueue(pMessage);
                return;
            }
            activeMessage = pMessage;
            StartCoroutine(DisplayRoutine());
        }

        private IEnumerator DisplayRoutine()
        {
            text.text = activeMessage.message;

            Color color = text.color;
            color.a = 0.0f;
            text.color = color;
            while (color.a < 1.0f)
            {
                yield return null;
                color.a += Time.deltaTime * 5.0f;
                text.color = color;
            }

            yield return new WaitForSeconds(activeMessage.seconds);

            while (color.a > 0.0f)
            {
                yield return null;
                color.a -= Time.deltaTime * 5.0f;
                text.color = color;
            }

            text.text = string.Empty;

            if (messages.Count > 0)
            {
                activeMessage = messages.Dequeue();
                StartCoroutine(DisplayRoutine());
                yield break;
            }
            activeMessage = null;
        }
    }
}
