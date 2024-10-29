using UnityEngine;

public class Load {
    public static int Int(string name) => PlayerPrefs.GetInt(name, 0);
    public static float Float(string name) => PlayerPrefs.GetFloat(name, 0.0f);
    public static string String(string name) => PlayerPrefs.GetString(name, string.Empty);
}