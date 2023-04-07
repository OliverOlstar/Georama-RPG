using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using OliverLoescher.Input;

namespace OliverLoescher
{
	public class InputBridge_EagleEye : InputBridge_Base
	{
		[SerializeField]
		private InputModule_Vector2 moveInput = new InputModule_Vector2();
		[SerializeField]
		private InputModule_ToggledInput<InputModule_Vector2> moveDeltaInput = new InputModule_ToggledInput<InputModule_Vector2>();
		[SerializeField]
		private InputModule_Scroll zoomInput = new InputModule_Scroll();
		[SerializeField]
		private InputModule_Scroll rotateInput = new InputModule_Scroll();

		public InputModule_Vector2 Move => moveInput;
		public InputModule_ToggledInput<InputModule_Vector2> MoveDelta => moveDeltaInput;
		public InputModule_Scroll Zoom => zoomInput;
		public InputModule_Scroll Rotate => rotateInput;

		public override InputActionMap Actions => InputSystem.Instance.EagleEye.Get();
		public override IEnumerable<IInputModule> GetAllInputModules()
		{
			yield return moveInput;
			yield return moveDeltaInput;
			yield return zoomInput;
			yield return rotateInput;
		}

		protected override void Awake()
		{
			moveInput.Initalize(InputSystem.Instance.EagleEye.Move, IsValid);
			moveDeltaInput.Initalize(InputSystem.Instance.EagleEye.MoveDelta, InputSystem.Instance.EagleEye.MoveDeltaButton, IsValid);
			zoomInput.Initalize(InputSystem.Instance.EagleEye.Zoom, IsValid);
			rotateInput.Initalize(InputSystem.Instance.EagleEye.Rotate, IsValid);

			base.Awake();
		}

		protected override void OnEnable()
		{
			Cursor.lockState = CursorLockMode.Confined;

			base.OnEnable();
		}

		protected override void OnDisable()
		{
			Cursor.lockState = CursorLockMode.None;

			base.OnDisable();
		}
	}
}