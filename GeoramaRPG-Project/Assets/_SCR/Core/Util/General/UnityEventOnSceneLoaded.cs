using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UnityEventOnSceneLoaded : MonoBehaviour
{
	[System.Serializable]
	public struct SceneEvent
	{
		[Min(0)] public int buildIndex;
		public UnityEvent OnEvent;
	}

	[SerializeField] private SceneEvent[] events = new SceneEvent[0];

	private void Awake() 
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		foreach (SceneEvent e in events)
		{
			if (scene.buildIndex == e.buildIndex)
			{
				e.OnEvent.Invoke();
			}
		}
	}
}
