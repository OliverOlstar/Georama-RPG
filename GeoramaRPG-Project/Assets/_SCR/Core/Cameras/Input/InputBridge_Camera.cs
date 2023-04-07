using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using OliverLoescher.Input;

namespace OliverLoescher
{
	public class InputBridge_Camera : InputBridge_Base
	{
		[SerializeField]
		private InputModule_Vector2Update lookInput = new InputModule_Vector2Update();
		[SerializeField]
		private InputModule_Vector2 lookDeltaInput = new InputModule_Vector2();
		[SerializeField]
		private InputModule_Scroll zoomInput = new InputModule_Scroll();

		public InputModule_Vector2Update Look => lookInput;
		public InputModule_Vector2 LookDelta => lookDeltaInput;
		public InputModule_Scroll Zoom => zoomInput;

		public override InputActionMap Actions => InputSystem.Instance.Camera.Get();
		public override IEnumerable<IInputModule> GetAllInputModules()
		{
			yield return lookInput;
			yield return lookDeltaInput;
			yield return zoomInput;
		}

		protected override void Awake()
		{
			lookInput.Initalize(InputSystem.Instance.Camera.Look, IsValid);
			lookDeltaInput.Initalize(InputSystem.Instance.Camera.LookDelta, IsValid);
			zoomInput.Initalize(InputSystem.Instance.Camera.Zoom, IsValid);

			base.Awake();
		}

		protected override void OnEnable()
		{
			Cursor.lockState = CursorLockMode.Locked;

			base.OnEnable();
		}

		protected override void OnDisable()
		{
			Cursor.lockState = CursorLockMode.None;

			base.OnDisable();
		}
	}
}