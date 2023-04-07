using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher.Georama
{
    public class BuildingHighlightBehaviour : HighlightModelBehaviour
    {
		public enum State
		{
			None,
			Hover,
			Selected,
			Dragging,
			Invalid
		}

		[SerializeField, ColorPalette("Buildings")]
		private Color hoverColor = Color.gray;
		[SerializeField, ColorPalette("Buildings")]
		private Color selectedColor = Color.white;
		[SerializeField, ColorPalette("Buildings")]
		private Color draggingColor = Color.cyan;
		[SerializeField, ColorPalette("Buildings")]
		private Color invalidColor = Color.red;

		[Button]
		public void Set(State pState)
		{
			switch (pState)
			{
				case State.None:
					Clear();
					break;
				case State.Hover:
					Set(hoverColor);
					break;
				case State.Selected:
					Set(selectedColor);
					break;
				case State.Dragging:
					Set(draggingColor);
					break;
				case State.Invalid:
					Set(invalidColor);
					break;
				default:
					throw new System.NotImplementedException();
			}
		}
	}
}
