using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StateMachineSettings : ScriptableObject
{
    public const string k_MyCustomSettingsPath = "Assets/MyCustomSettings.asset";

    [SerializeField]
    private List<UiNameInput> _pairsList = new List<UiNameInput>(); 

    public List<UiNameInput> PairsList { get => _pairsList;}

    public static StateMachineSettings GetOrCreateSettings()
    {
#if UNITY_EDITOR
        var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<StateMachineSettings>(k_MyCustomSettingsPath);

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<StateMachineSettings>();
            if(!Directory.Exists("Assets/"))
            {
                Directory.CreateDirectory("Assets/");
            }
            UnityEditor.AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return settings;
#endif
        return null;
    }

    public static StateMachineSettings GetSettings()
    {
        return GetOrCreateSettings();
    }

}
