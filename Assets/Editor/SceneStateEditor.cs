using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SceneState))]
public class SceneStateEditor : Editor {

    SceneState sceneState;

    public override void OnInspectorGUI() {

        bool menuState = GUILayout.Toggle(sceneState.inMenuState, "In Menu State");
        if (menuState != sceneState.inMenuState)
        {
            sceneState.inMenuState = menuState;
            if (menuState)
            {
                sceneState.SetMenuState();
            }
            else
            {
                sceneState.SetPlayableState();
            }
        }

        DrawDefaultInspector();
    }

    void OnEnable() {
        sceneState = (SceneState)target;
    }
  
}