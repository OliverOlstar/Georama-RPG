
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Set")]
public class UnimateSet : Unimate<UnimateSet, UnimateSet.Player>
{
	[SerializeField]
	private UnimaSet m_AnimationSet = new UnimaSet();
	public UnimaSet Set => m_AnimationSet;

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
					if (type == UnimaDurationType.Fixed && uDuration > duration)
					{
						duration = uDuration;
					}
					break;
			}
		}
		return type;
	}

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

	public class Player : UnimaPlayer<UnimateSet>
	{
		private List<IUnimaPlayer> m_Players = null;
		private IUnimaContext m_Context = null;

		protected override void OnInitialize()
		{
			m_Players = Animation.Set.InstantiatePlayers(GameObject);
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			m_Context = context;
			return true;
		}

		protected override void OnPreStart()
		{
			foreach (IUnimaPlayer player in m_Players)
			{
				player.Play(m_Context, StartTime); // Offset players by our start time so they can get PreStart/PreUpdate if needed
			}
		}

		protected override void OnPreUpdate(float deltaTime)
		{
			foreach (IUnimaPlayer player in m_Players)
			{
				player.UpdatePlaying(deltaTime);
			}
		}

		protected override bool OnUpdate(float deltaTime)
		{
			bool playing = false;
			foreach (IUnimaPlayer player in m_Players)
			{
				player.UpdatePlaying(deltaTime);
				if (player.IsPlaying())
				{
					playing = true;
				}
			}
			return playing;
		}

		protected override void OnStop(bool interrupted)
		{
			foreach (IUnimaPlayer player in m_Players)
			{
				player.Stop();
			}
		}
	}
}
