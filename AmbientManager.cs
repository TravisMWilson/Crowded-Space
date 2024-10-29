using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbientManager : MonoBehaviour {
    public static AmbientManager Instance;

    [SerializeField] private List<SoundData> ambientClips = new List<SoundData>();
    [SerializeField] private List<SoundData> musicListWarpScene = new List<SoundData>();
    [SerializeField] private List<SoundData> musicListMainHub = new List<SoundData>();
    [SerializeField] private List<SoundData> musicListShipCreation = new List<SoundData>();

    private Dictionary<int, AudioSource> audioChannels = new Dictionary<int, AudioSource>();

    private Coroutine randomMusicCoroutine;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        switch (scene.name) {
            case "WarpScene":
                PlayRandomMusic(musicListWarpScene);
                break;
            case "MainHub":
                PlayRandomMusic(musicListMainHub);
                break;
            case "ShipCreation":
                PlayRandomMusic(musicListShipCreation);
                break;
        }
    }

    private SoundData GetSoundData(string soundName, List<SoundData> soundList) => soundList.Find(sound => sound.clip.name == soundName);

    public void PlayRandomMusic(List<SoundData> musicList, int inputChannel = 1) {
        if (musicList == null || musicList.Count == 0) {
            Debug.LogError("No music names provided for random selection.");
            return;
        }

        string randomMusicName = musicList[Random.Range(0, musicList.Count)].clip.name;

        var soundData = GetSoundData(randomMusicName, musicList);

        if (soundData == null) {
            Debug.LogError($"Music clip '{randomMusicName}' not found.");
            return;
        }

        if (!audioChannels.ContainsKey(inputChannel)) {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.loop = false;
            newAudioSource.volume = soundData.volume / 100f;
            newAudioSource.spatialBlend = soundData.is3D ? 1f : 0f;
            audioChannels.Add(inputChannel, newAudioSource);
        }

        AudioSource channelAudioSource = audioChannels[inputChannel];

        if (channelAudioSource != null && channelAudioSource.clip != soundData.clip) {
            channelAudioSource.clip = soundData.clip;
            channelAudioSource.volume = soundData.volume / 100f;
            channelAudioSource.spatialBlend = soundData.is3D ? 1f : 0f;
            channelAudioSource.Play();

            if (randomMusicCoroutine != null) StopCoroutine(randomMusicCoroutine);
            randomMusicCoroutine = StartCoroutine(PlayNextRandomMusic(musicList, inputChannel));
        }
    }

    private IEnumerator PlayNextRandomMusic(List<SoundData> musicList, int inputChannel) {
        AudioSource channelAudioSource = audioChannels[inputChannel];

        yield return new WaitWhile(() => channelAudioSource.isPlaying);

        if (Instance == null) yield break;

        PlayRandomMusic(musicList, inputChannel);
    }

    public void PlayAmbient(string ambient, int inputChannel = 2) {
        var soundData = GetSoundData(ambient, ambientClips);

        if (soundData == null) {
            Debug.LogError($"Ambient clip '{ambient}' not found.");
            return;
        }

        if (!audioChannels.ContainsKey(inputChannel)) {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.loop = true;
            newAudioSource.volume = soundData.volume / 100f;
            newAudioSource.spatialBlend = soundData.is3D ? 1f : 0f;
            audioChannels.Add(inputChannel, newAudioSource);
        }

        AudioSource channelAudioSource = audioChannels[inputChannel];

        if (channelAudioSource.clip != soundData.clip) {
            channelAudioSource.clip = soundData.clip;
            channelAudioSource.volume = soundData.volume / 100f;
            channelAudioSource.spatialBlend = soundData.is3D ? 1f : 0f;
            channelAudioSource.Play();
        }
    }

    public void StopChannel(int inputChannel = 0) {
        if (inputChannel == 0) {
            foreach (var source in audioChannels.Values) {
                source.Stop();
            }
        } else if (audioChannels.ContainsKey(inputChannel)) {
            audioChannels[inputChannel].Stop();
        }
    }

    public void PauseChannel(int inputChannel = 0) {
        if (inputChannel == 0) {
            foreach (var source in audioChannels.Values) {
                source.Pause();
            }
        } else if (audioChannels.ContainsKey(inputChannel)) {
            audioChannels[inputChannel].Pause();
        }
    }

    public void ResumeChannel(int inputChannel = 0) {
        if (inputChannel == 0) {
            foreach (var source in audioChannels.Values) {
                source.UnPause();
            }
        } else if (audioChannels.ContainsKey(inputChannel)) {
            audioChannels[inputChannel].UnPause();
        }
    }

    public void FadeInClip(string clipName, int inputChannel = 1, float duration = 1.0f) {
        var soundData = GetSoundData(clipName, ambientClips);

        if (soundData == null) {
            Debug.LogError($"Clip '{clipName}' not found.");
            return;
        }

        if (!audioChannels.ContainsKey(inputChannel)) {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.loop = true;
            newAudioSource.clip = soundData.clip;
            newAudioSource.volume = 0f;
            newAudioSource.spatialBlend = soundData.is3D ? 1f : 0f;
            audioChannels.Add(inputChannel, newAudioSource);
        }

        _ = StartCoroutine(FadeIn(audioChannels[inputChannel], soundData.volume / 100f, duration));
    }

    public void FadeOutChannel(int inputChannel = 1, float duration = 1.0f) {
        if (audioChannels.ContainsKey(inputChannel)) {
            _ = StartCoroutine(FadeOut(audioChannels[inputChannel], duration));
        }
    }

    public void SetChannelVolume(int inputChannel, float volume) {
        if (audioChannels.ContainsKey(inputChannel)) {
            audioChannels[inputChannel].volume = Mathf.Clamp01(volume);
        }
    }

    public void SetAllChannelsVolume(float volume) {
        foreach (var source in audioChannels.Values) {
            source.volume = Mathf.Clamp01(volume);
        }
    }

    public bool IsClipPlaying(string ambient, int inputChannel) {
        var soundData = GetSoundData(ambient, ambientClips);

        return audioChannels.ContainsKey(inputChannel) && soundData != null
            && audioChannels[inputChannel].clip == soundData.clip && audioChannels[inputChannel].isPlaying;
    }

    public void MuteAll() {
        foreach (var source in audioChannels.Values) {
            source.mute = true;
        }
    }

    public void UnmuteAll() {
        foreach (var source in audioChannels.Values) {
            source.mute = false;
        }
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration) {
        float currentTime = 0f;
        float startVolume = source.volume;

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        source.volume = targetVolume;
        source.Play();
    }

    private IEnumerator FadeOut(AudioSource source, float duration) {
        float currentTime = 0f;
        float startVolume = source.volume;

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
