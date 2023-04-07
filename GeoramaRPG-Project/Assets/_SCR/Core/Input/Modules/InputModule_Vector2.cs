using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

namespace OliverLoescher.Input
{
	[System.Serializable]
    public class InputModule_Vector2 : InputModule_Base
	{
		[BoxGroup, HideInEditorMode, SerializeField]
		private Vector2 input = new Vector2();
		public Vector2 Input => input;
		public Vector3 InputHorizontal => new Vector3(input.x, 0.0f, input.y);

		[BoxGroup, SerializeField] 
		private Vector2 scalar = Vector2.one;
		[BoxGroup, SerializeField]
		private bool normalize = false;
		[BoxGroup, SerializeField]
		private bool invertY = false;

		[BoxGroup]
		public UnityEventsUtil.Vector2Event onChanged;

		public override void Enable()
		{
			inputAction.performed += OnPerformed;
			inputAction.canceled += OnPerformed;
		}
		public override void Disable()
		{
			inputAction.performed -= OnPerformed;
			inputAction.canceled += OnPerformed;
		}
		public override void Clear()
		{
			input = Vector2.zero;
			onChanged?.Invoke(input);
		}

		private void OnPerformed(InputAction.CallbackContext ctx)
		{
			if (!isValid.Invoke())
			{
				return;
			}
			input = ctx.ReadValue<Vector2>();
			input.x *= scalar.x;
			input.y *= scalar.y * (invertY ? -1 : 1);
			if (normalize)
			{
				input.Normalize();
			}
			onChanged?.Invoke(input);
		}
	}
}
