using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnityEventWithButton))]
public class UnityEventWithButtonEditor : Editor
{
	public override void OnInspectorGUI() 
	{
		base.OnInspectorGUI();

		UnityEventWithButton script = (UnityEventWithButton)target;

		if (GUILayout.Button("Invoke"))
		{
			script.InvokeEvent();
		}
	}
}
