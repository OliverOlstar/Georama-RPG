
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Animator Trigger")]
public class UnimateAnimatorTrigger : Unimate<UnimateAnimatorTrigger, UnimateAnimatorTrigger.Player>
{
	[SerializeField]
	private string m_TriggerName = string.Empty;

	[SerializeField]
	private bool m_Recursive = true;

	public override UnimaDurationType GetEditorDuration(out float seconds)
	{
		seconds = 0.0f;
		return UnimaDurationType.Fixed;
	}

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (string.IsNullOrEmpty(m_TriggerName))
		{
			return "Trigger name cannot be empty";
		}
		bool valid = m_Recursive ?
			gameObject.GetComponentsInChildren<Animator>().Length > 0 :
			gameObject.GetComponent<Animator>() != null;
		return valid ? null : $"Requires Animator component";
	}

	public class Player : UnimaPlayer<UnimateAnimatorTrigger>
	{
		private List<Animator> m_Components = new List<Animator>();
		private int m_StateHash = 0;

		protected override void OnInitialize()
		{
			if (Animation.m_Recursive)
			{
				GameObject.GetComponentsInChildren(m_Components);
			}
			else
			{
				GameObject.GetComponents(m_Components);
			}
			m_StateHash = Animator.StringToHash(Animation.m_TriggerName);
			for (int i = m_Components.Count - 1; i >= 0; i--)
			{
				Animator animator = m_Components[i];
				AnimatorControllerParameter[] parameters = animator.parameters;
				bool found = false;
				for (int j = 0; j < parameters.Length; j++)
				{
					if (parameters[j].nameHash == m_StateHash)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					m_Components.RemoveAt(i);
				}
			}
		}

		protected override void OnStart()
		{
			foreach (Animator animator in m_Components)
			{
				if (animator != null)
				{
					animator.SetTrigger(m_StateHash);
				}
			}
		}

		protected override bool OnUpdate(float deltaTime) { return false; }
	}
}
