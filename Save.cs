using UnityEngine;

public class Save {
    public static void Int(string name, int value) {
        PlayerPrefs.SetInt(name, value);
        PlayerPrefs.Save();
    }

    public static void Float(string name, float value) {
        PlayerPrefs.SetFloat(name, value);
        PlayerPrefs.Save();
    }

    public static void String(string name, string value) {
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }
}
