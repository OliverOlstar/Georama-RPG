using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using OliverLoescher.Input;
using OliverLoescher;

public class InputBridge_KinematicController : InputBridge_Base
{
	[SerializeField]
	private InputModule_Vector2Update lookInput = new InputModule_Vector2Update();
	[SerializeField]
	private InputModule_Vector2 lookDeltaInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Vector2 moveInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Toggle sprintInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle crouchInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Trigger jumpInput = new InputModule_Trigger();
	[SerializeField]
	private InputModule_Toggle primaryInput = new InputModule_Toggle();

	public InputModule_Vector2Update Look => lookInput;
	public InputModule_Vector2 LookDelta => lookDeltaInput;
	public InputModule_Vector2 Move => moveInput;
	public InputModule_Toggle Sprint => sprintInput;
	public InputModule_Toggle Crouch => crouchInput;
	public InputModule_Trigger Jump => jumpInput;
	public InputModule_Toggle Primary => primaryInput;

	public override InputActionMap Actions => OliverLoescher.InputSystem.Instance.FPS.Get();
	public override IEnumerable<IInputModule> GetAllInputModules()
	{
		yield return lookInput;
		yield return lookDeltaInput;
		yield return moveInput;
		yield return sprintInput;
		yield return crouchInput;
		yield return jumpInput;
		yield return primaryInput;
	}

	protected override void Awake()
	{
		lookInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.CameraMove, IsValid);
		lookDeltaInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.CameraMoveDelta, IsValid);
		moveInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Move, IsValid);
		sprintInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Sprint, IsValid);
		crouchInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Crouch, IsValid);
		jumpInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Jump, IsValid);
		primaryInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Primary, IsValid);

		jumpInput.onPerformed.AddListener(OnJumpPerformed);

		base.Awake();
	}

	protected override void OnDestroy()
	{
		jumpInput.onPerformed.RemoveListener(OnJumpPerformed);

		base.OnDestroy();
	}

	public void ClearCrouchIfToggle()
	{
		if (crouchInput.IsToggle)
		{
			crouchInput.Clear();
		}
	}

	private void OnJumpPerformed()
	{
		crouchInput.Clear();
	}
}
