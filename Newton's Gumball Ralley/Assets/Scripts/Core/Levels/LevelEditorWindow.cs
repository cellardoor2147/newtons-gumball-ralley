using UnityEngine;
using UnityEditor;

namespace Core.Levels
{
#if(UNITY_EDITOR)
    public class LevelEditorWindow : EditorWindow
    {
        private int worldIndex;
        private int levelIndex;
        private string customLevelName = "";
        private int repeatedBackgroundColumns;
        private int repeatedBackgroundRows;
        private bool shouldUseTimeConstraint;
        private float timeConstraint;
        private bool shouldUseScrapConstraint;
        private float scrapConstraint;
        private float placeableScrapLimit;

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
                LevelSerializer.Serialize(
                    worldIndex,
                    levelIndex,
                    customLevelName,
                    shouldUseTimeConstraint,
                    timeConstraint,
                    shouldUseScrapConstraint,
                    scrapConstraint,
                    repeatedBackgroundColumns,
                    repeatedBackgroundRows,
                    placeableScrapLimit
                );
            }
            if (GUILayout.Button("Load Level"))
            {
                string readFilePath = EditorUtility.OpenFilePanel(
                    "Select level JSON to load",
                    LevelSerializer.WRITE_DIRECTORY_PATH,
                    "json"
                );
                LevelData levelData =
                    LevelSerializer.DeserializeFromReadFilePath(readFilePath);
                LevelManager.LoadLevelWithLevelData(levelData);
                worldIndex = levelData.worldIndex;
                levelIndex = levelData.levelIndex;
                customLevelName = levelData.customLevelName;
                repeatedBackgroundColumns = levelData.repeatedBackgroundColumns;
                repeatedBackgroundRows = levelData.repeatedBackgroundRows;
                shouldUseTimeConstraint = levelData.starConditions.shouldUseTimeConstraint;
                timeConstraint = levelData.starConditions.timeConstraint;
                shouldUseScrapConstraint = levelData.starConditions.shouldUseScrapConstraint;
                scrapConstraint = levelData.starConditions.scrapConstraint;
                placeableScrapLimit = levelData.placeableScrapLimit;
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
            GUILayout.Label("Background Settings", EditorStyles.boldLabel);
            repeatedBackgroundColumns =
                EditorGUI.IntField(new Rect(0, 15, position.width, 15), "# of Columns", repeatedBackgroundColumns);
            repeatedBackgroundRows =
                EditorGUI.IntField(new Rect(0, 30, position.width, 15), "# of Rows", repeatedBackgroundRows);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 180, position.width, 90));
            GUILayout.Label("Star Constraints", EditorStyles.boldLabel);
            shouldUseTimeConstraint =
                EditorGUI.Toggle(new Rect(0, 15, position.width, 15), "Use Time Constraint", shouldUseTimeConstraint);
            timeConstraint = 
                EditorGUI.FloatField(new Rect(0, 30, position.width, 15), "Time Constraint", timeConstraint);
            shouldUseScrapConstraint =
                EditorGUI.Toggle(new Rect(0, 45, position.width, 15), "Use Scrap Constraint", shouldUseScrapConstraint);
            scrapConstraint =
                EditorGUI.FloatField(new Rect(0, 60, position.width, 15), "Scrap Constraint", scrapConstraint);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 270, position.width, 30));
            GUILayout.Label("Scrap Limit", EditorStyles.boldLabel);
            placeableScrapLimit =
                EditorGUI.FloatField(new Rect(0, 15, position.width, 15), "Scrap Limit", placeableScrapLimit);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 315, position.width, 1000));
            GUILayout.Label("How To Use", EditorStyles.boldLabel);
            GUILayout.Label(GetHowToUseText(), EditorStyles.helpBox);
            GUILayout.EndArea();
        }

        private string GetHowToUseText()
        {
            return "---Saving a level---"
                + "\n1. Place environment blocks in the 'Environment' game object in the scene hierarchy"
                + "\n2. Place preplaced simple machines in the 'Preplaced Objects' game object in the scene hierarchy"
                + "\n3. Move the Gumnall Machine (ball spawn point) object wherever you want to it be for this level"
                + "\n4. Add desired world/level indices, background settings, and star constraints for the resulting level"
                + "\n5. [Optional] Add custom level name above to save w/ a custom name (don't do this if you're saving an official level)"
                + "\n6. Press 'Save Level' button above (it should save automatically)"
                + "\nNOTE: 'Environment', 'Preplaced Objects', and 'Gumball Machine' should be at the highest level in the hierarchy"
                + "\n\n---Loading a level---"
                + "\n1. Press 'Load Level' button above"
                + "\n2. Select the level to load (should be a .json file)"
                + "\n3. Press 'Open' to load";
        }
    }
#endif
}
