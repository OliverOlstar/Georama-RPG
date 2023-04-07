using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

namespace OliverLoescher.Input
{
	[System.Serializable]
    public class InputModule_Trigger : InputModule_Base
	{
		[BoxGroup]
		public UnityEvent onPerformed;

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
			onPerformed?.Invoke();
		}
	}
}
