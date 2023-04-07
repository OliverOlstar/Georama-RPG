using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AssetSearchWindowUtil
{
	public static void GetOptimizedPaths(
		IAssetPickerPathSource pathSource,
		AssetPickerTreeSearchParameters? parameters,
		out List<string> names,
		out List<string> paths,
		out List<int> levels)
	{
		List<string> input = pathSource.GetPaths();
		char[] seperators = pathSource.GetPathSperators();
		string title = pathSource.GetSearchWindowTitle();

		// Create tree
		TreeNode root = new TreeNode(title, string.Empty, null);
		foreach (string item in input)
		{
			string[] entryTitle = item.Split(seperators);
			string name = Path.GetFileNameWithoutExtension(item);
			root.AddElement(entryTitle, 0, name, item);
		}

		// Compact tree
		root.CompactGroups(parameters.HasValue ? parameters.Value : AssetPickerTreeSearchParameters.Default);

		// Convert tree back to string arrays
		names = new List<string>();
		paths = new List<string>();
		levels = new List<int>();
		root.FillList(ref names, ref paths, ref levels, 0);
	}

	private class TreeNode
	{
		public string Name = string.Empty;
		public string Path = string.Empty; // If string.Empty, this is group & not an item

		public TreeNode Parent = null;
		public List<TreeNode> Children = new List<TreeNode>();

		public TreeNode(string name, string path, TreeNode parent)
		{
			Name = name;
			Path = path;
			Parent = parent;
		}

		public void AddElement(string[] groups, int groupIndex, string name, string path)
		{
			// Last group element should be == to 'name'
			if (groupIndex == groups.Length - 1)
			{
				Children.Add(new TreeNode(name, path, this));
				return;
			}

			// If group exists
			foreach (TreeNode child in Children)
			{
				if (child.Name == groups[groupIndex])
				{
					child.AddElement(groups, groupIndex + 1, name, path);
					return;
				}
			}

			// If group doesn't already exist
			Children.Add(new TreeNode(groups[groupIndex], string.Empty, this));
			Children[Children.Count - 1].AddElement(groups, groupIndex + 1, name, path);
		}

		public void RemoveSelf()
		{
			if (Parent == null)
			{
				return; // Cannot remove Root
			}
			foreach (TreeNode child in Children)
			{
				child.Parent = Parent;
			}
			Parent.Children.Remove(this);
			Parent.Children.AddRange(Children);
		}

		public void CompactGroups(AssetPickerTreeSearchParameters parameters) // Should be called on root Node
		{
			// If only one child group, remove it
			while (Children.Count == 1 && Children[0].Path == string.Empty)
			{
				// Normally we remove groups the other way removing itself if they have few children 
				// but we can not remove the root so we have to remove the other way for it by removing the children.
				Children[0].RemoveSelf();
			}

			// Compact Children
			bool childrenCanCompact = Children.Count < 8;
			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i].CompactGroups(parameters, childrenCanCompact))
				{
					i--;
				}
			}
		}

		private bool CompactGroups(AssetPickerTreeSearchParameters parameters, in bool parentHasMaxCount)
		{
			// Is item, not a group
			if (Path != string.Empty)
			{
				return false;
			}

			// Compact Children
			bool hasMaxCount = Children.Count < parameters.MaxCount;
			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i].CompactGroups(parameters, hasMaxCount))
				{
					i--;
				}
			}

			// Remove self
			if (Children.Count < (parentHasMaxCount ? parameters.MinCount : parameters.MinCountAboveMax))
			{
				RemoveSelf();
				return true;
			}
			return false;
		}

		public void FillList(ref List<string> names, ref List<string> paths, ref List<int> levels, int level)
		{
			names.Add(Name);
			paths.Add(Path);
			levels.Add(level);

			// Add Groups
			foreach (TreeNode child in Children)
			{
				if (child.Children.Count > 0)
				{
					child.FillList(ref names, ref paths, ref levels, level + 1);
				}
			}

			// Add Items
			foreach (TreeNode child in Children)
			{
				if (child.Children.Count == 0)
				{
					child.FillList(ref names, ref paths, ref levels, level + 1);
				}
			}
		}
	}
}
