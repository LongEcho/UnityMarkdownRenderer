using UnityEditor;
using UnityEngine;

public class EditorMarkdownPrefab : MonoBehaviour
{
    [MenuItem("GameObject/Markdown/Text", false, 10)]
    static void CreateMarkdownTextPrefab()
    {
        CreateMarkdownPrefab("MarkdownTextPrefab", "Markdown Text");
    }

    [MenuItem("GameObject/Markdown/Docs", false, 10)]
    static void CreateMarkdownDocsPrefab()
    {
        CreateMarkdownPrefab("MarkdownDocsPrefab", "Markdown Docs");
    }

    static void CreateMarkdownPrefab(string prefabName, string objectName)
    {
        // Load your custom prefab from the Resources folder
        GameObject prefab = Resources.Load<GameObject>(prefabName);

        if (prefab != null)
        {
            // Instantiate the prefab in the scene
            GameObject instance = Instantiate(prefab);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);

            // If a parent object is selected, set it as the parent of the new instance
            if (Selection.activeTransform != null)
            {
                instance.transform.SetParent(Selection.activeTransform, false);
            }

            instance.name = objectName;

            // Select the newly created instance in the hierarchy
            Selection.activeObject = instance;
        }
        else
        {
            Debug.LogError("Prefab not found! Make sure it is placed in a Resources folder.");
        }
    }
}
