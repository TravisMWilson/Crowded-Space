using TMPro;
using UnityEngine;

public class InputFieldValidator : MonoBehaviour {
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;
    [SerializeField] private int defaultValue;

    void Start() => inputField.onEndEdit.AddListener(ValidateInput);

    private void ValidateInput(string input) {
        if (int.TryParse(input, out int value)) {
            value = Mathf.Clamp(value, minValue, maxValue);
            inputField.text = value.ToString();
        } else {
            inputField.text = defaultValue.ToString();
        }
    }
}
