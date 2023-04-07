using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class DamageNumbers : MonoBehaviour
{
#region Singleton
    public static DamageNumbers Instance = null;
    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
#endregion

    [SerializeField] [DisableInPlayMode] private GameObject textPrefab = null;
    [SerializeField] [DisableInPlayMode] private int textCount = 12;
    private TMP_Text[] texts = new TMP_Text[0];
    private Coroutine[] textRoutines = new Coroutine[0];
    private int index = 0;

    [Space]
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private float seconds = 0.5f;
    [SerializeField] private float randomOffset = 0.2f;

    private void Start()
    {
        texts = new TMP_Text[textCount];
        textRoutines = new Coroutine[textCount];
        for (int i = 0; i < textCount; i++)
        {
            texts[i] = Instantiate(textPrefab, transform).GetComponent<TMP_Text>();
            texts[i].gameObject.SetActive(false);
        }
    }

    public void DisplayNumber(string pText, Vector3 pPosition, Color pColor)
    {
        index++;
        if (index == textCount)
            index = 0;

        texts[index].gameObject.SetActive(true);
        texts[index].text = pText;
        //textsMat[index].SetColor("_EmissionColor", pColor);

        Vector2 offset = Random.insideUnitCircle * randomOffset * 0.5f;
        pPosition += new Vector3(offset.x, 0, offset.y);

        if (textRoutines[index] != null)
            StopCoroutine(textRoutines[index]);
        textRoutines[index] = StartCoroutine(TextRoutine(texts[index], pPosition, pColor));
    }

    private IEnumerator TextRoutine(TMP_Text pText, Vector3 pPosition, Color pColor)
    {
        Vector3 curPosition = pPosition;

        float progress = 0;
        while (progress < 2)
        {
            pColor.a = alphaCurve.Evaluate(progress);
            pText.color = pColor;

            curPosition = pPosition + new Vector3(0, positionCurve.Evaluate(progress), 0);
            pText.transform.position = curPosition;

            progress += Time.deltaTime / seconds;
            yield return new WaitForEndOfFrame();
        }

        pText.gameObject.SetActive(false);
    }
}
