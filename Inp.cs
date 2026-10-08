using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Lee el teclado con el Input System nuevo o el antiguo, según lo que tenga tu Unity.
public static class Inp
{
#if ENABLE_INPUT_SYSTEM
    static bool P(Key k) { return Keyboard.current != null && Keyboard.current[k].wasPressedThisFrame; }
    public static bool Up()      { return P(Key.UpArrow) || P(Key.W); }
    public static bool Down()    { return P(Key.DownArrow) || P(Key.S); }
    public static bool Left()    { return P(Key.LeftArrow) || P(Key.A); }
    public static bool Right()   { return P(Key.RightArrow) || P(Key.D); }
    public static bool Restart() { return P(Key.R) || P(Key.Space) || P(Key.Enter); }
    public static bool Esc()     { return P(Key.Escape); }
#else
    public static bool Up()      { return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W); }
    public static bool Down()    { return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S); }
    public static bool Left()    { return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A); }
    public static bool Right()   { return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D); }
    public static bool Restart() { return Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return); }
    public static bool Esc()     { return Input.GetKeyDown(KeyCode.Escape); }
#endif
}
