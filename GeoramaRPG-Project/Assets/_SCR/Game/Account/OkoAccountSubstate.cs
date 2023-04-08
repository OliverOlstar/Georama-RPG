using Core;
using Newtonsoft.Json;
using UnityEngine;

public interface IAccountSubstate : IDirector
{
	void InitializeFromDisk(bool firstInit = false);
	bool WriteToDisk(out string json);
	void Dirty(string log = null);
	int InitializationPriority { get; }
}

public class DiskDirectorData
{
	[JsonProperty]
	private long m_Timestamp = 0L;
	[JsonIgnore]
	public long Timestamp => m_Timestamp;
	public void SetTimestamp() { m_Timestamp = Core.Chrono.UtcNowTimestamp; }
}

public enum DiskDirectorType
{
	ClientOnly,
	ClientServer
}

[PersistentDirector]
public abstract class OkoAccountSubstate<TAccountState, TAccountStateData> : IAccountSubstate
	where TAccountState : class, IAccountState
	where TAccountStateData : DiskDirectorData, new()
{
	// Higher priorities get initialized first
	public class InitalizePriority
	{
		public const int Default = 0;
	}

	protected TAccountStateData m_State = default;

	private bool m_Active = false;
	public bool Active => m_Active;
	private bool m_Dirty = false;

	protected virtual TypeNameHandling JSONTypeNameHandling => TypeNameHandling.None;

	public void Dirty(string log = null)
	{
		if (!m_Dirty || !Str.IsEmpty(log))
		{
			Log("Dirty", log);
		}
		m_Dirty = true;
	}

	public bool WriteToDisk(out string json)
	{
		if (!m_Dirty)
		{
			json = null;
			return false;
		}
		string key = GetKey();
		if (string.IsNullOrEmpty(key))
		{
			json = null;
			return false;
		}
		m_Dirty = false;
		OnPreWriteToDisk();
		m_State.SetTimestamp();
		json = JsonConvert.SerializeObject(m_State, new JsonSerializerSettings{ TypeNameHandling = JSONTypeNameHandling, });
		PlayerPrefs.SetString(key, json);
		return true;
	}

	//use sparingly. only when its extremely important to save right this second as save causes hiccups
	protected void ForceDirty(string log = null)
	{
		m_Dirty = true;
		WriteToDisk(out string json);
		PlayerPrefs.Save();
		Log("ForceDirty", $"{(string.IsNullOrEmpty(log) ? json : log)}\n{json}");
	}

	public virtual void OnCreate()
	{
		LoadFromDisk();
	}

	public virtual void OnDestroy()
	{
	}

	void IAccountSubstate.InitializeFromDisk(bool firstInit)
	{
		LoadFromDisk();
		OnInitializeFromDisk(firstInit);
	}

	int IAccountSubstate.InitializationPriority => InitializationPriority;

	protected virtual int InitializationPriority => InitalizePriority.Default;


	protected abstract void OnInitializeFromDisk(bool firstInit = false);

	protected virtual void OnPreWriteToDisk() { }

	private string GetKey()
	{
		if (!Core.Director.TryGet(out TAccountState userData))
		{
			Debug.LogError(GetType().Name + ".GetKey() User data must be initialized first");
			return null;
		}
		string key = Core.Str.Build(userData.GenericPlayerData.ID, ".", GetType().Name);
		return key;
	}

	private bool LoadFromDisk()
	{
		string key = GetKey();
		if (string.IsNullOrEmpty(key))
		{
			m_State = new TAccountStateData();
			Dirty();
			return false;
		}
		string json = PlayerPrefs.GetString(key, null);
		if (string.IsNullOrEmpty(json))
		{
			Log("LoadFromDisk", $"{key} no data on disk for this account");
			m_State = new TAccountStateData();
			Dirty();
			return false;
		}
		TAccountStateData state = AccountStateData.Util.SafeDeserialize<TAccountStateData>(json, JSONTypeNameHandling);
		if (state == null)
		{
			m_State = new TAccountStateData();
			ForceDirty(); // Force dirty so when we throw the exception bellow we can recover on next login
			DevException("InitializeFromDisk", $"Failed to deserialize state \n{json}");
			return false;
		}
		Log("LoadFromDisk", $"{key} from disk \n{json}");
		m_State = state;
		return true;
	}

#region Helpers
	private string LogPrefix(in string context) => Core.DebugUtil.ColorString(Core.ColorConst.Merlot, $"[Account] {GetType().Name}.{context}() ");
	protected void Log(in string context, in string log) => Debug.Log(LogPrefix(context) + log);
	protected void LogWarning(in string context, in string log) => Debug.LogWarning(LogPrefix(context) + log);
	protected void LogError(in string context, in string log) => Debug.LogError(LogPrefix(context) + log);
	protected void DevException(in string context, in string log) => Core.DebugUtil.DevException(LogPrefix(context) + log);
#endregion Helpers
}
