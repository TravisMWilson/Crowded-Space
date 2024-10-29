using UnityEngine;

[System.Serializable]
public class SoundData {
    public AudioClip clip;
    public int volume;
    public bool is3D;

    public SoundData(AudioClip clip, int volume, bool is3D) {
        this.clip = clip;
        this.volume = volume;
        this.is3D = is3D;
    }
}