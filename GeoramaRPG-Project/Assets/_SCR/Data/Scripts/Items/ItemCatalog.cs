using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DatabaseSource(@"ItemData.xlsx")]
public class ItemCatalog : Data.DataCatalog<ItemCatalog, ItemCatalogData, ItemData>
{
}
