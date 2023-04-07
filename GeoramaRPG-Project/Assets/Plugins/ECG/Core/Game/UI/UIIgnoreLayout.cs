using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIIgnoreLayout : MonoBehaviour
{
	static HashSet<int> s_InstanceIDs = new HashSet<int>();

	public static bool IsIgnoringLayout(Transform transform)
	{
		if (transform == null)
		{
			return false;
		}
		return s_InstanceIDs.Contains(transform.GetInstanceID());
	}

	void Awake()
	{
		if (gameObject != null)
		{
			s_InstanceIDs.Add(transform.GetInstanceID());
		}
	}

	void OnDestroy()
	{
		if (gameObject != null)
		{
			s_InstanceIDs.Remove(transform.GetInstanceID());
		}
	}
}
