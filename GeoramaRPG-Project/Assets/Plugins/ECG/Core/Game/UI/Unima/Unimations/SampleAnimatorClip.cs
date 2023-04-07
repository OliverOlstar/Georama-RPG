
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Core
{
	[RequireComponent(typeof(Animator))]
	public class SampleAnimatorClip : MonoBehaviour
	{
		private PlayableGraph m_Graph = default;
		private AnimationPlayableOutput m_Output = default;
		private AnimationClipPlayable m_Playable = default;
		private Animator m_Animator = null;
		private AnimationClip m_Clip = null;

		private void Awake()
		{
			m_Animator = Core.Util.GetOrAddComponent<Animator>(gameObject);
			if (m_Animator.runtimeAnimatorController != null)
			{
				Debug.LogWarning($"SampleAnimatorClip.Awake() Unima is stomping AnimatorController {m_Animator.runtimeAnimatorController.name} attached to {name} Animator, " +
					$"a GameObject cannot play an AnimatorController and a UnimateAnimationClip you must choose one system or the other");
				m_Animator.runtimeAnimatorController = null;
			}
		}

		public void OnDisable()
		{
			if (m_Graph.IsValid())
			{
				m_Graph.Destroy();
			}
			m_Clip = null;
		}

		public void SetClip(AnimationClip clip)
		{
			if (clip == null)
			{
				return;
			}
			if (!m_Graph.IsValid())
			{
				m_Graph = PlayableGraph.Create();
				m_Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
				//GraphVisualizerClient.Show(m_Graph);
				m_Output = AnimationPlayableOutput.Create(m_Graph, "Animation", m_Animator);
			}
			else
			{
				m_Playable.Destroy();
			}
			m_Clip = clip;
			m_Playable = AnimationClipPlayable.Create(m_Graph, m_Clip);
			m_Playable.Pause();
			m_Output.SetSourcePlayable(m_Playable);
		}

		public void SampleClip(AnimationClip clip, float time)
		{
			// Make sure you're sampling the clip you think you are
			if (m_Clip == null || m_Clip.GetInstanceID() != clip.GetInstanceID())
			{
				return;
			}
			m_Playable.SetTime(time);
			m_Graph.Evaluate();
		}
	}
}
