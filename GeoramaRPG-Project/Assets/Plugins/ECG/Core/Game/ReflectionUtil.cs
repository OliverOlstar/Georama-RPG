using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core
{
	public class ReflectionUtil
	{
		public static PropertyInfo GetPropertyInfoRecursively(Type type, string propertyName, BindingFlags bindingFlags)
		{
			PropertyInfo propertyInfo = null;
			do
			{
				propertyInfo = type.GetProperty(propertyName, bindingFlags);
				if (propertyInfo == null)
				{
					type = type.BaseType;
				}
			} while (type != typeof(object) && propertyInfo == null);

			return propertyInfo;
		}

		public static FieldInfo GetFieldInfoRecursively(Type type, string fieldName, BindingFlags bindingFlags)
		{
			FieldInfo fieldInfo = null;
			do
			{
				fieldInfo = type.GetField(fieldName, bindingFlags);
				if (fieldInfo == null)
				{
					type = type.BaseType;
				}
			} while (type != typeof(object) && fieldInfo == null);
			return fieldInfo;
		}


		public static bool TrySetField(object obj, string fieldName, object value)
		{
			return TrySetField(obj, fieldName, value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static bool TrySetField(object obj, string fieldName, object value, BindingFlags bindingFlags)
		{
			if (obj == null || string.IsNullOrEmpty(fieldName))
			{
				return false;
			}
			FieldInfo fieldInfo = GetFieldInfoRecursively(obj.GetType(), fieldName, bindingFlags);
			if (fieldInfo == null)
			{
				return false;
			}
			if (value != null && !fieldInfo.FieldType.Is(value.GetType()))
			{
				return false;
			}
			fieldInfo.SetValue(obj, value);
			return true;
		}
	}
}