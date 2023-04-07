using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Wait")]
public class UnimateWait : UnimateTween<UnimateWait, UnimateWait.Player>
{
	[SerializeField]
	private float m_Duration = 1.0f;
	public override float Duration => m_Duration;
	public override bool Loop => false;
	public override bool UpdateBeforeStart => false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateWait>
	{

	}
}
