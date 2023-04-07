using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetPickerAttribute
{
	bool AllowNull { get; }
	bool ForceFlatten { get; }
	string OverrideFirstName { get; }
}

public interface IAssetPickerPathSource
{
	string GetSearchWindowTitle();
	char[] GetPathSperators();
	List<string> GetPaths();
	bool TryGetUnityObjectType(out System.Type type);
}
