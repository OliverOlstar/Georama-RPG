using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using OliverLoescher.Input;

namespace OliverLoescher.Camera
{
	public class InputBridge_Spectator : InputBridge_Base
	{
		[SerializeField]
		private InputModule_Vector2Update lookInput = new InputModule_Vector2Update();
		[SerializeField]
		private InputModule_Vector2 lookDeltaInput = new InputModule_Vector2();
		[SerializeField]
		private InputModule_Vector2 moveInput = new InputModule_Vector2();
		[SerializeField]
		private InputModule_Scroll moveVerticalInput = new InputModule_Scroll();
		[SerializeField]
		private InputModule_Scroll zoomInput = new InputModule_Scroll();
		[SerializeField]
		private InputModule_Toggle modeInput = new InputModule_Toggle();
		[SerializeField]
		private InputModule_Toggle targetInput = new InputModule_Toggle();

		public InputModule_Vector2Update Look => lookInput;
		public InputModule_Vector2 LookDelta => lookDeltaInput;
		public InputModule_Vector2 Move => moveInput;
		public InputModule_Scroll MoveVertical => moveVerticalInput;
		public InputModule_Scroll Zoom => zoomInput;
		public InputModule_Toggle Mode => modeInput;
		public InputModule_Toggle Target => targetInput;

		public override InputActionMap Actions => InputSystem.Instance.SpectatorCamera.Get();
		public override IEnumerable<IInputModule> GetAllInputModules()
		{
			yield return lookInput;
			yield return lookDeltaInput;
			yield return moveInput;
			yield return moveVerticalInput;
			yield return zoomInput;
			yield return modeInput;
			yield return targetInput;
		}

		protected override void Awake()
		{
			lookInput.Initalize(InputSystem.Instance.SpectatorCamera.Look, IsValid);
			lookDeltaInput.Initalize(InputSystem.Instance.SpectatorCamera.LookDelta, IsValid);
			moveInput.Initalize(InputSystem.Instance.SpectatorCamera.MoveHorizontal, IsValid);
			moveVerticalInput.Initalize(InputSystem.Instance.SpectatorCamera.MoveVertical, IsValid);
			zoomInput.Initalize(InputSystem.Instance.SpectatorCamera.Zoom, IsValid);
			modeInput.Initalize(InputSystem.Instance.SpectatorCamera.ModeToggle, IsValid);
			targetInput.Initalize(InputSystem.Instance.SpectatorCamera.TargetToggle, IsValid);

			base.Awake();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Cursor.lockState = CursorLockMode.Locked;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Cursor.lockState = CursorLockMode.None;
		}
	}
}