using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
    public class DamageReciever : MonoBehaviour, IDamageable
    {
        [SerializeField] private IDamageable parent = null;

        [DisableInPlayMode] [SerializeField] private GameObject parentObject = null;
        [SerializeField] private float damageMultiplier = 1.0f;

        private void Awake() 
        {
            if (parentObject == null)
            {
                parent = transform.parent.GetComponentInParent<IDamageable>();

                if (parent == null)
                {
                    Debug.LogError("[DamageReciever] Couldn't find IDamagable through GetComponentInParent, destroying self", gameObject);
                    Destroy(this);
                }
            }
            else
            {
                parent = parentObject.GetComponent<IDamageable>();

                if (parent == null)
                {
                    Debug.LogError("[DamageReciever] Couldn't find IDamagable on parentObject, destroying self", gameObject);
                    Destroy(this);
                }
            }
        }

        void IDamageable.Damage(float pValue, GameObject pAttacker, Vector3 pPoint, Vector3 pDirection, Color pColor)
        {
            parent.Damage(DamageMultipler(pValue), pAttacker, pPoint, pDirection, pColor);
        }
        void IDamageable.Damage(float pValue, GameObject pAttacker, Vector3 pPoint, Vector3 pDirection)
        {
            parent.Damage(DamageMultipler(pValue), pAttacker, pPoint, pDirection);
        }
        private float DamageMultipler(float pValue) => pValue * damageMultiplier;

        GameObject IDamageable.GetGameObject()
        {
            if (parent == null)
            {
                return gameObject;
            }
            return parent.GetGameObject();
        }
        IDamageable IDamageable.GetParentDamageable()
        {
            if (parent == null)
            {
                return this;
            }
            return parent.GetParentDamageable();
        }
        SOTeam IDamageable.GetTeam()
		{
            if (parent == null)
            {
                return null;
            }
            return parent.GetTeam();
        }
    }
}