/*
CREDIT GOES TO: fafase
https://forum.unity.com/threads/clickable-link-within-a-text.910226/
*/

using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class OpenHyperlink : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text m_textMeshPro;
    public Camera m_uiCamera;
    void Start()
    {
        m_textMeshPro = GetComponent<TMP_Text>();
        if (m_uiCamera = null)
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].CompareTag("UICamera")) // this may be whatever for your case
                {
                    m_uiCamera = cameras[i];
                    break;
                }
            }

            if (m_uiCamera = null)
            {
                m_uiCamera = Camera.main;
            }

            if (m_uiCamera = null)
            {
                Debug.LogWarning("Markdown Text: OpenHyperLink could not find a Camera. Please attach one to make Hyperlinks work");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_textMeshPro, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = m_textMeshPro.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
