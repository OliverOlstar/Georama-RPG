using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterMoveJump : CharacterMoveManager.MoveController
{
	[SerializeField, Min(0.0f)]
	private float m_JumpForce = 6.0f;
	[SerializeField, Min(0)]
	private int m_AirJumpCount = 0;
	
	private int m_RemainingJumps = 0;
	
	public bool TryDoJump()
	{
		if (Character.OnGround.IsGrounded || m_RemainingJumps-- > 0)
		{
			Vector3 velocity = Character.Controller.Velocity;
			velocity.y = m_JumpForce;
			Character.Controller.SetVelocity(velocity);
			return true;
		}
		return false;
	}

	public override void OnGroundStateChange(CharacterOnGround.State pState)
	{
		if (pState == CharacterOnGround.State.Grounded)
		{
			m_RemainingJumps = m_AirJumpCount;
		}
	}
}
