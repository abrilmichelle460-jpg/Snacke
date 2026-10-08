using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Crea solo las escenas Menu y Game y configura el Build Settings.
[InitializeOnLoad]
public static class SnakeSetup
{
    static SnakeSetup()
    {
        PlayerSettings.productName = "Neon Snake";
        PlayerSettings.bundleVersion = "1.1";
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!File.Exists("Assets/Scenes/Menu.unity") || !File.Exists("Assets/Scenes/Game.unity")) Build();
        };
    }

    [MenuItem("Snake/Crear escenas y configurar build")]
    static void Build()
    {
        Directory.CreateDirectory("Assets/Scenes");

        var menu = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        new GameObject("MenuManager").AddComponent<MenuManager>();
        EditorSceneManager.SaveScene(menu, "Assets/Scenes/Menu.unity");

        var game = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        new GameObject("GameManager").AddComponent<SnakeGame>();
        EditorSceneManager.SaveScene(game, "Assets/Scenes/Game.unity");

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Menu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true)
        };
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        Debug.Log("Snake listo: escenas Menu y Game creadas. Abre Menu y pulsa Play.");
    }
}
