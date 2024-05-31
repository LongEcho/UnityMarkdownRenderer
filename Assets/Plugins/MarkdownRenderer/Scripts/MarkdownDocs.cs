using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class MarkdownDocs : MonoBehaviour
{
    [Tooltip("Controls the size of the font")]
    public float defaultTextSize = 48;

    [TextArea(15, 15)]
    [Tooltip("Please write the markdown somewhere else and then copy it here")]
    public string markdown;

    [Tooltip("Used to create text as a child")]
    public GameObject textMeshProPrefab;

    [Header("Outline")]
    [Tooltip("Creates a Outline with pressable buttons, check the demo.")]
    public bool createOutlineFromHeaders;

    [Tooltip("Button prefab that is used to fill the outline. Check the demo")]
    public GameObject buttonPrefab;

    [Tooltip("This will be the parent for the buttonPrefab.")]
    public Transform buttonContainer;

    [Tooltip("The scroll rect of the textmeshpro markdown renderer. Used to jump to a specific point after outline button click")]
    public RectTransform scrollRect;

    [Tooltip("Use this to control the animation when a outline button has been pressed.")]
    public AnimationCurve scrollAnimation = new AnimationCurve(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 1f, 2f, 0f));

    [Header("Experimental")]
    [Tooltip("This can cause issues. Be careful!")]
    public bool executeInEditMode;

    private List<TextMeshProUGUI> TMPTexts;
    private List<GameObject> headerObjects;
    [HideInInspector]
    public Coroutine animationRoutine;

    private void Awake()
    {
        if (Application.isPlaying || executeInEditMode)
        {
            UpdateText();
        }
    }

    private void OnValidate()
    {
        if (executeInEditMode)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        ClearOldTextObjects();

        TMPTexts = new List<TextMeshProUGUI>();
        headerObjects = new List<GameObject>();

        string[] lines = markdown.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        string currentTextBlock = "";
        GameObject currentTextObject = null;
        TextMeshProUGUI currentText = null;

        foreach (string line in lines)
        {
            if (line.TrimStart().StartsWith("#"))
            {
                // Create a new TextMeshPro object for the previous text block
                if (!string.IsNullOrEmpty(currentTextBlock))
                {
                    CreateTextObject(ref currentTextObject, ref currentText, currentTextBlock);
                    currentTextBlock = "";
                }

                // Add the heading to the current text block
                currentTextBlock += line + "\n";
            }
            else
            {
                // Append the line to the current text block
                currentTextBlock += line + "\n";
            }
        }

        // Create the last TextMeshPro object for the remaining text block
        if (!string.IsNullOrEmpty(currentTextBlock))
        {
            CreateTextObject(ref currentTextObject, ref currentText, currentTextBlock);
        }

        if (createOutlineFromHeaders)
        {
            CreateOutlineButtons();
        }
    }

    private void ClearOldTextObjects()
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in scrollRect.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            foreach (Transform child in scrollRect.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            foreach (Transform child in buttonContainer)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private string ParseMarkdown(string markdown)
    {
        // Headers
        markdown = Regex.Replace(markdown, @"###### (.*)", $"<style=\"Header6\">$1</style>");
        markdown = Regex.Replace(markdown, @"##### (.*)", $"<style=\"Header5\">$1</style>");
        markdown = Regex.Replace(markdown, @"#### (.*)", $"<style=\"Header4\">$1</style>");
        markdown = Regex.Replace(markdown, @"### (.*)", $"<style=\"Header3\">$1</style>");
        markdown = Regex.Replace(markdown, @"## (.*)", $"<style=\"Header2\">$1</style>");
        markdown = Regex.Replace(markdown, @"# (.*)", $"<style=\"Header1\">$1</style>");

        // Bold
        markdown = Regex.Replace(markdown, @"\*\*(.*?)\*\*", "<style=\"Bold\">$1</style>");
        markdown = Regex.Replace(markdown, @"__(.*?)__", "<style=\"Bold\">$1</style>");

        // Italic
        markdown = Regex.Replace(markdown, @"\*(.*?)\*", "<style=\"Italic\">$1</style>");
        markdown = Regex.Replace(markdown, @"_(.*?)_", "<style=\"Italic\">$1</style>");

        // Strike
        markdown = Regex.Replace(markdown, @"~~(.*?)~~", "<style=\"Strike\">$1</style>");

        // Highlight
        markdown = Regex.Replace(markdown, @"==(.*?)==", "<style=\"Highlight\">$1</style>");

        // Monospace
        markdown = Regex.Replace(markdown, @"`(.*?)`", "<style=\"Monospace\"> $1 </style>");

        // Links
        markdown = Regex.Replace(markdown, @"\[(.*?)\]\((.*?)\)", "<style=\"Link\"><link=\"$2\">$1</link></style>");

        return markdown;
    }

    private void CreateTextObject(ref GameObject currentTextObject, ref TextMeshProUGUI currentText, string currentTextBlock)
    {
        currentTextObject = Instantiate(textMeshProPrefab, scrollRect.transform);
        currentText = currentTextObject.GetComponent<TextMeshProUGUI>();
        currentText.fontSize = defaultTextSize;
        TMPTexts.Add(currentText);

        string parsedTextBlock = ParseMarkdown(currentTextBlock);
        currentText.text = parsedTextBlock;

        headerObjects.Add(currentTextObject);
    }

    private void CreateOutlineButtons()
    {
        // Clear existing buttons
        if (Application.isPlaying)
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            foreach (Transform child in buttonContainer)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Match headers and create buttons
        MatchCollection matches = Regex.Matches(markdown, @"^(#+) (.*)", RegexOptions.Multiline);

        int i = 0;
        foreach (Match match in matches)
        {
            int headerLevel = match.Groups[1].Value.Length;
            string headerText = match.Groups[2].Value;

            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            MarkdownOutlineButton outlineButton = button.GetComponent<MarkdownOutlineButton>();
            outlineButton.Setup(headerText, headerLevel, scrollRect, scrollAnimation, this);
            headerObjects[i].GetComponent<TMP_Text>().ForceMeshUpdate();
            StartCoroutine(SetHeaderPositions(headerObjects[i], outlineButton));
            i++;
        }
    }

    public IEnumerator<WaitForEndOfFrame> SetHeaderPositions(GameObject headerObject, MarkdownOutlineButton button)
    {
        yield return new WaitForEndOfFrame();
        float headerPos = 0 - headerObject.GetComponent<RectTransform>().localPosition.y;
        button.SetHeaderPosition(headerPos);
    }
}
