
using System;
using System.Collections.Generic;
using UnityEngine;

public class AccountInventoryState : DiskDirectorData
{
	public Dictionary<string, int> Inventory = new Dictionary<string, int>();
}

public class AccountInventory : OkoAccountSubstate<PlayerAccount, AccountInventoryState>
{
	protected override void OnInitializeFromDisk(bool firstInit = false)
	{
		if (firstInit)
		{
			// AccountInitInventoryCollection initCollection = AccountInitInventoryDB.GetLatestVersion();
			// Log("OnInitializeFromDisk", "Inventory is empty, initializing with data version " + initCollection.VersionNum);

			// foreach (AccountInitInventoryData initData in initCollection)
			// {
			// 	if (m_State.Inventory.ContainsKey(initData.KeyData.ID))
			// 	{
			// 		m_State.Inventory[initData.KeyData.ID] += initData.Quantity;
			// 	}
			// 	else
			// 	{
			// 		m_State.Inventory.Add(initData.KeyData.ID, initData.Quantity);
			// 	}
			// }
			// Dirty();
		}
	}

	// protected override void OnInitializeFromServer(LoginResponse login)
	// {
	// 	m_State.SetInventory(login);
	// }

	public bool TryGetKeyQuantity(string key, out int quantity)
	{
		return m_State.Inventory.TryGetValue(key, out quantity);
	}

	public int GetKeyQuantity(string key)
	{
		m_State.Inventory.TryGetValue(key, out int quantity);
		return quantity;
	}

	public bool HasKey(string key) => m_State.Inventory.TryGetValue(key, out int count) && count > 0;

	public bool HasKeyQuantity(string key, int quantity)
	{
		return GetKeyQuantity(key) >= quantity;
	}

	// public bool HasInventory(Inventory inventory)
	// {
	// 	inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
	// 	foreach (KeyValuePair<string, int> kvp in inventory)
	// 	{
	// 		if (!HasKeyQuantity(kvp.Key, kvp.Value))
	// 		{
	// 			return false;
	// 		}
	// 	}
	// 	return true;
	// }

	// public bool IsAffordable(string costItemSetID)
	// {
	// 	if (!ItemSetDB.TryGetItemSet(costItemSetID, out IItemSet itemSet))
	// 	{
	// 		throw new ArgumentNullException("itemSet");
	// 	}
	// 	return IsAffordable(itemSet);
	// }

	// public bool IsAffordable(IItemSet itemSet)
	// {
	// 	Inventory missingDelta = new Inventory();
	// 	GetMissingFunds(itemSet, missingDelta);
	// 	return missingDelta.Count == 0;
	// }

	// public void GetMissingFunds(IItemSet costs, Inventory missing)
	// {
	// 	Inventory inventory = new Inventory();
	// 	costs.GetItems(inventory);
	// 	GetMissingFunds(inventory, missing);
	// }

	// public void GetMissingFunds(Inventory costs, Inventory missing)
	// {
	// 	costs = costs ?? throw new ArgumentNullException("costs");
	// 	missing = missing ?? throw new ArgumentNullException("missing");
	// 	missing.Clear();
	// 	foreach (KeyValuePair<string, int> cost in costs)
	// 	{
	// 		TryGetKeyQuantity(cost.Key, out int owned);
	// 		if (owned >= cost.Value)
	// 		{
	// 			continue;
	// 		}
	// 		missing.Add(cost.Key, cost.Value - owned);
	// 	}
	// }

	// private void ApplyTransaction(InventoryTransaction transaction)
	// {
	// 	foreach (KeyValuePair<string, int> pair in transaction.Delta)
	// 	{
	// 		int delta = pair.Value;
	// 		if (delta == 0)
	// 		{
	// 			Debug.LogWarning("AccountInventory.ApplyTransaction() " + pair.Key + " delta is zero, that's weird?");
	// 			continue;
	// 		}
	// 		if (m_State.Inventory.TryGetValue(pair.Key, out int quantity))
	// 		{
	// 			int newQuantity = quantity + delta;
	// 			if (newQuantity < 0)
	// 			{
	// 				newQuantity = 0;
	// 				Debug.LogError("AccountInventory.ApplyTransaction() Trying to spend " +
	// 					pair.Value + " of " + pair.Key + " but we only have " + quantity);
	// 			}
	// 			//capping items so that players cant get above an int value and brick their game. server also caps separately
	// 			m_State.Inventory[pair.Key] = Mathf.Min(newQuantity, GlobalTunableDB.ItemMaxLimit);
	// 		}
	// 		else
	// 		{
	// 			if (delta > 0)
	// 			{
	// 				//capping items so that players cant get above an int value and brick their game. server also caps separately
	// 				m_State.Inventory.Add(pair.Key, Mathf.Min(pair.Value, GlobalTunableDB.ItemMaxLimit));
	// 			}
	// 			else
	// 			{
	// 				Debug.LogError("AccountInventory.ApplyTransaction() Trying to spend " +
	// 					pair.Value + " of " + pair.Key + " but we have none");
	// 			}
	// 		}
	// 	}
	// 	AccountMessage.Route(AccountMessage.Type.InventoryUpdated, transaction);
	// }

	// private void OnCheatItem(InventoryTransaction transaction)
	// {
	// 	ApplyTransaction(transaction);
	// 	Dirty("OnCheatItem");
	// }
}
