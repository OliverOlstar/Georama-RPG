using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
    [DisallowMultipleComponent]
    public class CharacterValue : MonoBehaviour
    {
        public delegate void ValueChangeEvent(float pValue, float pChange);
        public delegate void ValueEvent();

        // [Tooltip("Zero counts as infinite")]
        [SerializeField, Min(0.0f)] protected float value = 100.0f;
        protected float maxValue = 100.0f;

        [SerializeField] protected bool doRecharge = false;
        [SerializeField] private bool canRunOut = true;
        [SerializeField, ShowIf("@canRunOut && doRecharge")] private bool canRechargeBackIn = true;

        [Header("Recharge")]
        [SerializeField, Min(0.0f), ShowIf("@doRecharge")] private float rechargeValueTo = 100.0f;
        [SerializeField, Min(0.0f), ShowIf("@doRecharge")] private float rechargeDelay = 1.0f;
        [SerializeField, Min(0.0f), ShowIf("@doRecharge")] private float rechargeRate = 20.0f;

        [Header("UI")]
        [SerializeField] protected List<BarValue> UIBars = new List<BarValue>();

        public ValueChangeEvent onValueChangedEvent;
        public ValueChangeEvent onValueLoweredEvent;
        public ValueChangeEvent onValueRaisedEvent;
        public ValueEvent onValueOutEvent;
        public ValueEvent onValueInEvent;
        public ValueChangeEvent onMaxValueChangedEvent;

        [FoldoutGroup("Events")] public UnityEventsUtil.DoubleFloatEvent onValueChanged;
        [FoldoutGroup("Events")] public UnityEventsUtil.DoubleFloatEvent onValueLowered;
        [FoldoutGroup("Events")] public UnityEventsUtil.DoubleFloatEvent onValueRaised;
        [FoldoutGroup("Events"), ShowIf("@canRunOut")] public UnityEvent onValueOut;
        [FoldoutGroup("Events"), ShowIf("@canRunOut && doRecharge && canRechargeBackIn")] public UnityEvent onValueIn;
        [FoldoutGroup("Events")] public UnityEventsUtil.DoubleFloatEvent onMaxValueChanged;

        public bool isOut { get; protected set; } = false;

        protected virtual void Start() 
        {
            maxValue = value;

            for (int i = 0; i < UIBars.Count; i++)
            {
                if (UIBars[i] == null)
                {
                    UIBars.RemoveAt(i);
                    i--;
                }
                else
                {
                    UIBars[i].InitValue(1.0f);
                }
            }
        }

        public float Get() { return value; }
        public void Modify(float pValue)
        {
            Set(value + pValue);
        }
        public void Set(float pValue)
        {
            float change = pValue - value;
            bool lower = pValue < value;
            value = Mathf.Clamp(pValue, 0.0f, maxValue);

            if (lower)
            {
                if (canRunOut && isOut == false && value == 0.0f)
                {
                    OnValueOut();
                }
                else
                {
                    OnValueLowered(value, change);
                }
            }
            else
            {
                if (canRechargeBackIn && isOut == true && value == maxValue)
                {
                    OnValueIn();
                }
                else
                {
                    OnValueRaised(value, change);
                }
            }

            if (doRecharge)
            {
                StopAllCoroutines();
                StartCoroutine(RechargeRoutine());
            }
            
            OnValueChanged(value, change);
        }

        public float GetMax() { return maxValue; }
        public void ModifyMax(float pValue)
        {
            SetMax(maxValue + pValue);
        }
        public void SetMax(float pValue)
        {
            float change = Mathf.Abs(maxValue - pValue);
            
            maxValue = pValue;
            value = Mathf.Clamp(pValue, 0.0f, maxValue);
            
            OnMaxValueChanged(maxValue, change);
        }

        private IEnumerator RechargeRoutine()
        {
            yield return new WaitForSeconds(rechargeDelay);

            while (value < Mathf.Min(maxValue, rechargeValueTo))
            {
                value += Time.deltaTime * rechargeRate;
                value = Mathf.Min(value, maxValue);

                foreach (BarValue bar in UIBars)
                    bar.SetValue(value / maxValue);

                yield return null;
            }

            if (canRechargeBackIn && isOut)
                OnValueIn();
        }

        public virtual void OnValueChanged(float pValue, float pChange)
        {
            foreach (BarValue bar in UIBars)
                bar.SetValue(pValue / maxValue);

            onValueChangedEvent?.Invoke(pValue, pChange);
            onValueChanged?.Invoke(pValue, pChange);
        }

        public virtual void OnValueLowered(float pValue, float pChange)
        {
            onValueLoweredEvent?.Invoke(pValue, pChange);
            onValueLowered?.Invoke(pValue, pChange);
        }

        public virtual void OnValueRaised(float pValue, float pChange)
        {
            onValueRaisedEvent?.Invoke(pValue, pChange);
            onValueRaised?.Invoke(pValue, pChange);
        }

        public virtual void OnValueOut()
        {
            isOut = true;
            foreach (BarValue bar in UIBars)
                bar.SetToggled(true);

            onValueOutEvent?.Invoke();
            onValueOut?.Invoke();
        }

        public virtual void OnValueIn()
        {
            isOut = false;
            foreach (BarValue bar in UIBars)
                bar.SetToggled(false);

            onValueInEvent?.Invoke();
            onValueIn?.Invoke();
        }

        public virtual void OnMaxValueChanged(float pMaxValue, float pChange)
        {
            foreach (BarValue bar in UIBars)
                bar.SetValue(value / pMaxValue);
            
            onMaxValueChangedEvent?.Invoke(pMaxValue, pChange);
            onMaxValueChanged?.Invoke(pMaxValue, pChange);
        }
    }
}