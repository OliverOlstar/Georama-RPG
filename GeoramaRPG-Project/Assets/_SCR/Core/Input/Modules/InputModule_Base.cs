using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace OliverLoescher.Input
{
    public abstract class InputModule_Base : IInputModule
	{
		protected Func<bool> isValid = null;
		protected InputAction inputAction = null;

		public virtual void Initalize(InputAction pInputAction, Func<bool> pIsValid)
		{
			inputAction = pInputAction;
			isValid = pIsValid;
		}

		public abstract void Enable();
		public abstract void Disable();
		public abstract void Clear();
		public virtual void Update(in float pDeltaTime) { }
	}
}

public interface IInputModule
{
	public void Enable();
	public void Disable();
	public void Clear();
	public void Update(in float pDeltaTime);
}