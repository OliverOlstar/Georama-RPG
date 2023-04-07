
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicMaterialInstance : MonoBehaviour
{
	private Dictionary<Material, Material> m_Instances = new Dictionary<Material, Material>();
	private Material m_Instance = null;

	public static Material InstatiateCopy(Graphic graphic)
	{
		if (graphic.material == null)
		{
			return null;
		}

		GraphicMaterialInstance instance = Core.Util.GetOrAddComponent<GraphicMaterialInstance>(graphic.gameObject);

		// Check if another system has changed the material since we last cached a copy
		if (instance.m_Instance != null &&
			instance.m_Instance.GetInstanceID() == graphic.material.GetInstanceID())
		{
			return instance.m_Instance;
		}

		// Check if we have already cached a copy of this source material
		if (!instance.m_Instances.TryGetValue(graphic.material, out Material mat))
		{
			// We need to instantiate a copy of the material before we can modify the material so we don't modify the source
			// This behaviour caches a reference to the instance so we don't make multiple copies of the source
			mat = Instantiate(graphic.material);
			instance.m_Instances.Add(graphic.material, mat);
		}
		graphic.material = mat;
		instance.m_Instance = mat;
		return mat;
	}

	private void OnDestroy()
	{
		foreach (Material mat in m_Instances.Values)
		{
			Destroy(mat);
		}
	}
}
