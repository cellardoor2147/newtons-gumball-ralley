using UnityEngine;
using UnityEditor;

namespace Core
{
#if(UNITY_EDITOR)
    public class LevelEditorWindow : EditorWindow
    {
        private int worldIndex;
        private int levelIndex;
        private string customLevelName = "";
        private float timeConstraint;
        private float scrapConstraint;

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
                LevelSerializer.Serialize(worldIndex, levelIndex, customLevelName, timeConstraint, scrapConstraint);
            }
            if (GUILayout.Button("Load Level"))
            {
                string readFilePath = EditorUtility.OpenFilePanel(
                    "Select level JSON to load",
                    LevelSerializer.WRITE_DIRECTORY_PATH,
                    "json"
                );
                LevelData levelData = LevelSerializer.Deserialize(readFilePath);
                LevelManager.LoadLevelWithLevelData(levelData);
                worldIndex = levelData.worldIndex;
                levelIndex = levelData.levelIndex;
                customLevelName = levelData.customLevelName;
            }

            GUILayout.BeginArea(new Rect(0, 45, position.width, 60));
            GUILayout.Label("Name Settings", EditorStyles.boldLabel);
            worldIndex =
                EditorGUI.IntField(new Rect(0, 15, position.width, 15), "World (1-8)", worldIndex);
            levelIndex =
                EditorGUI.IntField(new Rect(0, 30, position.width, 15), "Level (1-3)", levelIndex);
            customLevelName =
                EditorGUI.TextField(new Rect(0, 45, position.width, 15), "Custom Level Name", customLevelName);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 120, position.width, 60));
            GUILayout.Label("Star Constraints", EditorStyles.boldLabel);
            timeConstraint = 
                EditorGUI.FloatField(new Rect(0, 15, position.width, 15), "Time Constraint", timeConstraint);
            scrapConstraint =
                EditorGUI.FloatField(new Rect(0, 30, position.width, 15), "Scrap Constraint", scrapConstraint);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 180, position.width, 1000));
            GUILayout.Label("How To Use", EditorStyles.boldLabel);
            GUILayout.Label(GetHowToUseText(), EditorStyles.helpBox);
            GUILayout.EndArea();
        }

        private string GetHowToUseText()
        {
            return "---Saving a level---"
                + "\n1. In the hierarchy, go to the 'Background' game object, then to its child game object. Set the desired number of rows/columns for the repeated background here"
                + "\n2. Place environment blocks in the 'Environment' game object in the scene hierarchy"
                + "\n3. Place preplaced simple machines in the 'Preplaced Objects' game object in the scene hierarchy"
                + "\n4. Move the sling anchor (ball slingshot) object wherever you want to it be for this level"
                + "\n5. Add desired world/level indices above for the resulting level"
                + "\n6. [Optional] Add custom level name above to save w/ a custom name (don't do this if you're saving an official level)"
                + "\n7. Press 'Save Level' button above (it should save automatically)"
                + "\nNOTE: 'Background', 'Environment', 'Preplaced Objects', and 'Sling Anchor' should be at the highest level in the hierarchy"
                + "\n\n---Loading a level---"
                + "\n1. Press 'Load Level' button above"
                + "\n2. Select the level to load (should be a .json file)"
                + "\n3. Press 'Open' to load";
        }
    }
#endif
}
