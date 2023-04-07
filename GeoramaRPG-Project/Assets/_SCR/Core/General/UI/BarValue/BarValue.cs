using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using OliverLoescher;

public class BarValue : MonoBehaviour
{
	[HideInInspector] public GameObject root => transform.parent.parent.gameObject; // Override .gameObject to ensure they are referencing the root object

	[SerializeField] private Image topBar = null;
	[SerializeField] private Image bottemBar = null;
	[SerializeField, ShowIf("@doFadeIn")] private Image backgroudBar = null;
	private Coroutine moveRoutine = null;

	[Header("Colors")]
	[Tooltip("Leave null if color changing is not desired")]
	[SerializeField] private Image coloringImage = null;
	[SerializeField] private Image secondColoringImage = null;
	[HideIf("@coloringImage == null"), SerializeField] private Color defaultColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	[HideIf("@secondColoringImage == null"), SerializeField] private Color secondDefaultColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
	[HideIf("@coloringImage == null || doHealColor == false"), SerializeField] private Color healColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	[HideIf("@coloringImage == null"), SerializeField] private Color toggledColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
	[HideIf("@secondColoringImage == null"), SerializeField] private Color secondToggledColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
	[HideIf("@coloringImage == null"), SerializeField] private Color inactiveColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);
	[HideIf("@secondColoringImage == null"), SerializeField] private Color secondInactiveColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

	[Header("Timings")]
	[SerializeField, Min(0.0f)] private float delay = 0.75f;
	[Tooltip("Seconds for bar to fill from 0% to 100% (0% to 50% will take half the amount of seconds)")]
	[SerializeField, DisableInPlayMode, Min(Util.NEARZERO)] private float seconds = 1.0f;
	private float inverseSeconds;

	[Space]
	[ShowIf("@doColorFades && coloringImage != null"), DisableInPlayMode, SerializeField, Min(Util.NEARZERO)] private float colorSeconds = 0.1f;
	private float inverseColorSeconds;

	[Space]
	[SerializeField, DisableInPlayMode, ShowIf("@doFadeIn"), Min(0.0f)] private float fadeInSeconds = 1.0f;
	private float inverseFadeInSeconds;
	[SerializeField, Min(0.0f), ShowIf("@doFadeIn && fadeOutType != FadeOutType.Null")] private float fadeOutDelay = 2.0f;
	[SerializeField, DisableInPlayMode, ShowIf("@doFadeIn && fadeOutType != FadeOutType.Null"), Min(0.0f)] private float fadeOutSeconds = 1.0f;
	private float inverseFadeOutSeconds;

	[Header("Options")]
	[SerializeField] private bool useCurve = false;
	[SerializeField, ShowIf("@useCurve")] private AnimationCurve valueCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f, -1.0f, 1.0f), new Keyframe(1.0f, 1.0f, -1.0f, 1.0f));
	[HideIf("@coloringImage == null"), SerializeField] private bool doHealColor = false;
	[HideIf("@coloringImage == null"), SerializeField] private bool doColorFades = false;
	[SerializeField] private bool doFadeIn = false;
	[SerializeField, ShowIf("@doFadeIn")] private FadeOutType fadeOutType = FadeOutType.Null;

	private bool isToggled = false;

	private Coroutine colorRoutine = null;
	private Coroutine secondColorRoutine = null;

	private Coroutine fadeRoutine = null;
	private bool isFadedOut = false;
	private float fadeOutTime = 0.0f;

	private void Awake() 
	{
		inverseSeconds = 1 / seconds;
		inverseColorSeconds = 1 / colorSeconds;
		inverseFadeInSeconds = 1 / fadeInSeconds;
		inverseFadeOutSeconds = 1 / fadeOutSeconds;

		InitValue(1.0f);
	}

	public void InitValue(float pValue01)
	{
		if (useCurve)
			pValue01 = valueCurve.Evaluate(pValue01);

		if (moveRoutine != null)
			StopCoroutine(moveRoutine);

		bottemBar.fillAmount = pValue01;
		topBar.fillAmount = pValue01;
		
		SetColor(defaultColor, secondDefaultColor, true);

		FadeInit();
	}

	private void Update() 
	{
		UpdateFade();
	}

	private void OnEnable() 
	{
		if (isToggled)
			SetColor(toggledColor, secondToggledColor);
		else
			SetColor(defaultColor, secondDefaultColor);
	}

	private void OnDisable() 
	{
		if (colorRoutine != null)
			StopCoroutine(colorRoutine);
		SetColor(inactiveColor, secondInactiveColor, true);
	}

	public void SetToggled(bool pToggled)
	{
		isToggled = pToggled;
		if (isToggled)
			SetColor(toggledColor, secondToggledColor);
		else
			SetColor(defaultColor, secondDefaultColor);
	}

	public void SetValue(float pValue01)
	{
		if (!isActiveAndEnabled)
		{
			SetValueInstant(pValue01);
			return;
		}

		if (useCurve)
			pValue01 = valueCurve.Evaluate(pValue01);

		if (moveRoutine != null)
			StopCoroutine(moveRoutine);

		if (pValue01 > topBar.fillAmount)
		{
			moveRoutine = StartCoroutine(TopBarRoutine(pValue01));
			bottemBar.fillAmount = pValue01;

			if (doHealColor)
				SetColor(isToggled ? toggledColor : healColor);
		}
		else
		{
            moveRoutine = StartCoroutine(BottemBarRoutine(pValue01));
            topBar.fillAmount = pValue01;
            
            if (doHealColor)
            {
                if (isToggled)
                    SetColor(toggledColor, secondToggledColor);
                else
                    SetColor(defaultColor, secondDefaultColor);
            }
        }
        
        ResetFade();
    }

	public void SetValueInstant(float pValue01)
	{
		if (useCurve)
			pValue01 = valueCurve.Evaluate(pValue01);

		if (moveRoutine != null)
			StopCoroutine(moveRoutine);

		bottemBar.fillAmount = pValue01;
		topBar.fillAmount = pValue01;
	}

    private IEnumerator TopBarRoutine(float pValue01)
    {
        yield return new WaitForSeconds(delay);

        while (topBar.fillAmount < pValue01)
        {
            topBar.fillAmount = Mathf.Min(pValue01, topBar.fillAmount + (Time.deltaTime * inverseSeconds));
            yield return null;
        }
    }

    private IEnumerator BottemBarRoutine(float pValue01)
    {
        yield return new WaitForSeconds(delay);

        while (bottemBar.fillAmount > pValue01)
        {
            bottemBar.fillAmount = Mathf.Max(pValue01, bottemBar.fillAmount - (Time.deltaTime * inverseSeconds));
            yield return null;
        }
    }

