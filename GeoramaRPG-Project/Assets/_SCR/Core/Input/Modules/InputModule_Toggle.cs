using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

namespace OliverLoescher.Input
{
	[System.Serializable]
    public class InputModule_Toggle : InputModule_Base
	{
		[SerializeField, BoxGroup]
		private bool isToggle = false;
		public bool IsToggle => isToggle;
		[Space, HideInEditorMode, SerializeField, BoxGroup]
		private bool input = false;
		public bool Input => input;

		// Events
		[BoxGroup]
		public UnityEventsUtil.BoolEvent onChanged;
		[BoxGroup]
		public UnityEvent onPerformed;
		[BoxGroup]
		public UnityEvent onCanceled;

		public override void Enable()
		{
			inputAction.performed += OnPerformed;
			inputAction.canceled += OnCanceled;
		}
		public override void Disable()
		{
			inputAction.performed -= OnPerformed;
			inputAction.canceled -= OnCanceled;
		}
		public override void Clear()
		{
			Set(false);
		}

		private void OnPerformed(InputAction.CallbackContext ctx)
		{
			if (!isValid.Invoke())
			{
				return;
			}
			// True if not toggle || not currently pressed
			// False if toggle && currently pressed
			Set(isToggle == false || input == false);
		}
		private void OnCanceled(InputAction.CallbackContext ctx)
		{
			if (!isValid.Invoke())
			{
				return;
			}
			if (!isToggle)
			{
				Set(false);
			}
		}

		private void Set(bool pValue)
		{
			if (input != pValue)
			{
				input = pValue;

				// Events
				onChanged.Invoke(input);
				if (input)
				{
					onPerformed?.Invoke();
				}
				else
				{
					onCanceled?.Invoke();
				}
			}
		}
	}
}
