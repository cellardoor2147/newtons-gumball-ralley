using UnityEngine;
using UnityEditor;

namespace Core
{
    public class LevelEditorWindow : EditorWindow
    {
        private int worldIndex;
        private int levelIndex;
        private string customLevelName = "";

        [MenuItem("Window/Level Editor")]
        private static void Init()
        {
            LevelEditorWindow window =
                (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Save Level"))
            {
                LevelSerializer.Serialize(worldIndex, levelIndex, customLevelName);
            }
            if (GUILayout.Button("Load Level"))
            {
                string readFilePath =
                    EditorUtility.OpenFilePanel("Select level JSON to load", "", "json");
                LevelData levelData = LevelSerializer.Deserialize(readFilePath);
                worldIndex = levelData.worldIndex;
                levelIndex = levelData.levelIndex;
                customLevelName = levelData.customLevelName;
            }
            GUILayout.Label("Name Settings", EditorStyles.boldLabel);
            worldIndex =
                EditorGUI.IntField(new Rect(0, 60, position.width, 15), "World (1-8)", worldIndex);
            levelIndex =
                EditorGUI.IntField(new Rect(0, 75, position.width, 15), "Level (1-3)", levelIndex);
            customLevelName =
                EditorGUI.TextField(new Rect(0, 90, position.width, 15), "Custom Level Name", customLevelName);
        }
    }
}
