using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    [RequireComponent(typeof(Health))]
    public class HealthDamageNumbers : MonoBehaviour
    {
        [SerializeField] private new Transform transform = null;
        [SerializeField] private Vector3 offset = Vector3.up * 2;

        private Health health;

        private void Awake() 
        {
            if (transform == null)
                transform = gameObject.transform;

            health = GetComponent<Health>();

            health.onValueLowered.AddListener(OnDamaged);
            health.onValueRaised.AddListener(OnHealed);
            health.onValueOut.AddListener(OnDeath);
        }

        public void OnDamaged(float pValue, float pChange)
        {
            DisplayNumber(Mathf.CeilToInt(pChange), health.damageColor);
        }

        public void OnHealed(float pValue, float pChange)
        {
            DisplayNumber(Mathf.CeilToInt(pChange), health.healColor);
        }

        public void OnDeath()
        {
            DisplayNumber("DEATH", health.deathColor);
        }

        public void DisplayNumber(int pValue, Color pColor)
        {
            DisplayNumber((Mathf.Abs(pValue)).ToString(), pColor);
        }

        public void DisplayNumber(string pText, Color pColor)
        {
            DamageNumbers.Instance.DisplayNumber(pText, transform.position + offset, pColor);
        }
    }
}