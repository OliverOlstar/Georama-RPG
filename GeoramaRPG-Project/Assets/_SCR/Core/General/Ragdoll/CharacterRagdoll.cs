using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour
{
	public Transform[] transforms = null;

	private void Reset()
	{
		transforms = transform.GetComponentsInChildren<Transform>();
	}

	public void MatchPosition(CharacterRagdoll toRagdoll)
	{
		if (toRagdoll.transforms.Length != transforms.Length)
		{
			Debug.LogError($"[{nameof(CharacterRagdoll)}] SwitchTo() was passed a {nameof(CharacterRagdoll)} that can not be matched");
			return;
		}
		for (int i = 0; i < transforms.Length; i++)
		{
			transforms[i].transform.position = toRagdoll.transforms[i].transform.position;
			transforms[i].transform.rotation = toRagdoll.transforms[i].transform.rotation;
			//transforms[i].transform.localScale = toRagdoll.transforms[i].transform.localScale;
		}
	}
}
