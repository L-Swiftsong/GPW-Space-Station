using UnityEngine;

public static class PlayerSettings
{
    static PlayerSettings()
    {
        // Initialise PlayerSettings.
        UpdateSettingsFromPlayerPrefs();
    }
    /// <summary> Update our current settings (If PlayerPrefs values exist use them, otherwise use our defaults).</summary>
    public static void UpdateSettingsFromPlayerPrefs()
    {
        // Mouse.
        MouseHorizontalSensititvity = PlayerPrefs.GetFloat("MouseVerticalSensitivity", MouseSensitivityRange.Default.x);
        MouseVerticalSensititvity = PlayerPrefs.GetFloat("MouseVerticalSensitivity", MouseSensitivityRange.Default.y);
        MouseInvertY = PlayerPrefs.GetInt("MouseInvertYAxis", 0) == 1;

        // Gamepad.
        GamepadHorizontalSensititvity = PlayerPrefs.GetFloat("GamepadHorizontalSensitivity", GamepadSensitivityRange.Default.x);
        GamepadVerticalSensititvity = PlayerPrefs.GetFloat("GamepadVerticalSensitivity", GamepadSensitivityRange.Default.y);
        GamepadInvertY = PlayerPrefs.GetInt("GamepadInvertYAxis", 0) == 1;

        // General.
        ToggleCrouch = PlayerPrefs.GetInt("ToggleCrouch", 0) == 1;
        ToggleSprint = PlayerPrefs.GetInt("ToggleSprint", 0) == 1;
        ToggleInventory = PlayerPrefs.GetInt("ToggleInventory", 0) == 1;
    }
    /// <summary> Save our current settings to PlayerPrefs.</summary>
    public static void SaveSettingsToPlayerPrefs()
    {
        // Mouse.
        PlayerPrefs.SetFloat("MouseVerticalSensitivity", MouseHorizontalSensititvity);
        PlayerPrefs.SetFloat("MouseVerticalSensitivity", MouseVerticalSensititvity);
        PlayerPrefs.SetInt("MouseInvertYAxis", MouseInvertY ? 1 : 0);

        // Gamepad.
        PlayerPrefs.SetFloat("GamepadHorizontalSensitivity", GamepadHorizontalSensititvity);
        PlayerPrefs.SetFloat("GamepadVerticalSensitivity", GamepadVerticalSensititvity);
        PlayerPrefs.SetInt("GamepadInvertYAxis", GamepadInvertY ? 1 : 0);

        // General.
        PlayerPrefs.SetInt("ToggleCrouch", ToggleCrouch ? 1 : 0);
        PlayerPrefs.SetInt("ToggleSprint", ToggleSprint ? 1 : 0);
        PlayerPrefs.SetInt("ToggleInventory", ToggleInventory ? 1 : 0);


        // Perform a PlayerPrefs save.
        PlayerPrefs.Save();
    }


    // Mouse.
    public static (Vector2 Min, Vector2 Max, Vector2 Default) MouseSensitivityRange => (Min: new Vector2(x: 1.0f, y: 1.0f), Max: new Vector2(x: 50.0f, y: 50.0f), Default: new Vector2(10.0f, 7.5f));
    public static float MouseHorizontalSensititvity { get; set; }
    public static float MouseVerticalSensititvity { get; set; }

    public static bool MouseInvertY { get; set; }


    // Gamepad.
    public static (Vector2 Min, Vector2 Max, Vector2 Default) GamepadSensitivityRange => (Min: new Vector2(x: 50.0f, y: 50.0f), Max: new Vector2(x: 300.0f, y: 300.0f), Default: new Vector2(175.0f, 150.0f));
    public static float GamepadHorizontalSensititvity { get; set; }
    public static float GamepadVerticalSensititvity { get; set; }
    
    public static bool GamepadInvertY { get; set; }



    // Assorted.
    public static bool ToggleCrouch { get; set; }
    public static bool ToggleSprint { get; set; }
    public static bool ToggleInventory { get; set; }
}
