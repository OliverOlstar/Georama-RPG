using UnityEngine;

public interface IUnimaPlayer
{
	bool IsPlaying();
	void Play(IUnimaContext context, float offsetStartTime);
	void UpdatePlaying(float deltaTime);
	void Stop();
}

public class UnimaPlayer<TAnimation> : IUnimaPlayer where TAnimation : UnimateBase
{
	private TAnimation m_Animation = null;
	public TAnimation Animation => m_Animation;
	private GameObject m_GameObject = null;
	public GameObject GameObject => m_GameObject;
	public Transform Transform => m_GameObject.transform;
	private UnimaTiming m_Timing = null;
	public UnimaTiming Timing => m_Timing;

	private float m_OffsetStartTime = 0.0f;
	private float m_InternalTimer = 0.0f;

	private float m_Timer = 0.0f;
	public float Timer => m_Timer;

	private bool m_Playing = false;
	public bool IsPlaying() { return m_Playing && m_Timing.m_WaitToFinish; }
	private bool m_Started = false;

	protected float StartTime => m_Timing.m_StartTime + m_OffsetStartTime;

	public void Initialize(UnimateBase animation, UnimaTiming timing, GameObject gameObject)
	{
		m_Animation = animation as TAnimation;
		m_Timing = timing;
		m_GameObject = gameObject;
		OnInitialize();
	}

	public void UpdatePlaying(float deltaTime)
	{
		if (!m_Playing || m_Animation == null)
		{
			return;
		}

		m_InternalTimer += deltaTime;
		float startTime = m_Timing.m_StartTime + m_OffsetStartTime;
		if (m_InternalTimer < startTime)
		{
			OnPreUpdate(deltaTime);
			if (m_InternalTimer < startTime)
			{
				return;
			}
		}

		m_Timer = m_InternalTimer - startTime;
		if (!m_Started)
		{
			m_Started = true;
			OnStart();
		}

		bool playing = OnUpdate(deltaTime);
		if (!playing)
		{
			m_Playing = false;
			OnStop(false); // Finished naturally
		}
	}

	public void Stop()
	{
		if (!m_Playing)
		{
			return;
		}
		m_Playing = false;
		if (m_Started)
		{
			OnStop(true); // Was interrupted by an outside source
		}
	}

	public void Play(IUnimaContext context, float offsetStartTime)
	{
		if (m_Animation == null)
		{
			return;
		}
		if (m_Playing)
		{
			OnStop(true); // Interrupted
		}
		if (!TryPlay(context))
		{
			return;
		}
		m_Playing = true;
		m_Started = false;
		m_InternalTimer = 0.0f;
		m_Timer = 0.0f;
		m_OffsetStartTime = offsetStartTime;
		float startTime = m_Timing.m_StartTime + m_OffsetStartTime;
		if (m_Timing.m_StartTime < Core.Util.EPSILON)
		{
			OnPreStart();
			m_Started = true;
			OnStart();
			OnUpdate(0.0f);
		}
		else
		{
			OnPreStart();
			OnPreUpdate(0.0f);
		}
	}

	protected virtual void OnInitialize() { }

	protected virtual bool TryPlay(IUnimaContext context) { return true; }

	protected virtual void OnPreStart() { }

	protected virtual void OnPreUpdate(float deltaTime) { }

	protected virtual void OnStart() { }

	protected virtual bool OnUpdate(float deltaTime) { return true; }

	protected virtual void OnStop(bool interrupted) { }
}
