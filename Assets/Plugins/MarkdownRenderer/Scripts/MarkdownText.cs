using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

[RequireComponent(typeof(TMP_Text))]
[ExecuteAlways]
public class MarkdownText : MonoBehaviour
{
    [TextArea(15, 15)]
    [Tooltip("Please write the markdown somewhere else and then copy it here")]
    public string markdown;

    [Tooltip("Update Text in Edit Mode")]
    public bool executeInEditMode = false;

    private TextMeshProUGUI textMeshProUGUI;
    private TMP_StyleSheet styleSheet;

    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        styleSheet = Resources.Load<TMP_StyleSheet>("MarkdownStyleSheet");

        if ((executeInEditMode || Application.isPlaying) && textMeshProUGUI != null)
        {
            UpdateText();
        }
    }

    private void OnValidate()
    {
        styleSheet = Resources.Load<TMP_StyleSheet>("MarkdownStyleSheet");

        if ((executeInEditMode || Application.isPlaying) && textMeshProUGUI != null)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        if (styleSheet == null)
        {
            Debug.LogError("MarkdownStyleSheet in Resources folder missing!");
        }
        else if (styleSheet != null && textMeshProUGUI.styleSheet != styleSheet)
        {
            textMeshProUGUI.styleSheet = styleSheet;
        }

        string parsedText = ParseMarkdown(markdown);
        textMeshProUGUI.text = parsedText;
    }

    private string ParseMarkdown(string markdown)
    {
        // Headers with relative sizes based on defaultTextSize and bold tags
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
}
