using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Crosshair : MonoBehaviour
{
	[FoldoutGroup("Outline"), SerializeField] private bool outline = true;
	[FoldoutGroup("Outline"), SerializeField] private Color outlineColor = Color.black;
	[FoldoutGroup("Outline"), SerializeField, Min(0.0f)] private float outlineScale = 0.35f;

	[FoldoutGroup("Center Dot"), SerializeField] private bool centerDot = true;
	[FoldoutGroup("Center Dot"), SerializeField] private Color centerDotColor = Color.green;
	[FoldoutGroup("Center Dot"), SerializeField, Min(0.0f)] private Vector2 centerDotScale = Vector2.one;

	[FoldoutGroup("Inner Lines"), SerializeField] private bool innerLines = true;
	[FoldoutGroup("Inner Lines"), SerializeField] private Color innerColor = Color.green;
	[FoldoutGroup("Inner Lines"), SerializeField, Min(0.0f)] private float innerLength = 1.0f;
	[FoldoutGroup("Inner Lines"), SerializeField, Min(0.0f)] private float innerThickness = 1.0f;
	[FoldoutGroup("Inner Lines"), SerializeField, Min(0.0f)] private float innerOffset = 1.0f;

	[FoldoutGroup("Outer Lines"), SerializeField] private bool outerLines = true;
	[FoldoutGroup("Outer Lines"), SerializeField] private Color outerColor = Color.green;
	[FoldoutGroup("Outer Lines"), SerializeField, Min(0.0f)] private float outerLength = 1.0f;
	[FoldoutGroup("Outer Lines"), SerializeField, Min(0.0f)] private float outerThickness = 1.0f;
	[FoldoutGroup("Outer Lines"), SerializeField, Min(0.0f)] private float outerOffset = 1.0f;

	[Header("References")]
	[SerializeField] private Image centerDotImage = null;
	[SerializeField] private Image centerDotOutlineImage = null;
	
	[Space]
	[SerializeField] private Image innerLeftImage = null;
	[SerializeField] private Image innerLeftOutlineImage = null;
	[SerializeField] private Image innerRightImage = null;
	[SerializeField] private Image innerRightOutlineImage = null;
	[SerializeField] private Image innerUpImage = null;
	[SerializeField] private Image innerUpOutlineImage = null;
	[SerializeField] private Image innerDownImage = null;
	[SerializeField] private Image innerDownOutlineImage = null;
	
	[Space]
	[SerializeField] private Image outerLeftImage = null;
	[SerializeField] private Image outerLeftOutlineImage = null;
	[SerializeField] private Image outerRightImage = null;
	[SerializeField] private Image outerRightOutlineImage = null;
	[SerializeField] private Image outerUpImage = null;
	[SerializeField] private Image outerUpOutlineImage = null;
	[SerializeField] private Image outerDownImage = null;
	[SerializeField] private Image outerDownOutlineImage = null;

	private void Update() 
	{
		// Center
		SetLine(centerDotImage, centerDotOutlineImage, Vector2.zero, centerDotScale, centerDotColor, centerDot);

		// Inner
		Vector2 scale = new Vector2(innerLength, innerThickness);
		SetLine(innerLeftImage, innerLeftOutlineImage, new Vector2(-innerOffset, 0.0f), scale, innerColor, innerLines);
		SetLine(innerRightImage, innerRightOutlineImage, new Vector2(innerOffset, 0.0f), scale, innerColor, innerLines);
		
		scale = new Vector2(innerThickness, innerLength);
		SetLine(innerUpImage, innerUpOutlineImage, new Vector2(0.0f, -innerOffset), scale, innerColor, innerLines);
		SetLine(innerDownImage, innerDownOutlineImage, new Vector2(0.0f, innerOffset), scale, innerColor, innerLines);

		// Outer
		scale = new Vector2(outerLength, outerThickness);
		SetLine(outerLeftImage, outerLeftOutlineImage, new Vector2(-outerOffset, 0.0f), scale, outerColor, outerLines);
		SetLine(outerRightImage, outerRightOutlineImage, new Vector2(outerOffset, 0.0f), scale, outerColor, outerLines);
		
		scale = new Vector2(outerThickness, outerLength);
		SetLine(outerUpImage, outerUpOutlineImage, new Vector2(0.0f, -outerOffset), scale, outerColor, outerLines);
		SetLine(outerDownImage, outerDownOutlineImage, new Vector2(0.0f, outerOffset), scale, outerColor, outerLines);
	}

	private void SetLine(Image pImage, Image pOutline, Vector2 pPosition, Vector2 pScale, Color pColor, bool pEnabled)
	{
		if (pEnabled)
		{
			// Image
			pImage.gameObject.SetActive(true);
			pImage.rectTransform.localPosition = pPosition;
			pImage.rectTransform.localScale = new Vector2(pScale.x + 0.1f, pScale.y + 0.1f);
			pImage.color = pColor;

			// Image outline
			pOutline.gameObject.SetActive(outline);
			if (outline)
			{
				pOutline.rectTransform.localPosition = pPosition;
				pOutline.rectTransform.localScale = new Vector2(pScale.x + outlineScale + 0.1f, pScale.y + outlineScale + 0.1f);
				pOutline.color = outlineColor;
			}
		}
		else
		{
			pImage.gameObject.SetActive(false);
			pOutline.gameObject.SetActive(false);
		}
	}

	// private void OnGUI()
	// {
	//	 Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

	//	 if (centerDot)
	//	 {
	//		 DrawDot(center, centerDotScale, centerDotColor);
	//	 }
		
	//	 if (innerLines)
	//	 {
	//		 // Left
	//		 Vector2 pos = center + (Vector2.left * innerOffset);
	//		 Vector2 scale = new Vector2(innerLength, innerThickness);
	//		 DrawDot(pos, scale, centerDotColor);
			
	//		 // Right
	//		 pos = center + (Vector2.right * innerOffset);
	//		 // scale = new Vector2(innerLength, innerThickness);
	//		 DrawDot(pos, scale, centerDotColor);

	//		 // Up
	//		 pos = center + (Vector2.up * innerOffset);
	//		 scale = new Vector2(innerThickness, innerLength);
	//		 DrawDot(pos, scale, centerDotColor);
			
	//		 // Down
	//		 pos = center + (Vector2.down * innerOffset);
	//		 // scale = new Vector2(innerLength, innerThickness);
	//		 DrawDot(pos, scale, centerDotColor);
	//	 }
	// }
	
	// private void DrawQuadSimple(Rect pPosition, Color pColor) 
	// {
	//	 Texture2D texture = new Texture2D(1, 1);
	//	 texture.SetPixel(0, 0, pColor);
	//	 texture.Apply();
	//	 GUI.skin.box.normal.background = texture;
	//	 GUI.Box(pPosition, GUIContent.none);
	// }

	// private void DrawOulineQuad(Rect pPosition, Color pColor)
	// {
	//	 if (outline)
	//	 {
	//		 pPosition.height += outlineScale * 6.0f;
	//		 pPosition.width += outlineScale * 6.0f;
	//		 pPosition.position -= Vector2.one * outlineScale * 3.0f;
	//		 DrawQuadSimple(pPosition, pColor);
	//	 }
	// }

	// private void DrawDot(Vector2 pPosition, Vector2 pScale, Color pColor)
	// {
	//	 Rect rect = new Rect(pPosition - (pScale * 3.0f),  pScale * 6.0f);

	//	 // Outline
	//	 if (outline)
	//		 DrawOulineQuad(rect, pColor);

	//	 // Line
	//	 DrawQuadSimple(rect, pColor);
	// }
}
