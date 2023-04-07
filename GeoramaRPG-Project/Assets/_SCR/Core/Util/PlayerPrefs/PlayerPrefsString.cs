using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    public struct PlayerPrefsString
    {
		private readonly string key;
		private string value;

		public PlayerPrefsString(string pKey, string pDefaultValue = null)
		{
			key = pKey;
			value = PlayerPrefs.GetString(key, pDefaultValue);
		}

		public string Get()
		{
			return value;
		}

		public void Set(string pValue)
		{
			if (value == pValue)
			{
				return;
			}
			value = pValue;
			PlayerPrefs.SetString(key, value);
		}
	}
}
