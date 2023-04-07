
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class UnimaDrawerUtil
{
	public static float LEFT_INDENT = 16.0f;
	public static float RIGHT_INDENT = 4.0f;

	public static Rect AddIndent(Rect position)
	{
		position.x += LEFT_INDENT;
		position.width -= LEFT_INDENT;
		position.width -= RIGHT_INDENT;
		return position;
	}
	public static Rect RemoveIndent(Rect position)
	{
		position.x -= LEFT_INDENT;
		position.width += LEFT_INDENT;
		position.width += RIGHT_INDENT;
		return position;
	}
}
