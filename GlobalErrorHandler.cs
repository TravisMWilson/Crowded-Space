using System.IO;
using UnityEngine;

public class GlobalErrorHandler : MonoBehaviour {
    private string logFilePath;

    void Start() {
        logFilePath = Path.Combine(Application.persistentDataPath, "ErrorLog.txt");
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type) {
        if (type is LogType.Error or LogType.Exception) {
            string errorMessage = $"[{type}] {logString}\n{stackTrace}\n";
            //PopupManager.Instance.ShowPopup("Error captured: " + errorMessage, PopupType.OK);

            File.AppendAllText(logFilePath, errorMessage);
        }
    }

    private void OnDestroy() => Application.logMessageReceived -= HandleLog;
}
