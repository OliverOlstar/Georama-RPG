using System.Collections.Generic;

public static class DictionaryExtentionMethods
{
	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue value)
	{
		if (self.ContainsKey(key))
		{
			return false;
		}
		self[key] = value;
		return true;
	}

	public static bool RemoveAndRetrieve<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, out TValue value)
	{
		self.TryGetValue(key, out value);
		return self.Remove(key);
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> other)
	{
		foreach (KeyValuePair<TKey, TValue> kvp in other)
		{
			self.Add(kvp.Key, kvp.Value);
		}
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> other)
	{
		foreach (KeyValuePair<TKey, TValue> kvp in other)
		{
			self.Add(kvp.Key, kvp.Value);
		}
	}

	public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue = default)
	{
		if (!self.TryGetValue(key, out TValue value))
		{
			value = defaultValue;
		}
		return value;
	}
}