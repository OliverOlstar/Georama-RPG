using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class AccountStateData
{
	public static readonly string DEFAULT_ID = "NewUser";

	[JsonProperty]
	private string m_ID = DEFAULT_ID;
	[JsonIgnore]
	public string ID => m_ID;

	public bool IsNewAccount() { return string.Equals(m_ID, DEFAULT_ID); }

	[JsonConstructor]
	public AccountStateData() { }

	public AccountStateData(string id)
	{
		m_ID = id;
	}

	public static class Util
	{
		private static readonly JsonSerializerSettings FROM_JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
#if !RELEASE
			Formatting = Formatting.Indented, // Don't want to waste characters/processing on this in release
#endif
		};

		public static T SafeDeserialize<T>(string json, TypeNameHandling jsonTypeNameHandling = TypeNameHandling.None)
		{
			if (string.IsNullOrEmpty(json))
			{
				return default;
			}
			T data = default;
			try
			{
				TypeNameHandling initialTNH = FROM_JSON_SERIALIZER_SETTINGS.TypeNameHandling;
				if (jsonTypeNameHandling != TypeNameHandling.None)
				{
					FROM_JSON_SERIALIZER_SETTINGS.TypeNameHandling = jsonTypeNameHandling;
				}

				data = JsonConvert.DeserializeObject<T>(json, FROM_JSON_SERIALIZER_SETTINGS);

				FROM_JSON_SERIALIZER_SETTINGS.TypeNameHandling = initialTNH;
				return data;
			}
			catch (System.Exception exception)
			{
				Debug.LogError($"RPC.Util.SafeDeserialize() Parse {typeof(T)} {exception} failed {json}");
				return default(T);
			}
		}
	}
}

public interface IAccountState : IDirector
{
	AccountStateData GenericPlayerData { get; }
}

[PersistentDirector]
public class AccountState<TAccountData> : IAccountState where TAccountData : AccountStateData, new()
{
	private static readonly string KEY = "CurrentAccountKey";

	private TAccountData m_AccountData = null;
	public TAccountData PlayerData => m_AccountData;

	AccountStateData IAccountState.GenericPlayerData => m_AccountData;

	public virtual void OnCreate()
	{
		if (!TryLoadFromDisk())
		{
			Log(GetType().Name + ".OnCreate() Default account created");
			m_AccountData = new TAccountData();
			Dirty();
		}
	}

	public virtual void OnDestroy() { }

	protected void OnNewAccount()
	{
		Log(GetType().Name + ".OnNewAccount() Default account created");
		m_AccountData = new TAccountData();
		Dirty();
	}

	protected void OnUpdateAccount(TAccountData data)
	{
		Log(GetType().Name + ".OnUpdateAccount() Updating account from server " + data.ID);
		m_AccountData = data;
		Dirty();
	}

	private bool TryLoadFromDisk()
	{
		string json = PlayerPrefs.GetString(KEY, null);
		if (string.IsNullOrEmpty(json))
		{
			Log(GetType().Name + ".OnCreate() " + KEY + " no data on disk for this account");
			return false;
		}
		m_AccountData = AccountStateData.Util.SafeDeserialize<TAccountData>(json);
		if (m_AccountData == null)
		{
			Debug.LogError(GetType().Name + ".OnCreate() Failed to deserialize data " + json);
			return false;
		}
		Log(GetType().Name + ".InitializeFromDisk() Read from disk \n" + json);
		return true;
	}

	protected void Dirty()
	{
		string json = JsonConvert.SerializeObject(m_AccountData, Formatting.Indented);
		Log($"{GetType().Name}.Dirty() Writing to disk \n{json}");
		PlayerPrefs.SetString(KEY, json);
	}

	protected void Log(string log)
	{
		Debug.Log(Core.DebugUtil.ColorString(Core.ColorConst.Merlot, "[Account] ") + log);
	}
}
