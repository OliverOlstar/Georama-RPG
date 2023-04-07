using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;

namespace OliverLoescher.Input
{
	[System.Serializable]
    public class InputModule_ToggledInput<T> : IInputModule where T : InputModule_Base, new()
	{
		[SerializeField]
		private InputModule_Toggle toggle = new InputModule_Toggle();
		[SerializeField]
		private T value = new T();

		public InputModule_Toggle Toggle => toggle;
		public T Value => value;

		public void Initalize(InputAction pInputAction, InputAction pToggleInputAction, Func<bool> pIsValid)
		{
			toggle.Initalize(pToggleInputAction, pIsValid);
			value.Initalize(pInputAction, pIsValid);
		}

		public void Enable() // IInputModule
		{
			toggle.Enable();

			toggle.onPerformed.AddListener(EnableValue);
			toggle.onCanceled.AddListener(DisableValue);
		}

		public void Disable() // IInputModule
		{
			toggle.Disable();
			value.Disable();

			toggle.onPerformed.RemoveListener(EnableValue);
			toggle.onCanceled.RemoveListener(DisableValue);
		}

		public void Clear() // IInputModule
		{
			toggle.Clear();
			value.Clear();
		}

		public void Update(in float pDeltaTime) { } // IInputModule

		private void DisableValue()
		{
			value.Disable();
			value.Clear();
		}
		private void EnableValue()
		{
			value.Enable();
		}
	}
}
