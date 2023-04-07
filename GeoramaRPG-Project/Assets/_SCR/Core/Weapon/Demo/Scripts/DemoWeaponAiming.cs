using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OliverLoescher.Weapon.Demo
{
	public class DemoWeaponAiming : MonoBehaviour
	{
		[SerializeField]
		private new UnityEngine.Camera camera = null;

		void Update()
		{
			if (Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit))
			{
				transform.LookAt(hit.point);
			}
		}
	}
}