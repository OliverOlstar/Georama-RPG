using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

namespace OliverLoescher
{
    public class HighlightModelBehaviour : MonoBehaviour
    {
		[SerializeField]
		private MeshFilter[] meshRenderers = new MeshFilter[1];
		[SerializeField, Required]
		private Material material = null;

		private void Reset()
		{
			meshRenderers = GetComponentsInChildren<MeshFilter>();
		}

		private void Awake()
		{
			material = Instantiate(material);
		}

		public void Set(Color pColor)
		{
			material.SetColor("_EmissionColor", pColor);
			enabled = true;
		}

		public void Clear()
		{
			enabled = false;
		}

		private void LateUpdate()
		{
			foreach (MeshFilter renderer in meshRenderers)
			{
				for (int i = 0; i < renderer.sharedMesh.subMeshCount; i++)
				{
					Graphics.DrawMesh(renderer.sharedMesh, renderer.transform.localToWorldMatrix, material, 1, MainCamera.Camera, i, null, ShadowCastingMode.Off, receiveShadows: false, null, LightProbeUsage.Off, null);
				}
			}
		}
	}
}
