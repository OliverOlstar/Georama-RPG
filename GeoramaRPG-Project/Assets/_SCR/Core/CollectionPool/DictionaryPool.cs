using System.Collections.Generic;

namespace Core
{
	public sealed class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
	{
	}
}
