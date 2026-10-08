using UnityEngine;

// Hace que el juego arranque SOLO al pulsar Play, aunque la escena esté vacía.
public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (Object.FindObjectOfType<MenuManager>() != null || Object.FindObjectOfType<SnakeGame>() != null) return;
        if (Camera.main == null)
        {
            var c = new GameObject("Main Camera"); c.tag = "MainCamera"; c.AddComponent<Camera>();
        }
        new GameObject("Bootstrap").AddComponent<MenuManager>();
    }
}
