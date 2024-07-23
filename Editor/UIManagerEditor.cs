using UnityEditor;
[CustomEditor(typeof(UiManager))]
public class UIManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var UiManager = target as UiManager;
        if (UiManager.settings == null)
        {
            UiManager.settings = StateMachineSettings.GetOrCreateSettings();
            PrefabUtility.RecordPrefabInstancePropertyModifications(UiManager.settings);
        }
    }

}


