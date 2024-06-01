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

    public void Setup(string headerText, int headerLevel, MarkdownDocs instance)
    {
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
        markdownDocs.LerpPosition(headerPosition);
    }


}
