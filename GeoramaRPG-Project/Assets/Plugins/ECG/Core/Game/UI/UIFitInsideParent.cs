using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIFitInsideParent : MonoBehaviour {
	public float horizontalInset, verticalInset;
	public bool allowUpscale = false;

	void Rescale()
	{
		var rt = GetComponent<RectTransform>();
		if (rt == null)
			return;
		var pRt = rt.parent.GetComponent<RectTransform>();
		if (pRt == null)
			return;
		var w = rt.rect.width + horizontalInset * 2f + Mathf.Abs(rt.localPosition.x) * 2f;
		var h = rt.rect.height + verticalInset * 2f + Mathf.Abs(rt.localPosition.y) * 2f;
		var ar = w / h;
		var pW = pRt.rect.width;
		var pH = pRt.rect.height;
		var pAr = pW / pH;
		var scaleBefore = rt.localScale.x;
		var scaleAfter = allowUpscale ? 1000f : 1f;
		if (ar > pAr)
			scaleAfter = Mathf.Min(pW / w, scaleAfter);
		else
			scaleAfter = Mathf.Min(pH / h, scaleAfter);
		if (!Core.Util.Approximately(scaleAfter, scaleBefore))
			rt.localScale = new Vector3(scaleAfter, scaleAfter, rt.localScale.z);
	}

	void OnEnable()
	{
		Rescale();
	}

	void LateUpdate()
	{
		Rescale();
	}
}
