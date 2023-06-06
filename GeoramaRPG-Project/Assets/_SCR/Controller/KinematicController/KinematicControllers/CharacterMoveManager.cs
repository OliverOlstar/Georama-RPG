using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CharacterRigidbodyController))]
public class CharacterMoveManager : CharacterBehaviour
{
	[System.Serializable]
	public class MoveController
	{
		private Character m_Character;
		public Character Character => m_Character;

		public void Initalize(Character pCharacter)
		{
			m_Character = pCharacter;
			OnInitialize();
		}

		protected virtual void OnInitialize() { }
		public virtual void OnDestroy() { }
		public virtual void Tick(float pDeltaTime) { }
		public virtual void OnGroundStateChange(CharacterOnGround.State pState) { }
		public virtual void OnMoveStateChange(CharacterMoveState.State pState) { }
	}

	[SerializeField]
	private CharacterMoveDefault m_MoveDefault = new CharacterMoveDefault();
	[SerializeField]
	private CharacterMoveJump m_MoveJump = new CharacterMoveJump();
	[SerializeField]
	private CharacterMoveDodge m_MoveDodge = new CharacterMoveDodge();

	[FoldoutGroup("Events"), SerializeField]
	private UnityEvent m_OnJumpEvent = new UnityEvent();

	private IEnumerable<MoveController> MoveControllers()
	{
		yield return m_MoveDefault;
		yield return m_MoveJump;
		yield return m_MoveDodge;
	}
	public Vector3 Velocity => m_Rigidbody.Velocity;
	public bool IsGrounded => Character.OnGround.IsGrounded;

	public Vector3 Forward => m_MoveDefault.Forward();
	public Vector3 Right => m_MoveDefault.Right();

	public UnityEvent OnJumpEvent => m_OnJumpEvent;

	private CharacterRigidbodyController m_Rigidbody;

	protected override void OnInitalize()
	{
		m_Rigidbody = GetComponent<CharacterRigidbodyController>();
		Character.OnGround.OnStateChanged.AddListener(OnGroundStateChange);
		Character.MoveState.OnStateChangeEvent.AddListener(OnMoveStateChange);
		Character.Input.Jump.onPerformed.AddListener(DoJump);

		foreach (MoveController controller in MoveControllers())
		{
			controller.Initalize(Character);
		}
	}

	protected override void OnDestroyed()
	{
		Character.OnGround.OnStateChanged.RemoveListener(OnGroundStateChange);
		Character.MoveState.OnStateChangeEvent.RemoveListener(OnMoveStateChange);
		Character.Input.Jump.onPerformed.RemoveListener(DoJump);

		foreach (MoveController controller in MoveControllers())
		{
			controller.OnDestroy();
		}
	}

	private void OnGroundStateChange(CharacterOnGround.State pState)
	{
		foreach (MoveController controller in MoveControllers())
		{
			controller.OnGroundStateChange(pState);
		}
	}

	private void OnMoveStateChange(CharacterMoveState.State pState)
	{
		foreach (MoveController controller in MoveControllers())
		{
			controller.OnMoveStateChange(pState);
		}
	}

	private void DoJump()
	{
		if (m_MoveJump.TryDoJump())
		{
			m_OnJumpEvent.Invoke();
		}
	}

	protected override void Tick(float pDeltaTime)
	{
		foreach (MoveController controller in MoveControllers())
		{
			controller.Tick(pDeltaTime);
		}
	}

	public void Move(Vector3 pMove) => m_Rigidbody.Move(pMove);
	public void ModifyVelocity(Vector3 pVelocity) => m_Rigidbody.ModifyVelocity(pVelocity);
	public void SetVelocity(Vector3 pVelocity) => m_Rigidbody.SetVelocity(pVelocity);
	public void SetDrag(float pDrag) => m_Rigidbody.SetDrag(pDrag);
}