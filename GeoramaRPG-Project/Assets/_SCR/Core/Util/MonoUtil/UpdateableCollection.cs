using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	public class UpdateableCollection
	{
		private List<Updateable> m_Updateables = new List<Updateable>();
		private List<Updateable> m_ToRemove = new List<Updateable>();
		private List<Updateable> m_ToAdd = new List<Updateable>();

		public void Add(Updateable pItem)
		{
			m_ToAdd.Add(pItem);
			m_ToRemove.Remove(pItem);
		}

		public bool Remove(Updateable pItem)
		{
			if (m_Updateables.Contains(pItem) && !m_ToRemove.Contains(pItem))
			{
				m_ToRemove.Add(pItem);
				m_ToAdd.Remove(pItem);
				return true;
			}
			return false;
		}

		public void Update(float pDeltaTime)
		{
			ClearToAdd();
			ClearToRemove();
			foreach (Updateable updatable in m_Updateables)
			{
				updatable.Action.Invoke(pDeltaTime);
			}
		}

		private void ClearToRemove()
		{
			while (m_ToRemove.Count > 0)
			{
				m_Updateables.Remove(m_ToRemove[0]);
				m_ToRemove.RemoveAt(0);
			}
		}

		private void ClearToAdd()
		{
			while (m_ToAdd.Count > 0)
			{
				AddInternal(m_ToAdd[0]);
				m_ToAdd.RemoveAt(0);
			}
		}

		private void AddInternal(in Updateable pItem)
		{
			int index;
			for (index = 0; index < m_Updateables.Count; index++)
			{
				if (m_Updateables[index].Priority <= pItem.Priority)
				{
					break;
				}
			}
			m_Updateables.Insert(index, pItem);
		}
	}
}
