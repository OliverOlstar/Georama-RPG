using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SearchWindowStyle
{
    Full,
    Compact,
    Flatten,
    NoGroups
}

public interface ISearchWindowParameters
{
	bool AllowNull { get; }
	SearchWindowStyle Style { get; }
	AssetPickerTreeSearchParameters? TreeSearchParameters { get; }
}

public struct AssetPickerTreeSearchParameters
{
	public static readonly AssetPickerTreeSearchParameters Default = new AssetPickerTreeSearchParameters
	{
		MinCount = 3,
		MaxCount = 8,
		MinCountAboveMax = 2,
	};

	public int MinCount;
	public int MaxCount;
	public int MinCountAboveMax;

	public AssetPickerTreeSearchParameters(int minCount, int maxCount, int minCountAboveMax)
	{
		MinCount = minCount;
		MaxCount = maxCount;
		MinCountAboveMax = minCountAboveMax;
	}
}

public class SearchWindowAttribute : PropertyAttribute, ISearchWindowParameters // Base class
{
	public SearchWindowStyle Style { get; protected set; } = SearchWindowStyle.Compact;
	public bool AllowNull { get; protected set; } = true;

	// Compact Style
	public AssetPickerTreeSearchParameters? TreeSearchParameters { get; protected set; } = null;

    // Type
	public Type CustomType { get; protected set; } = null;
	public string PathPrefix { get; protected set; } = string.Empty;

	public SearchWindowAttribute(
		SearchWindowStyle style = SearchWindowStyle.Compact,
		string pathPrefix = null)
	{
		Style = style;
		AllowNull = true;
		PathPrefix = pathPrefix;
	}
}

public class StringSearchWindowAttribute : SearchWindowAttribute, IAssetPickerPathSource
{
    public string[] StringPaths { get; protected set; } = null;
    public bool UseTextField { get; protected set; } = false;
	public string Title { get; protected set; } = null;

	public StringSearchWindowAttribute(string[] stringPaths, string title = "Strings", SearchWindowStyle style = SearchWindowStyle.Full, bool useTextField = false, bool allowNull = false)
    {
        Style = style;
        AllowNull = allowNull;

		StringPaths = stringPaths;
		UseTextField = useTextField;
		Title = title;
	}

	string IAssetPickerPathSource.GetSearchWindowTitle() => Title;

	char[] IAssetPickerPathSource.GetPathSperators() => new char[] { };

	List<string> IAssetPickerPathSource.GetPaths() => new List<string>(StringPaths);

	bool IAssetPickerPathSource.TryGetUnityObjectType(out System.Type type) { type = null; return false; }
}

public class DataIDSearchWindowAttribute : SearchWindowAttribute
{
	public System.Type DBType { get; protected set; } = null;
	public bool UseTextField { get; protected set; } = true;

	public DataIDSearchWindowAttribute(System.Type dbType, bool allowNull = true, bool useTextField = true, string overrideNoneString = null)
	{
		DBType = dbType;
		AllowNull = allowNull;
		TreeSearchParameters = new AssetPickerTreeSearchParameters(4, 8, 2);
		UseTextField = useTextField;
	}
}

#region Wrapper Attributes
public class SearchWindowNonNullAttribute : SearchWindowAttribute
{
	public SearchWindowNonNullAttribute(SearchWindowStyle style = SearchWindowStyle.Compact, string title = null, string pathPrefix = null)
	{
		Style = style;
		AllowNull = false;
		PathPrefix = pathPrefix;
	}
}

public class ProjectileSearchWindowAttribute : SearchWindowAttribute
{
    public ProjectileSearchWindowAttribute()
    {
        AllowNull = false;
        PathPrefix = "Assets/Prefabs/Projectiles/Projectile";
    }
}

public class LocatorSearchWindowAttribute : SearchWindowAttribute
{
    public LocatorSearchWindowAttribute()
    {
        AllowNull = false;
		TreeSearchParameters = new AssetPickerTreeSearchParameters(4, 8, 0);
    }
}

public class HitSearchWindowAttribute : SearchWindowAttribute
{
    public HitSearchWindowAttribute()
    {
        AllowNull = false;
    }
}
#endregion