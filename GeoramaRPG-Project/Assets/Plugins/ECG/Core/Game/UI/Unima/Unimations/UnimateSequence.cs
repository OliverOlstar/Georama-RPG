
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Sequence")]
public class UnimateSequence : Unimate<UnimateSequence, UnimateSequence.Player>
{
	[SerializeField]
	private UnimaSet m_AnimationSet = new UnimaSet();
	public UnimaSet Set => m_AnimationSet;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (EditorFindInfiniteLoop(m_AnimationSet))
		{
			return "INFINITE LOOP!! A set cannot reference itself";
		}
		foreach (UnimaReference u in m_AnimationSet)
		{
			if (u.Animation == null)
			{
				return "Set contains a missing unimation reference";
			}
			if (!u.Animation.EditorValidate(gameObject, out string error))
			{
				return error;
			}
		}
		return null;
	}

	private bool EditorFindInfiniteLoop(UnimaSet set)
	{
		foreach (UnimaReference anim in set)
		{
			if (anim.Animation is UnimateSet child)
			{
				if (this.GetInstanceID() == child.GetInstanceID())
				{
					return true;
				}
				if (EditorFindInfiniteLoop(child.Set))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override UnimaDurationType GetEditorDuration(out float duration)
	{
		UnimaDurationType type = UnimaDurationType.Fixed;
		duration = 0.0f;
		foreach (UnimaReference u in m_AnimationSet)
		{
			if (u.Animation == null || !u.Timing.m_WaitToFinish)
			{
				continue;
			}
			UnimaDurationType uType = u.Animation.GetEditorDuration(out float uDuration);
			switch (uType)
			{
				case UnimaDurationType.Infinite:
					duration = -1.0f;
					return UnimaDurationType.Infinite;
				case UnimaDurationType.Arbitrary:
					duration = -1.0f;
					type = UnimaDurationType.Arbitrary;
					break;
				case UnimaDurationType.Fixed:
					if (type == UnimaDurationType.Fixed)
					{
						duration += uDuration;
					}
					break;
			}
			
		}
		return type;
	}

	public class Player : UnimaPlayer<UnimateSequence>
	{
		private List<IUnimaPlayer> m_Players = null;
		IUnimaContext m_Context = null;
		private int m_Index = 0;

		protected override void OnInitialize()
		{
			m_Players = Animation.Set.InstantiatePlayers(GameObject);
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			m_Context = context;
			return m_Players.Count > 0;
		}

		protected override void OnPreStart()
		{
			m_Index = 0;
			// Offset first player in the sequence by our start time so it can get PreStart/PreUpdate if needed
			m_Players[m_Index].Play(m_Context, StartTime);
		}

		protected override void OnPreUpdate(float deltaTime)
		{
			m_Players[m_Index].UpdatePlaying(deltaTime);
		}

		protected override bool OnUpdate(float deltaTime)
		{
			IUnimaPlayer player = m_Players[m_Index];
			player.UpdatePlaying(deltaTime);
			if (!player.IsPlaying() && m_Index < m_Players.Count - 1)
			{
				m_Index++;
				player = m_Players[m_Index];
				player.Play(m_Context, 0.0f);
			}
			return player.IsPlaying();
		}

		protected override void OnStop(bool interrupted)
		{
			m_Players[m_Index].Stop();
		}
	}
}
