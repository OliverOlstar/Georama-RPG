using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InitialInventoryData : DataBin, Data.IDataCollectionItem
{
	[SerializeField]
	private string m_SetID = null;
	public string SetID => m_SetID;

	[SerializeField, Data.Validate.String.ExistsInCatalog(typeof(ItemCatalog))]
	private string m_ItemID = string.Empty;
	public string ItemID => m_ItemID;
	public ItemData ItemData => ItemCatalog.GetFromCatalog<ItemData>(m_ItemID);

	[SerializeField]
	private int m_Quantity = 1;
	public int Quantity => m_Quantity;
	
	string Data.IDataCollectionItem.CollectionID => m_SetID;
}
