using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Wait Forever")]
public class UnimateWaitForever : UnimateTween<UnimateWaitForever, UnimateWaitForever.Player>
{
	public override float Duration => 1.0f;
	public override bool Loop => true;
	public override bool UpdateBeforeStart => false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateWaitForever>
	{

	}
}
