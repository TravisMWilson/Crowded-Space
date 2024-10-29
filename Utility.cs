using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class Utility : MonoBehaviour {
    public static bool IsMoving = false;
    public static bool InDelay = false;
    public static bool StopCoroutines = false;

    private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qdt", "Qnt", "Sxt", "Spt", "Oct", "Nnt", "Dct" };

    public static Utility Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static string FormatNumber(long number) {
        if (number == 0) return "0";

        int suffixIndex = 0;
        double tempNumber = number;

        while (Math.Abs(tempNumber) >= 1000 && suffixIndex < Suffixes.Length - 1) {
            tempNumber /= 1000;
            suffixIndex++;
        }

        return tempNumber.ToString("0.#") + Suffixes[suffixIndex];
    }

    public static void ApplyToAllChildren(Transform parent, Action<Transform> action) {
        foreach (Transform child in parent) {
            action(child);
            ApplyToAllChildren(child, action);
        }
    }

    public static Color ParseColorFromRGBAString(string colorString) {
        string pattern = @"RGBA\(\s*([\d\.]+)\s*,\s*([\d\.]+)\s*,\s*([\d\.]+)\s*,\s*([\d\.]+)\s*\)";
        Match match = Regex.Match(colorString, pattern, RegexOptions.IgnoreCase);

        if (match.Success) {
            float r = float.Parse(match.Groups[1].Value);
            float g = float.Parse(match.Groups[2].Value);
            float b = float.Parse(match.Groups[3].Value);
            float a = float.Parse(match.Groups[4].Value);

            return new Color(r, g, b, a);
        } else {
            return Color.black;
        }
    }

    public static Vector3 ParseVector3FromString(string vectorString) {
        string pattern = @"\(\s*([\d\.\-]+)\s*,\s*([\d\.\-]+)\s*,\s*([\d\.\-]+)\s*\)";
        Match match = Regex.Match(vectorString, pattern);

        if (match.Success) {
            float x = float.Parse(match.Groups[1].Value);
            float y = float.Parse(match.Groups[2].Value);
            float z = float.Parse(match.Groups[3].Value);

            return new Vector3(x, y, z);
        } else {
            return Vector3.zero;
        }
    }

    public static Quaternion ParseQuaternionFromString(string quaternionString) {
        string pattern = @"\(\s*([\d\.\-]+)\s*,\s*([\d\.\-]+)\s*,\s*([\d\.\-]+)\s*,\s*([\d\.\-]+)\s*\)";
        Match match = Regex.Match(quaternionString, pattern);

        if (match.Success) {
            float x = float.Parse(match.Groups[1].Value);
            float y = float.Parse(match.Groups[2].Value);
            float z = float.Parse(match.Groups[3].Value);
            float w = float.Parse(match.Groups[4].Value);

            return new Quaternion(x, y, z, w);
        } else {
            return Quaternion.identity;
        }
    }

    public static void ClearChildObjects(Transform parent) {
        foreach (Transform child in parent) {
            Destroy(child.gameObject);
        }
    }

    public static string ConvertHoursToTimeFormat(double hours) {
        int wholeHours = (int)hours;
        int minutes = (int)((hours - wholeHours) * 60);

        return $"{wholeHours:D2}:{minutes:D2}";
    }

    public static string ConvertMinutesToTimeFormat(double minutes) {
        int wholeMinutes = (int)minutes;
        int seconds = (int)((minutes - wholeMinutes) * 60);

        return $"{wholeMinutes:D2}:{seconds:D2}";
    }

    public static string ConvertSecondsToHourTimeFormat(float seconds) {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt(seconds % 3600 / 60);
        return $"{hours:D2}:{minutes:D2}";
    }

    public static string ConvertSecondsToMinuteTimeFormat(float seconds) {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:D2}:{remainingSeconds:D2}";
    }

    public static float ConvertHoursToSeconds(float hours) {
        int wholeHours = (int)hours;
        int minutes = (int)((hours - wholeHours) * 60);
        return (wholeHours * 3600) + (minutes * 60);
    }

    public static float ConvertMinutesToSeconds(int minutes) => minutes * 60f;

    public static void MoveChildrenAndDestroy(Transform originalParent) {
        GameObject newParent = new GameObject("NewParent");
        newParent.transform.SetPositionAndRotation(originalParent.position, originalParent.rotation);

        foreach (Transform child in originalParent) {
            child.SetParent(newParent.transform);
        }

        Destroy(newParent);
    }

    public void DoIfNotMoving(float delay, Action action) {
        if (IsMoving) return;
        IsMoving = true;

        action();

        Delay(delay, () => IsMoving = false);
    }

    public static Bounds GetTransformBounds(Transform parent) {
        Bounds bounds = new Bounds(parent.position, Vector3.zero);
        bool hasBounds = false;

        foreach (Transform child in parent) {
            if (child.GetComponent<ParticleSystem>() != null) continue;

            if (child.TryGetComponent<Renderer>(out var renderer)) {
                if (!hasBounds) {
                    bounds = renderer.bounds;
                    hasBounds = true;
                } else {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }

        return bounds;
    }

    public static Bounds GetUnrotatedBounds(Transform parent) {
        Bounds bounds = new Bounds(parent.position, Vector3.zero);
        bool hasBounds = false;

        foreach (Transform child in parent) {
            if (child.GetComponent<ParticleSystem>() != null) continue;

            if (child.TryGetComponent<Renderer>(out var renderer)) {
                Bounds localBounds = renderer.bounds;
                Vector3 localCenter = parent.InverseTransformPoint(localBounds.center);
                Vector3 localExtents = renderer.bounds.extents;

                localBounds.center = localCenter;
                localBounds.extents = localExtents;

                if (!hasBounds) {
                    bounds = localBounds;
                    hasBounds = true;
                } else {
                    bounds.Encapsulate(localBounds);
                }
            }
        }

        return bounds;
    }

    public static string AddSpacesBeforeCapitals(string input) => Regex.Replace(input, @"(?<!^)(?=[A-Z])", " ");

    public void Delay(float duration, Action action) => _ = StartCoroutine(StartDelay(duration, action));
    public void MoveTransform(RectTransform panel, Vector2 startPos, Vector2 endPos, float moveDuration) => _ = StartCoroutine(StartMoveTransform(panel, startPos, endPos, moveDuration));
    public void MoveTransform(Transform transform, float verticalDistance, float horizontalDistance, float duration, float delay = 0.0f) => _ = StartCoroutine(StartMoveTransform(transform, verticalDistance, horizontalDistance, duration, delay));
    public void ScaleTransform(Transform transform, bool grow, float duration, bool visible = true, float delay = 0.0f) => _ = StartCoroutine(StartScaleTransform(transform, grow, duration, visible, delay));
    public void FadeCanvasGroup(CanvasGroup group, float fadeDuration, bool fadeIn = false, float delay = 0.0f) => _ = StartCoroutine(StartFadeCanvasGroup(group, fadeDuration, fadeIn, delay));
    public void LerpToParent(GameObject lerpObject, GameObject parent, float duration = 1f, float lerpSpeed = 5f) => _ = StartCoroutine(StartLerpToParent(lerpObject, parent, duration, lerpSpeed));

    private IEnumerator StartLerpToParent(GameObject lerpObject, GameObject parent, float duration = 1f, float lerpSpeed = 5f) {
        if (lerpObject == null) yield break;

        Vector3 startPosition = lerpObject.transform.position;

        float time = 0f;

        while (time < duration) {
            time += Time.deltaTime * lerpSpeed;

            Vector3 endPosition = parent.transform.position;

            if (lerpObject != null) {
                lerpObject.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            } else {
                yield break;
            }

            yield return null;
        }

        if (lerpObject != null) {
            lerpObject.transform.position = parent.transform.position;
        }
    }

    private IEnumerator StartDelay(float duration, Action action) {
        float elapsedTime = 0.0f;

        InDelay = true;

        while (elapsedTime < duration && !StopCoroutines) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        action();

        InDelay = false;
    }

    private IEnumerator StartMoveTransform(RectTransform panel, Vector2 startPos, Vector2 endPos, float moveDuration) {
        float elapsedTime = 0.0f;

        while (elapsedTime < moveDuration && !StopCoroutines) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            float sineT = Mathf.Sin(t * Mathf.PI * 0.5f);

            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, sineT);
            yield return null;
        }

        panel.anchoredPosition = endPos;
    }

    private IEnumerator StartMoveTransform(Transform transform, float verticalDistance, float horizontalDistance, float duration, float delay = 0.0f) {
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startPosition + new Vector3(horizontalDistance, verticalDistance, 0);
        float elapsedTime = 0.0f;

        while (elapsedTime < duration + delay && !StopCoroutines) {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= delay) {
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime - delay) / duration);
            }

            yield return null;
        }

        transform.localPosition = endPosition;
    }

    private IEnumerator StartScaleTransform(Transform transform, bool grow, float duration, bool visible = true, float delay = 0.0f) {
        Vector3 startScale = grow ? Vector3.zero : Vector3.one;
        Vector3 endScale = grow ? Vector3.one : Vector3.zero;
        float elapsedTime = 0.0f;

        transform.gameObject.SetActive(true);

        while (elapsedTime < duration + delay && !StopCoroutines) {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= delay) {
                transform.localScale = Vector3.Lerp(startScale, endScale, (elapsedTime - delay) / duration);
            }

            yield return null;
        }

        transform.localScale = endScale;
        transform.gameObject.SetActive(visible);
    }

    private IEnumerator StartFadeCanvasGroup(CanvasGroup group, float fadeDuration, bool fadeIn = false, float delay = 0.0f) {
        float fadeAmount = fadeIn ? 0.0f : 1.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration + delay && !StopCoroutines) {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= delay) {
                fadeAmount = fadeIn ? Mathf.Clamp01((elapsedTime - delay) / fadeDuration) : Mathf.Clamp01(1.0f - ((elapsedTime - delay) / fadeDuration));
            }

            group.alpha = fadeAmount;
            yield return null;
        }

        group.alpha = fadeIn ? 1.0f : 0.0f;
    }
}
