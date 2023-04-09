using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ImportDataMenuItem
{
	[MenuItem("Assets/Import Data/Import Game Data %#d")]
	public static void Import()
	{
		Data.ImportData.ImportForEditor();
	}

	[MenuItem("Assets/Import Data/Force Full Game Data Import %&d")]
	public static void ForceImport()
	{
		Data.ImportData.SetForceValidateAndRegenerateNextImport();
		Data.ImportData.ImportForEditor();
	}
}
