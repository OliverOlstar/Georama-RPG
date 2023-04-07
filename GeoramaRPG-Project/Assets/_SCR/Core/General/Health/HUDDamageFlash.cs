using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OliverLoescher
{
   // [RequiredComponent(typeof(Health))]
    public class HUDDamageFlash : MonoBehaviour
    {
        [SerializeField] private Image image = null;
        private Color initalColor;

        [SerializeField] [Range(Util.NEARZERO, 1.0f)] private float flashSeconds = 0.1f;

        private Health health;

        private void Start()
        {
            initalColor = image.color;
            image.gameObject.SetActive(false);

            health = GetComponent<Health>();

            health.onValueLowered.AddListener(OnDamaged);
            health.onValueRaised.AddListener(OnHealed);
        }
        
        public void OnDamaged(float pValue, float pChange) { SetColor(Color.Lerp(health.damageColor, health.deathColor, 1 - ((float)health.Get() / (float)health.GetMax())), flashSeconds); }
        public void OnHealed(float pValue, float pChange) { SetColor(health.healColor, flashSeconds); }

        public void SetColor(Color pColor, float pSeconds)
        {
            image.color = pColor;
            image.gameObject.SetActive(true);

            CancelInvoke();
            Invoke(nameof(ResetColor), pSeconds);
        }

        private void ResetColor()
        {
            image.color = initalColor;
            image.gameObject.SetActive(false);
        }
    }
}