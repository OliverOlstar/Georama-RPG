using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InitialInventoryCollection : Data.DataCollection<InitialInventoryData>
{
	public InitialInventoryCollection(string id, List<InitialInventoryData> list) : base(id, list) { }
}

[System.Serializable]
public class InitialInventoryCollections : Data.DataCollections<InitialInventoryCollection, InitialInventoryData>
{
	public InitialInventoryCollections(List<InitialInventoryCollection> collections) : base(collections) { }
}

[DatabaseSource(@"ItemData.xlsx")]
public class InitialInventoryDB : 
	Data.DataBaseWithCollectionBin<InitialInventoryDB, InitialInventoryCollections, InitialInventoryCollection, InitialInventoryData>
{
	public override void AddValidationDependencies(List<Type> dependencies)
	{
		ItemCatalog.Instance.AddValidationDependenciesAndSelf(dependencies);
	}
}