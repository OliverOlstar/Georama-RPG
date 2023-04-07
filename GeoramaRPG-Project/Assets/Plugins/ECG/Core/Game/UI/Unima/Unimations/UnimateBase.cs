
using System.Collections.Generic;
using UnityEngine;

public abstract class UnimateBase : ScriptableObject
{
	public abstract IUnimaPlayer CreatePlayer(UnimaTiming timing, GameObject gameObject);

	public bool EditorValidate(GameObject gameObject, out string error)
	{
		error = OnEditorValidate(gameObject);
		return string.IsNullOrEmpty(error);
	}

	protected abstract string OnEditorValidate(GameObject gameObject);

	public abstract UnimaDurationType GetEditorDuration(out float seconds);
}
