using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Data;

[DatabaseSource(@"GearData.xlsx")]
public class GearDB : DataBaseBin<GearDB, GearData>, IDataCatalogDB
{
	IEnumerable<IDataDictItem> IDataCatalogDB.CatalogData => GetValues();

	bool IDataCatalogDB.TryGetCatalogData(string id, out IDataDictItem data)
	{
		bool found = TryGet(id, out GearData gearData);
		data = gearData;
		return found;
	}

}
