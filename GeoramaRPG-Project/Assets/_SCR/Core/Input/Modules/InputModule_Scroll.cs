using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;

namespace OliverLoescher.Input
{
	[System.Serializable]
    public class InputModule_Scroll : InputModule_Base
	{
		[Space, SerializeField, BoxGroup]
		private float input = 0.0f;
		public float Input => input;

		// Events
		[BoxGroup]
		public UnityEventsUtil.FloatEvent onChanged;

		public override void Initalize(InputAction pInputAction, Func<bool> pIsValid)
		{
			base.Initalize(pInputAction, pIsValid);
		}

		public override void Enable()
		{
			inputAction.performed += OnPerformed;
		}
		public override void Disable()
		{
			inputAction.performed -= OnPerformed;
		}
		public override void Clear() { }

		private void OnPerformed(InputAction.CallbackContext ctx)
		{
			if (!isValid.Invoke())
			{
				return;
			}
			input = ctx.ReadValue<float>();
			onChanged?.Invoke(input);
		}
	}
}
