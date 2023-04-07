using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
