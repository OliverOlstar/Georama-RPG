
using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerAccountData : AccountStateData
{
	public static readonly string DEFAULT_BUILD_VERSION = "na";

	public PlayerAccountData() : base() { }
}

public class PlayerAccount : AccountState<PlayerAccountData>, ILateUpdatable
{
	public static PlayerAccountData Player => Core.Director.TryGet(out PlayerAccount account) ? account.PlayerData :
		throw new System.ArgumentNullException("PlayerAccount should never be null");

	// public static AccountInventory Inventory => Core.Director.Get<AccountInventory>() ?? 
	// 	throw new System.ArgumentNullException("PlayerAccount.Inventory should never be null");

	// public static AccountPrefs Prefs => Core.Director.Get<AccountPrefs>() ??
	// 	throw new System.ArgumentNullException("PlayerAccount.Prefs should never be null");

	// public static AccountBattle Battle => Core.Director.Get<AccountBattle>() ??
	// 	throw new System.ArgumentNullException("PlayerAccount.Battle should never be null");

	private Dictionary<System.Type, IAccountSubstate> m_Substates = new Dictionary<System.Type, IAccountSubstate>();
	
	public override void OnCreate()
	{
		base.OnCreate();
		foreach (System.Type type in Core.TypeUtility.GetTypesImplementingInterface(typeof(IAccountSubstate)))
		{
			IAccountSubstate substate = Core.Director.GetOrCreate(type) as IAccountSubstate;
			m_Substates.Add(type, substate);
		}
		
		Chrono.RegisterLate(this);
	}

	public void InitializeNewAccount()
	{
		if (GameStateManager.IsInGame())
		{
			Debug.LogError("PlayerAccount.NewAccount() Cannot call this function while in game, it's not safe");
			return;
		}
		PlayerPrefs.DeleteAll();
		OnNewAccount();
		List<IAccountSubstate> substates = ListPool<IAccountSubstate>.Request();
		substates.AddRange(m_Substates.Values);
		substates.Sort(CompareSubstateByPriority);
		foreach (IAccountSubstate substate in substates)
		{
			substate.InitializeFromDisk(true);
		}
		ListPool<IAccountSubstate>.Return(substates);
	}

	public void InitializeFromDisk(bool firstInit = false)
	{
		List<IAccountSubstate> substates = ListPool<IAccountSubstate>.Request();
		substates.AddRange(m_Substates.Values);
		substates.Sort(CompareSubstateByPriority);
		foreach (IAccountSubstate substate in substates)
		{
			substate.InitializeFromDisk(firstInit);
		}
		ListPool<IAccountSubstate>.Return(substates);
	}

	public void RegisterAccount(PlayerAccountData data)
	{
		// This function is for when we're using a fresh local account and we first contact the server
		// We want to set our user name and password with the server authorized ones to legitimize the account
		if (!PlayerData.IsNewAccount())
		{
			Debug.LogError(GetType().Name + ".RegisterAccount() Cannot create account, we already have a proper one on disk");
			return;
		}
		OnUpdateAccount(data);
		// Setting our account ID changes the key DiskDirectors use to write to disk
		// Dirtying all of the disk directors will force them to re-serialize their state and regenerate their keys with our new account ID
		foreach (IAccountSubstate substate in m_Substates.Values)
		{
			substate.Dirty("PlayerAccount.RegisterAccount()");
		}
	}

	public void OnLateUpdate(double deltaTime)
	{
		string jsons = string.Empty;
		foreach (IAccountSubstate substate in m_Substates.Values)
		{
			if (substate.WriteToDisk(out string json))
			{
				jsons += $"\n[Account] {substate.GetType().Name}.WriteToDisk()\n{json}\n";
			}
		}
		if (!string.IsNullOrEmpty(jsons))
		{
			Debug.Log(Core.DebugUtil.ColorString(Core.ColorConst.Merlot, "[Account]") + " Writing state to disk" + jsons);
			PlayerPrefs.Save();
		}
	}

	private int CompareSubstateByPriority(IAccountSubstate x, IAccountSubstate y)
	{
		// higher value priority first 
		return y.InitializationPriority.CompareTo(x.InitializationPriority);
	}

	public void OnRegistered()
	{
		
	}

	public void OnDeregistered()
	{
		
	}
	
	public double DeltaTime { get; }
}
