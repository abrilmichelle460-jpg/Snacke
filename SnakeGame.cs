using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pon este script en un GameObject vacío de la escena "Game". No necesita nada más.
public class SnakeGame : MonoBehaviour
{
    public int size = 20;
    public float tick = 0.12f;
    public AudioClip eatSfx, dieSfx, bgm; // opcionales: si están vacíos se generan sonidos

    List<Vector2Int> body = new List<Vector2Int>();
    List<SpriteRenderer> parts = new List<SpriteRenderer>();
    Transform[] eyes = new Transform[4]; // 2 ojos + 2 pupilas
    Vector2Int dir = Vector2Int.right, nextDir = Vector2Int.right, food;
    Transform foodObj;
    Sprite sq;
    AudioSource sfx;
    float timer;
    int score, best;
    bool dead;
    Texture2D panelTex;
    Transform root;

    void Start()
    {
        root = new GameObject("SnakeWorld").transform;
        var t = new Texture2D(1, 1); t.SetPixel(0, 0, Color.white); t.Apply();
        sq = Sprite.Create(t, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        panelTex = new Texture2D(1, 1); panelTex.SetPixel(0, 0, new Color(0, 0, 0, 0.6f)); panelTex.Apply();

        var cam = Camera.main;
        if (cam == null) { var cg = new GameObject("Main Camera"); cg.tag = "MainCamera"; cam = cg.AddComponent<Camera>(); }
        cam.clearFlags = CameraClearFlags.SolidColor; cam.orthographic = true; cam.orthographicSize = size / 2f + 2;
        cam.transform.position = new Vector3(size / 2f - 0.5f, size / 2f - 0.5f, -10);
        cam.backgroundColor = new Color(0.05f, 0.09f, 0.08f);

        sfx = root.gameObject.AddComponent<AudioSource>();
        if (eatSfx == null) eatSfx = Beep(880f, 0.08f);
        if (dieSfx == null) dieSfx = Beep(150f, 0.45f);
        if (bgm != null) { var m = root.gameObject.AddComponent<AudioSource>(); m.clip = bgm; m.loop = true; m.volume = 0.3f; m.Play(); }
        best = PlayerPrefs.GetInt("best", 0);

        BuildBoard();
        foodObj = Make(new Color(0.95f, 0.25f, 0.25f), 3, 0.75f).transform;
        for (int i = 0; i < 4; i++) eyes[i] = Make(i < 2 ? Color.white : Color.black, 6, i < 2 ? 0.28f : 0.14f).transform;
        Reset();
    }

    // Tablero con cuadros alternados y borde
    void BuildBoard()
    {
        var c1 = new Color(0.16f, 0.30f, 0.22f); var c2 = new Color(0.19f, 0.34f, 0.25f);
        var wall = new Color(0.30f, 0.20f, 0.12f);
        for (int x = -1; x <= size; x++)
            for (int y = -1; y <= size; y++)
            {
                bool edge = x < 0 || y < 0 || x >= size || y >= size;
                var sr = Make(edge ? wall : ((x + y) % 2 == 0 ? c1 : c2), -10, 1f);
                sr.transform.position = new Vector3(x, y, 0);
            }
    }

    SpriteRenderer Make(Color c, int order, float scale)
    {
        var go = new GameObject("p");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sq; sr.color = c; sr.sortingOrder = order;
        go.transform.SetParent(root, false);
        go.transform.localScale = Vector3.one * scale;
        return sr;
    }

    void Reset()
    {
        foreach (var p in parts) Destroy(p.gameObject);
        parts.Clear(); body.Clear();
        for (int i = 0; i < 3; i++) body.Add(new Vector2Int(size / 2 - i, size / 2));
        dir = nextDir = Vector2Int.right;
        score = 0; dead = false; timer = 0;
        SyncParts(); PlaceFood();
    }

    void SyncParts()
    {
        while (parts.Count < body.Count) parts.Add(Make(Color.green, 2, 0.9f));
        for (int i = 0; i < body.Count; i++)
        {
            float k = body.Count > 1 ? (float)i / (body.Count - 1) : 0;
            parts[i].transform.position = new Vector3(body[i].x, body[i].y, 0);
            parts[i].transform.localScale = Vector3.one * Mathf.Lerp(0.95f, 0.65f, k);
            parts[i].color = dead ? Color.Lerp(new Color(0.9f, 0.3f, 0.3f), new Color(0.5f, 0.1f, 0.1f), k)
                                  : Color.Lerp(new Color(0.40f, 1f, 0.45f), new Color(0.10f, 0.55f, 0.35f), k);
        }
        // Ojos según la dirección
        var h = new Vector3(body[0].x, body[0].y, 0);
        var f = new Vector3(dir.x, dir.y, 0); var p = new Vector3(-dir.y, dir.x, 0);
        eyes[0].position = h + f * 0.12f + p * 0.22f; eyes[1].position = h + f * 0.12f - p * 0.22f;
        eyes[2].position = h + f * 0.2f + p * 0.22f;  eyes[3].position = h + f * 0.2f - p * 0.22f;
    }

    void PlaceFood()
    {
        do food = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        while (body.Contains(food));
        foodObj.position = new Vector3(food.x, food.y, 0);
    }

    void Update()
    {
        foodObj.localScale = Vector3.one * (0.7f + 0.08f * Mathf.Sin(Time.time * 6f));

        if (dead)
        {
            if (Inp.Restart()) Reset();
            if (Inp.Esc()) GoMenu();
            return;
        }
        if (Inp.Up()) Turn(Vector2Int.up);
        if (Inp.Down()) Turn(Vector2Int.down);
        if (Inp.Left()) Turn(Vector2Int.left);
        if (Inp.Right()) Turn(Vector2Int.right);
        if (Inp.Esc()) { GoMenu(); return; }

        timer += Time.deltaTime;
        if (timer < tick) return;
        timer = 0; dir = nextDir;

        var head = body[0] + dir;
        if (head.x < 0 || head.y < 0 || head.x >= size || head.y >= size || body.GetRange(0, body.Count - 1).Contains(head))
        { Die(); return; }

        body.Insert(0, head);
        if (head == food)
        {
            score++; sfx.PlayOneShot(eatSfx); PlaceFood();
            tick = Mathf.Max(0.06f, tick - 0.003f); // sube la velocidad poco a poco
        }
        else body.RemoveAt(body.Count - 1);
        SyncParts();
    }

    void Turn(Vector2Int d) { if (d + dir != Vector2Int.zero) nextDir = d; }

    void GoMenu()
    {
        if (Application.CanStreamedLevelBeLoaded("Menu")) SceneManager.LoadScene("Menu");
        else { Destroy(root.gameObject); gameObject.AddComponent<MenuManager>(); Destroy(this); }
    }

    void Die()
    {
        dead = true; sfx.PlayOneShot(dieSfx);
        if (score > best) { best = score; PlayerPrefs.SetInt("best", best); }
        SyncParts();
    }

    void OnGUI()
    {
        var lbl = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        lbl.normal.textColor = Color.white;

        GUI.DrawTexture(new Rect(Screen.width / 2f - 200, 10, 400, 46), panelTex);
        GUI.Label(new Rect(Screen.width / 2f - 200, 10, 400, 46), "Puntos: " + score + "   |   Récord: " + best, lbl);

        // Botones en pantalla (por si el teclado no responde)
        if (GUI.Button(new Rect(15, 10, 110, 46), "MENÚ")) { GoMenu(); return; }
        float bx = Screen.width - 190, by = Screen.height - 190;
        if (!dead)
        {
            if (GUI.Button(new Rect(bx + 65, by, 60, 60), "▲")) Turn(Vector2Int.up);
            if (GUI.Button(new Rect(bx, by + 65, 60, 60), "◄")) Turn(Vector2Int.left);
            if (GUI.Button(new Rect(bx + 130, by + 65, 60, 60), "►")) Turn(Vector2Int.right);
            if (GUI.Button(new Rect(bx + 65, by + 130, 60, 60), "▼")) Turn(Vector2Int.down);
        }

        if (dead)
        {
            if (GUI.Button(new Rect(Screen.width / 2f - 100, Screen.height / 2f + 125, 200, 50), "REINICIAR")) Reset();
            var r = new Rect(Screen.width / 2f - 260, Screen.height / 2f - 110, 520, 220);
            GUI.DrawTexture(r, panelTex);
            var title = new GUIStyle(lbl) { fontSize = 52 }; title.normal.textColor = new Color(1f, 0.4f, 0.4f);
            GUI.Label(new Rect(r.x, r.y + 15, r.width, 80), "GAME OVER", title);
            GUI.Label(new Rect(r.x, r.y + 95, r.width, 40), "Puntaje final: " + score, lbl);
            var small = new GUIStyle(lbl) { fontSize = 20 }; small.normal.textColor = new Color(0.8f, 0.9f, 0.8f);
            GUI.Label(new Rect(r.x, r.y + 150, r.width, 40), "R = reiniciar   ·   Esc = menú", small);
        }
    }

    AudioClip Beep(float freq, float seconds)
    {
        int rate = 44100, n = (int)(rate * seconds);
        var data = new float[n];
        for (int i = 0; i < n; i++) data[i] = Mathf.Sin(2 * Mathf.PI * freq * i / rate) * 0.3f * (1f - (float)i / n);
        var clip = AudioClip.Create("beep", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
