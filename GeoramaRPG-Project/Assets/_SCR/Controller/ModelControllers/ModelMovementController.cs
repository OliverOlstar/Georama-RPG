using OliverLoescher;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovementController : MonoBehaviour
{
	public enum RotationTarget
	{
		Velocity,
		Forward
	}

	[SerializeField]
	private CharacterValues values = null;
	[SerializeField]
	private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Default, MonoUtil.Priorities.ModelController);

	[Header("Tilt")]
	[SerializeField]
	private float tiltScalar = 1.0f;
	[SerializeField]
	private float tiltDampening = 5.0f;

	[Header("Rotation")]
	[SerializeField]
	private RotationTarget rotationTarget = default;
	[SerializeField]
	private float rotationDampening = 5.0f;

	private void Start()
	{
		updateable.Register(Tick);
	}

	private void OnDestroy()
	{
		updateable.Deregister();
	}

	private void Tick(float pDeltaTime)
	{
		DoTilt(pDeltaTime);
		DoRotation(pDeltaTime);
	}

	private void DoTilt(in float pDeltaTime)
	{
		if (tiltDampening <= 0.0f || values.InputMove == Vector3.zero)
		{
			return;
		}
		Vector3 eularAngles = transform.localEulerAngles;
		Quaternion toWorld = Quaternion.FromToRotation(Util.Horizontalize(transform.forward), values.Forward);
		Vector3 move = toWorld * values.InputMoveHorizontal;
		eularAngles.x = move.z * tiltScalar; // Forward
		eularAngles.z = -move.x * tiltScalar; // Right
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eularAngles), tiltDampening * pDeltaTime);
	}

	private void DoRotation(in float pDeltaTime)
	{
		if (rotationDampening <= 0.0f)
		{
			return;
		}
		Vector3 forward = rotationTarget == RotationTarget.Velocity ? Util.Horizontalize(values.Velocity, values.Up) : values.Forward;
		if (forward.sqrMagnitude <= Util.NEARZERO)
		{
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), rotationDampening * pDeltaTime);
	}
}