#region Color
    public void SetColor(Color pColor, bool pInstant = false)
    {
        SetColor(coloringImage, pColor, colorRoutine, pInstant);
    }

    public void SetColor(Color pColor, Color pSecondColor, bool pInstant = false)
    {
        SetColor(coloringImage, pColor, colorRoutine, pInstant);
        SetColor(secondColoringImage, pSecondColor, secondColorRoutine, pInstant);
    }

    private void SetColor(Image pImage, Color pColor, Coroutine pRoutine, bool pInstant = false)
    {
        if (pImage != null)
        {
            if (doColorFades && !pInstant && isActiveAndEnabled)
            {
                if (pRoutine != null)
                    StopCoroutine(pRoutine);
                pRoutine = StartCoroutine(SetColorRoutine(pImage, pColor));
            }
            else
            {
                SetImageColor(pImage, pColor);
            }
        }
    }

    private IEnumerator SetColorRoutine(Image pImage, Color pColor)
    {
        // Check color
        Color startColor = pImage.color;
        if (startColor != pColor)
        {
            // Transition color
            float progress01 = 0;
            while (progress01 < 1)
            {
                progress01 += Time.deltaTime * inverseColorSeconds;
                SetImageColor(pImage, Color.Lerp(startColor, pColor, progress01));
                yield return null;
            }
        }
    }

    // This is called any time the color of the image is to be set
    private void SetImageColor(Image pImage, Color pColor)
    {
        pColor.a = pImage.color.a;
        pImage.color = pColor;
    }
#endregion

#region Fading
    private void FadeInit()
    {
        if (doFadeIn) // If should do any fading
        {
            SetImageAlpha(topBar, 0.0f);
            SetImageAlpha(bottemBar, 0.0f);
            SetImageAlpha(backgroudBar, 0.0f);
            isFadedOut = true;
        }
        else
        {
            fadeOutType = FadeOutType.Null;
        }
    }

    private void ResetFade()
    {
        if (doFadeIn) // If should do any fading
        {
            if (isFadedOut) // Fade In
            {
                if (fadeRoutine != null)
                    StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeRoutine(false, inverseFadeInSeconds));
            }
             
            if (fadeOutType != FadeOutType.Null) // Fade Out
            {
                fadeOutTime = Time.time + fadeOutDelay;
            }
        }
    }

    private void UpdateFade()
    {
        if (fadeOutType == FadeOutType.Always || (topBar.fillAmount == 1 && fadeOutType == FadeOutType.OnlyWhenMax))
            if (isFadedOut == false && Time.time >= fadeOutTime) // Fade Out
            {
                if (fadeRoutine != null)
                    StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeRoutine(true, inverseFadeOutSeconds));
            }
    }

    private IEnumerator FadeRoutine(bool pOut, float pInverseSeconds)
    {
        isFadedOut = pOut;

        // Fade
        float progress01 = 0;
        while (progress01 < 1)
        {
            progress01 += Time.deltaTime * pInverseSeconds;
            float v;

            if (pOut)
                v = Mathf.Lerp(1, 0, progress01);
            else
                v = Mathf.Lerp(0, 1, progress01);

            SetImageAlpha(topBar, v);
            SetImageAlpha(bottemBar, v);
            SetImageAlpha(backgroudBar, v);
            yield return null;
        }
    }

    private void SetImageAlpha(Image pImage, float pAlpha)
    {
        Color c = pImage.color;
        c.a = pAlpha;
        pImage.color = c;
    }

    public enum FadeOutType
    {
        Null,
        OnlyWhenMax,
        Always
    }
#endregion
}