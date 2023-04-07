using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using OliverLoescher;

namespace OliverLoescher
{
    [RequireComponent(typeof(Health))]
    public class DamageFlash : MonoBehaviour
    {
        private Health health = null;

        [SerializeField] [DisableInPlayMode] private Renderer[] renderers;
        private Color[] initalColors;

        [Header("Time")]
        [SerializeField] [Range(Util.NEARZERO, 1.0f)] private float flashSeconds = 0.1f;
        [SerializeField] [Range(Util.NEARZERO, 5.0f)] private float flashDeathSeconds = 0.1f;

        private void Start()
        {
            health = GetComponent<Health>();

            health.onValueLowered.AddListener(OnDamaged);
            health.onValueRaised.AddListener(OnHealed);
            health.onValueOut.AddListener(OnDeath);

            initalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    initalColors[i] = renderers[i].material.color;
                }
            }
        }
        
        public void OnDamaged(float pValue, float pChange) { SetColor(health.damageColor, flashSeconds); }
        public void OnHealed(float pValue, float pChange) { SetColor(health.healColor, flashSeconds); }
        public void OnDeath() { SetColor(health.deathColor, flashDeathSeconds); }

        public void SetColor(Color pColor, float pSeconds)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color = pColor;
                }
            }

            CancelInvoke();
            Invoke(nameof(ResetColor), pSeconds);
        }

        private void ResetColor()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = initalColors[i];
            }
        }
    }
}
