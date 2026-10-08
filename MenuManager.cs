using UnityEngine;
using UnityEngine.SceneManagement;

// Pon este script en un GameObject vacío de la escena "Menu".
public class MenuManager : MonoBehaviour
{
    Texture2D bg, btn, btnHover, panel;

    Texture2D Tex(Color c) { var t = new Texture2D(1, 1); t.SetPixel(0, 0, c); t.Apply(); return t; }

    void Start()
    {
        if (Camera.main) { Camera.main.clearFlags = CameraClearFlags.SolidColor; Camera.main.backgroundColor = new Color(0.05f, 0.09f, 0.08f); }
        bg = Tex(new Color(0.05f, 0.09f, 0.08f));
        panel = Tex(new Color(0.12f, 0.22f, 0.16f, 0.9f));
        btn = Tex(new Color(0.20f, 0.55f, 0.30f));
        btnHover = Tex(new Color(0.30f, 0.75f, 0.40f));
    }

    void Update() { if (Inp.Esc()) Salir(); }

    void Salir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg);
        float cx = Screen.width / 2f, cy = Screen.height / 2f;
        GUI.DrawTexture(new Rect(cx - 250, cy - 240, 500, 480), panel);

        var shadow = new GUIStyle(GUI.skin.label) { fontSize = 54, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        shadow.normal.textColor = new Color(0, 0, 0, 0.5f);
        GUI.Label(new Rect(cx - 250 + 4, cy - 220 + 4, 500, 110), "NEON SNAKE", shadow);
        var title = new GUIStyle(shadow); title.normal.textColor = new Color(0.45f, 1f, 0.5f);
        GUI.Label(new Rect(cx - 250, cy - 220, 500, 110), "NEON SNAKE", title);

        var sub = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
        sub.normal.textColor = new Color(0.8f, 0.9f, 0.8f);
        GUI.Label(new Rect(cx - 250, cy - 120, 500, 40), "Flechas o WASD para moverte", sub);
        GUI.Label(new Rect(cx - 250, cy - 90, 500, 40), "Récord: " + PlayerPrefs.GetInt("best", 0), sub);
        var ver = new GUIStyle(sub) { fontSize = 16 }; ver.normal.textColor = new Color(0.6f, 0.75f, 0.6f);
        GUI.Label(new Rect(cx - 250, cy + 190, 500, 30), "v1.1", ver);

        var s = new GUIStyle(GUI.skin.button) { fontSize = 30, fontStyle = FontStyle.Bold };
        s.normal.background = btn; s.hover.background = btnHover; s.active.background = btnHover;
        s.normal.textColor = s.hover.textColor = s.active.textColor = Color.white;

        if (GUI.Button(new Rect(cx - 130, cy - 20, 260, 70), "JUGAR", s))
        {
            if (Application.CanStreamedLevelBeLoaded("Game")) SceneManager.LoadScene("Game");
            else { gameObject.AddComponent<SnakeGame>(); Destroy(this); }
        }
        if (GUI.Button(new Rect(cx - 130, cy + 70, 260, 70), "SALIR", s))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
