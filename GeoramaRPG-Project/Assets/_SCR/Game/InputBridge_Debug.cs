using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using OliverLoescher.Input;

public class InputBridge_Debug : InputBridge_Base
{
		[SerializeField]
		private InputModule_Trigger cheatMenuInput = new InputModule_Trigger();

		public InputModule_Trigger CheatMenu => cheatMenuInput;

		public override InputActionMap Actions => OliverLoescher.InputSystem.Instance.Debug.Get();
		public override IEnumerable<IInputModule> GetAllInputModules()
		{
			yield return cheatMenuInput;
		}

		protected override void Awake()
		{
			cheatMenuInput.Initalize(OliverLoescher.InputSystem.Instance.Debug.CheatMenu, IsValid);
			base.Awake();
		}
}
