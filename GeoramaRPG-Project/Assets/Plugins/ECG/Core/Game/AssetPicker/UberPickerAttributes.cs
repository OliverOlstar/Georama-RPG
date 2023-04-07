using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UberPicker
{
	public class AssetAttribute : PropertyAttribute, IAssetPickerAttribute
	{
		private bool m_AllowNone;
		private string m_Path;
		public string PathPrefix => m_Path;
		private string m_OverrideButtonName;
		private readonly bool m_CanBeNested;

		bool IAssetPickerAttribute.AllowNull => m_AllowNone;
		bool IAssetPickerAttribute.ForceFlatten => false;
		string IAssetPickerAttribute.OverrideFirstName => m_OverrideButtonName;

		public bool CanBeNested => m_CanBeNested;
		
		/// <param name="allowNull"></param>
		/// <param name="path"></param>
		/// <param name="overrideButtonName"></param>
		/// <param name="canBeNested">Set to false to avoid loading assets, strongly recommended for assets with SerializeReference attribute on any of their fields which can cause slow import time in the editor</param>
		public AssetAttribute(bool allowNull = true, string path = null, string overrideButtonName = null, bool canBeNested = true)
		{
			m_AllowNone = allowNull;
			m_Path = path;
			m_OverrideButtonName = overrideButtonName;
			m_CanBeNested = canBeNested;
		}
	}

	public class AssetNonNullAttribute : UberPicker.AssetAttribute
	{
		public AssetNonNullAttribute(string path = null, string overrideButtonName = null) : base(false, path, overrideButtonName)
		{

		}
	}

	public class AssetNameAttribute : PropertyAttribute, IAssetPickerAttribute
	{
		private System.Type m_Type;
		public System.Type Type => m_Type;
		private bool m_AllowNone;
		private string m_Path;
		public string PathPrefix => m_Path;

		bool IAssetPickerAttribute.AllowNull => m_AllowNone;
		bool IAssetPickerAttribute.ForceFlatten => false;
		string IAssetPickerAttribute.OverrideFirstName => null;

		public AssetNameAttribute(System.Type assetType, bool allowEmpty = false, string path = null)
		{
			m_Type = assetType;
			m_AllowNone = allowEmpty;
			m_Path = path;
		}
	}

	public class DataIDAttribute : PropertyAttribute, IAssetPickerAttribute
	{
		private System.Type m_DBType;
		public System.Type DBType => m_DBType;
		private bool m_AllowNone;
		private bool m_Flatten;
		private string m_OverrideNoneString;

		string IAssetPickerAttribute.OverrideFirstName => m_OverrideNoneString;
		bool IAssetPickerAttribute.ForceFlatten => m_Flatten;
		bool IAssetPickerAttribute.AllowNull => m_AllowNone;

		public DataIDAttribute(System.Type assetType, bool allowEmpty = false, bool flatten = true, string overrideNoneString = null)
		{
			m_DBType = assetType;
			m_AllowNone = allowEmpty;
			m_Flatten = flatten;
			m_OverrideNoneString = overrideNoneString;
		}
	}
}
