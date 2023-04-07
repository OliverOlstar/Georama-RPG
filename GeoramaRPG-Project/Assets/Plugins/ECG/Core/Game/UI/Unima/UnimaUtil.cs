
using System.Collections.Generic;
using UnityEngine;

public static class UnimaUtil
{
	public static void GetBetas(
		GameObject gameObject,
		string alphaID,
		List<IUnimaControllerSource> betaSources,
		List<UnimaController> betaControllers)
	{
		betaSources.Clear();
		betaControllers.Clear();
		if (string.IsNullOrEmpty(alphaID))
		{
			return;
		}
		FindControllerSources(gameObject, betaSources);
		for (int i = betaSources.Count - 1; i >= 0; i--)
		{
			IUnimaControllerSource childSource = betaSources[i];
			int endIndex = betaControllers.Count; // Record last current controller
			childSource.AddControllers(betaControllers);
			for (int j = betaControllers.Count - 1; j >= endIndex; j--) // Loop over new controllers removing non-family
			{
				UnimaController controller = betaControllers[j];
				if (controller.Mode != UnimaControllerMode.Beta ||
					string.IsNullOrEmpty(controller.AlphaID) ||
					!string.Equals(controller.AlphaID, alphaID))
				{
					betaControllers.RemoveAt(j);
				}
			}
			if (endIndex == betaControllers.Count)
			{
				betaSources.RemoveAt(i); // Didn't add any controllers from this source
			}
		}
	}

	public static bool TryGetAlpha(GameObject gameObject, string alphaID, out UnimaController alphaController, out IUnimaControllerSource alphaSource)
	{
		List<IUnimaControllerSource> sources = Core.ListPool<IUnimaControllerSource>.Request();
		List<UnimaController> controllers = Core.ListPool<UnimaController>.Request();
		FindControllerSources(gameObject, sources);
		int length = sources.Count;
		for (int i = 0; i < length; i++)
		{
			controllers.Clear();
			sources[i].AddControllers(controllers);
			int controllerCount = controllers.Count;
			for (int j = 0; j < controllerCount; j++)
			{
				UnimaController controller = controllers[j];
				if (controller.Mode == UnimaControllerMode.Alpha &&
					string.Equals(controller.AlphaID, alphaID))
				{
					alphaController = controller;
					alphaSource = sources[i];
					Core.ListPool<IUnimaControllerSource>.Return(sources);
					Core.ListPool<UnimaController>.Return(controllers);
					return true;
				}
			}
		}
		Core.ListPool<IUnimaControllerSource>.Return(sources);
		Core.ListPool<UnimaController>.Return(controllers);
		alphaController = null;
		alphaSource = null;
		return false;
	}

	public static void FindControllerSources(GameObject gameObject, List<IUnimaControllerSource> listToFill)
	{
		List<IUnimaControllerSource> sources = Core.ListPool<IUnimaControllerSource>.Request();
		gameObject.GetComponentsInParent(false, sources);
		listToFill.AddRange(sources);
		gameObject.GetComponentsInChildren(false, sources);
		listToFill.AddRange(sources);
		Core.ListPool<IUnimaControllerSource>.Return(sources);
	}
}
