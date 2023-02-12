using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutEngine))]
public class LayoutEngineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(15);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.y += 10;
        lastRect.height = 2;
        lastRect.width = Screen.width - 30;
        lastRect.x = 15;
        GUI.Box(lastRect, "");
        GUILayout.Space(5);

        LayoutEngine layoutEngine = (LayoutEngine)target;
        if (GUILayout.Button("Clean"))
        {
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
        }
        if (GUILayout.Button("Generate"))
        {
            layoutEngine.Generate();
        }

        if (GUILayout.Button("Regenerate"))
        {
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Clean();
            layoutEngine.Generate();
        }
    }
}
