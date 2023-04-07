using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Georama
{
	[CreateAssetMenu(fileName = "NewBuildingData", menuName = "ScriptableObject/Georama/Building")]
	public class SOGeoramaBuilding : ScriptableObject
    {
		[Header("Display")]
		[SerializeField]
		private string displayName = string.Empty;
		[SerializeField]
		private Sprite displaySprite = null;
		[SerializeField, TextArea]
		private string displayDescription = string.Empty;

		[Header("Building")]
		[SerializeField]
		private GameObject prefab = null;
	}
}
