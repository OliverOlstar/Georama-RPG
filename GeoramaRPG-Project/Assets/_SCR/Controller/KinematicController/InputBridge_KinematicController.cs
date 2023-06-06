using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using OliverLoescher.Input;
using OliverLoescher;

public class InputBridge_KinematicController : InputBridge_Base
{
	[SerializeField]
	private InputModule_Vector2 moveInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Toggle sprintInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle walkInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle lockOnInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle crouchInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Trigger jumpInput = new InputModule_Trigger();
	[SerializeField]
	private InputModule_Trigger dodgeInput = new InputModule_Trigger();
	[SerializeField]
	private InputModule_Toggle primaryInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle secondaryInput = new InputModule_Toggle();

	public InputModule_Vector2 Move => moveInput;
	public InputModule_Toggle Sprint => sprintInput;
	public InputModule_Toggle Walk => walkInput;
	public InputModule_Toggle LockOn => lockOnInput;
	public InputModule_Toggle Crouch => crouchInput;
	public InputModule_Trigger Jump => jumpInput;
	public InputModule_Trigger Dodge => dodgeInput;
	public InputModule_Toggle Primary => primaryInput;
	public InputModule_Toggle Secondary => secondaryInput;

	public override InputActionMap Actions => OliverLoescher.InputSystem.Instance.FPS.Get();
	public override IEnumerable<IInputModule> GetAllInputModules()
	{
		yield return moveInput;
		yield return sprintInput;
		yield return walkInput;
		yield return lockOnInput;
		yield return crouchInput;
		yield return jumpInput;
		yield return dodgeInput;
		yield return primaryInput;
		yield return secondaryInput;
	}

	protected override void Awake()
	{
		moveInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Move, IsValid);
		sprintInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Sprint, IsValid);
		walkInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Walk, IsValid);
		lockOnInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Strafe, IsValid);
		crouchInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Crouch, IsValid);
		jumpInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Jump, IsValid);
		dodgeInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Dodge, IsValid);
		primaryInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Primary, IsValid);
		secondaryInput.Initalize(OliverLoescher.InputSystem.Instance.FPS.Secondary, IsValid);

		sprintInput.onPerformed.AddListener(OnMotionActionPerformed);
		jumpInput.onPerformed.AddListener(OnMotionActionPerformed);
		dodgeInput.onPerformed.AddListener(OnMotionActionPerformed);

		base.Awake();
	}

	protected override void OnDestroy()
	{
		sprintInput.onPerformed.RemoveListener(OnMotionActionPerformed);
		jumpInput.onPerformed.RemoveListener(OnMotionActionPerformed);
		dodgeInput.onPerformed.RemoveListener(OnMotionActionPerformed);

		base.OnDestroy();
	}

	private void OnMotionActionPerformed()
	{
		if (crouchInput.IsToggle)
		{
			crouchInput.Clear();
		}
		if (walkInput.IsToggle)
		{
			walkInput.Clear();
		}
	}

	public void ClearLockOnIfIsToggle()
	{
		if (lockOnInput.IsToggle)
		{
			lockOnInput.Clear();
		}
	}
}
