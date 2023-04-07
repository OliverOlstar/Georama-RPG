using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicForceControllerValues : CharacterValues
{
	[SerializeField]
	private KinematicForceController controller = null;
	[SerializeField]
	private InputBridge_KinematicController input = null;

	public override Vector3 Forward => controller.Forward();
	public override Vector3 Right => controller.Right();
	public override Vector3 Up => controller.Capsule.Up;

	public override Vector3 Velocity => controller.Velocity;

	public override Vector3 InputMove => input.Move.Input;
	public override Vector3 InputMoveHorizontal => input.Move.InputHorizontal;
}