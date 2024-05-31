using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarkdownOutlineButton : MonoBehaviour
{
    public TMP_Text buttonText;
    private float headerPosition;
    private RectTransform scrollRect;
    private AnimationCurve animationCurve;
    private MarkdownDocs markdownDocs;

    public void Setup(string headerText, int headerLevel, RectTransform scrollRect, AnimationCurve animationCurve, MarkdownDocs instance)
    {      
        this.scrollRect = scrollRect;
        this.animationCurve = animationCurve;
        markdownDocs = instance;

        // Set the button text with indentation
        buttonText.text = new string(' ', (headerLevel - 1) * 4) + headerText;

        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    public void SetHeaderPosition(float headerPosition)
    {
        this.headerPosition = headerPosition;
    }

    private void OnButtonClicked()
    {
        Coroutine animationRoutine = markdownDocs.animationRoutine;

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        markdownDocs.animationRoutine = StartCoroutine(LerpPosition(headerPosition, 1f)); // 1f is the duration of the lerp
    }

    private IEnumerator LerpPosition(float targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = scrollRect.localPosition;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = animationCurve.Evaluate(t);
            scrollRect.localPosition = new Vector3(startPosition.x, Mathf.Lerp(startPosition.y, targetPosition, curveValue), startPosition.z);

            time += Time.deltaTime;
            yield return null;
        }

        scrollRect.localPosition = new Vector3(startPosition.x, targetPosition, startPosition.z);
    }
}
