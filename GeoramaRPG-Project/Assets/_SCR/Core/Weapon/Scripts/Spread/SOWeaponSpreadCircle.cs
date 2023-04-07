using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Spread/Circle")]
	public class SOWeaponSpreadCircle : SOWeaponSpreadExpandBase
	{
		public float spreadRadius = 0.2f;
		public float spreadRadiusMax = 0.5f;

		public override Vector3 ApplySpread(Vector3 pDirection)
		{
			float spread = Mathf.Lerp(spreadRadius, spreadRadiusMax, spread01);
			return Quaternion.Euler(Util.GetRandomPointOnCircle(spread)) * pDirection;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(in Transform pTransform, in Transform pMuzzle)
		{
			Handles.matrix = pTransform.localToWorldMatrix;
			Vector3 localForward = pTransform.InverseTransformVector(pMuzzle.forward);

			Handles.color = Color.cyan;
			Handles.DrawWireDisc(localForward * 1.0f, localForward, spreadRadius * 0.01f);

			Handles.color = Color.blue;
			Handles.DrawWireDisc(localForward * 1.0f, localForward, spreadRadiusMax * 0.01f);

			if (Application.isPlaying)
			{
				Handles.color = Color.green;
				float spread = Mathf.Lerp(spreadRadius, spreadRadiusMax, spread01);
				Handles.DrawWireDisc(localForward * 1.0f, localForward, spread * 0.01f);
			}
		}
#endif
	}
}
