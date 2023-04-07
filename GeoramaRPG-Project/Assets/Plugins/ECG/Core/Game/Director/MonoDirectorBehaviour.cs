
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public class MonoDirectorBehaviour : MonoBehaviour
	{
		protected virtual void Awake()
		{

		}

		protected virtual void OnEnable()
		{
			Core.MonoDirector.Register(this);
		}

		protected virtual void OnDisable()
		{
			Core.MonoDirector.Deregister(this);
		}
	}
}

