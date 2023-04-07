using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterValues : MonoBehaviour
{
	public abstract Vector3 Forward { get; }
	public abstract Vector3 Right { get; }
	public abstract Vector3 Up { get; }

	public abstract Vector3 Velocity { get; }

	public abstract Vector3 InputMove { get; }
	public abstract Vector3 InputMoveHorizontal { get; }
}
