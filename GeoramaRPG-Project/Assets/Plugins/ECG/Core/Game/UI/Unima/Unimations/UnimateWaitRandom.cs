using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Wait Random")]
public class UnimateWaitRandom : Unimate<UnimateWaitRandom, UnimateWaitRandom.Player>
{
	[SerializeField]
	private float m_MinDuration = 1.0f;
	[SerializeField]
	private float m_MaxDuration = 1.0f;

	public override UnimaDurationType GetEditorDuration(out float seconds)
	{
		seconds = m_MaxDuration;
		return UnimaDurationType.Fixed;
	}

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (m_MinDuration > m_MaxDuration)
		{
			return "Min Duration cannot be greater than Max Duration";
		}
		return null;
	}

	public class Player : UnimaPlayer<UnimateWaitRandom>
	{
		private float m_Duration = -1.0f;
		protected override void OnStart()
		{
			m_Duration = Random.Range(Animation.m_MinDuration, Animation.m_MaxDuration);
		}
		protected override bool OnUpdate(float deltaTime)
		{
			m_Duration -= deltaTime;
			return m_Duration > 0.0f;
		}
	}
}
