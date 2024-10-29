using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    [SerializeField] private int initialPoolSize = 10;

    [SerializeField] private List<SoundData> soundClips = new List<SoundData>();
    [SerializeField] private List<SoundData> singleInstanceSounds = new List<SoundData>();

    private Dictionary<string, AudioSource> playingSingleInstanceSounds = new Dictionary<string, AudioSource>();
    private List<AudioSource> audioSourcePool = new List<AudioSource>();

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < initialPoolSize; i++) {
            _ = AddAudioSourceToPool();
        }
    }

    public void PlaySound(string soundName) {
        var soundData = soundClips.Find(sound => sound.clip.name == soundName);

        if (soundData.clip != null) {
            PlayClip(soundData.clip, soundData.volume, Vector3.zero, soundData.is3D, soundName);
        }
    }

    public void PlaySingleInstanceSound(string soundName) {
        var soundData = singleInstanceSounds.Find(sound => sound.clip.name == soundName);

        if (soundData.clip != null) {
            PlayClip(soundData.clip, soundData.volume, Vector3.zero, soundData.is3D, soundName, isSingleInstance: true);
        }
    }

    public void StopSound(string soundName) {
        if (playingSingleInstanceSounds.ContainsKey(soundName)) {
            AudioSource source = playingSingleInstanceSounds[soundName];
            source.Stop();
            _ = playingSingleInstanceSounds.Remove(soundName);
        } else {
            foreach (var source in audioSourcePool) {
                if (source.clip != null && source.clip.name == soundName) {
                    source.Stop();
                    break;
                }
            }
        }
    }

    public void SetGlobalVolume(float volume) {
        foreach (var source in audioSourcePool) {
            source.volume = volume / 100f;
        }
    }

    public void PlayRandomSound(List<string> soundNames) {
        if (soundNames.Count == 0) return;

        int randomIndex = Random.Range(0, soundNames.Count);
        PlaySound(soundNames[randomIndex]);
    }

    public void LoadSound(string soundPath, bool is3D) {
        AudioClip clip = Resources.Load<AudioClip>(soundPath);

        if (clip != null) {
            soundClips.Add(new SoundData(clip, 100, is3D));
        }
    }

    public List<string> GetCurrentlyPlayingSounds() {
        List<string> currentlyPlaying = new List<string>();

        foreach (var sound in playingSingleInstanceSounds.Keys) {
            currentlyPlaying.Add(sound);
        }

        foreach (var source in audioSourcePool) {
            if (source.isPlaying && source.clip != null) {
                currentlyPlaying.Add(source.clip.name);
            }
        }

        return currentlyPlaying;
    }

    public void FadeIn(string soundName, float duration, Vector3 position) {
        if (playingSingleInstanceSounds.TryGetValue(soundName, out var source)) {
            if (source != null) {
                _ = StartCoroutine(FadeAudioSource(source, (int)source.volume * 100, duration, fadeIn: true));
            }
        } else {
            var soundData = soundClips.Find(sound => sound.clip.name == soundName);
            if (soundData.clip != null) {
                AudioSource newSource = GetAvailableSource();

                if (newSource != null) {
                    newSource.clip = soundData.clip;
                    newSource.volume = 0;
                    newSource.transform.position = position;
                    newSource.Play();
                    playingSingleInstanceSounds[soundName] = newSource;
                    _ = StartCoroutine(FadeAudioSource(newSource, soundData.volume, duration, fadeIn: true));
                }
            }
        }
    }

    public void FadeOut(string soundName, float duration) {
        if (playingSingleInstanceSounds.TryGetValue(soundName, out var source)) {
            _ = StartCoroutine(FadeAudioSource(source, 0, duration, fadeIn: false));
        }
    }

    private IEnumerator FadeAudioSource(AudioSource source, int targetVolume, float duration, bool fadeIn) {
        float startVolume = source.volume;
        float timeElapsed = 0;

        while (timeElapsed < duration) {
            source.volume = Mathf.Lerp(startVolume, targetVolume / 100f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume / 100f;

        if (!fadeIn) {
            source.Stop();
        }
    }

    public delegate void SoundFinishedDelegate(string soundName);
    public event SoundFinishedDelegate OnSoundFinished;

    private IEnumerator RemoveSingleInstanceSoundWhenDone(AudioSource source, string soundName) {
        yield return new WaitWhile(() => source.isPlaying);

        OnSoundFinished?.Invoke(soundName);
        _ = playingSingleInstanceSounds.Remove(soundName);
    }

    private void PlayClip(AudioClip clip, int volume, Vector3 position, bool is3D, string soundName, bool isSingleInstance = false) {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        if (source != null) {
            source.clip = clip;
            source.volume = volume / 100f;
            source.transform.position = position;
            source.spatialBlend = is3D ? 1 : 0;

            if (isSingleInstance) {
                if (playingSingleInstanceSounds.ContainsKey(soundName) && playingSingleInstanceSounds[soundName].isPlaying) {
                    return;
                }

                source.Play();
                playingSingleInstanceSounds[soundName] = source;
                _ = StartCoroutine(RemoveSingleInstanceSoundWhenDone(source, soundName));
            } else {
                source.PlayOneShot(clip);
            }
        }
    }

    private AudioSource GetAvailableSource() {
        AudioSource availableSource = audioSourcePool.Find(source => !source.isPlaying)
                                      ?? AddAudioSourceToPool();
        return availableSource;
    }

    private AudioSource AddAudioSourceToPool() {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Add(newSource);
        return newSource;
    }
}
