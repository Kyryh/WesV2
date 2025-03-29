using UnityEngine;

static class Utils {

    // Method to find a GameObject given its name
    // because GameObject.Find() doesn't return inactive objects and
    // Transform.Find() needs a Transform instance (so i can't search globally in the Scene)
    public static GameObject FindGameObjectByName(string name) {
        GameObject[] gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject gameObject in gameObjects) {
            if (gameObject.name == name) {
                return gameObject;
            }
        }

        Plugin.LogError("huh??? " + name);
        return null;
    }

    // Methods to make creating subtitles easier, blatantly copied from armedturret's PrimePresidents
    public static SubtitledAudioSource.SubtitleDataLine MakeLine(string subtitle, float time = 0) {
        return new SubtitledAudioSource.SubtitleDataLine {
            subtitle = subtitle,
            time = time
        };
    }

    public static SubtitledAudioSource.SubtitleData CreateSubtitleData(params SubtitledAudioSource.SubtitleDataLine[] array) {
        return new SubtitledAudioSource.SubtitleData {
            lines = array
        };
    }

    // Method to Instantiate a new GameObject with an AudioSource, play it and
    // destroy it afterwards to not create clutter
    // Basically what the static method AudioSource.PlayClipAtPoint() does,
    // but i couldn't get it to work for some reason so i've just rewritten it here
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position) {

        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.95f, 1f);
        audioSource.clip = clip;
        audioSource.Play();
        // +1 because idk better safe than sorry ig
        GameObject.Destroy(gameObject, clip.length + 1);
    }

    public static AudioSource CreateVoiceObject(string name, Transform parent = null) {
        GameObject voice = new GameObject(name);
        voice.transform.SetParent(parent);
        voice.transform.localPosition = Vector3.zero;

        AudioSource audioSource = voice.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 3D audio stuff
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0;

        voice.AddComponent<SoundPause>();

        return audioSource;
    }

    public static AudioSource CreateOneTimeVoiceObject(string name, AudioClip clip, SubtitledAudioSource.SubtitleData subtitles, Transform parent = null) {
        GameObject voice = new GameObject(name);
        voice.transform.SetParent(parent);
        voice.transform.localPosition = Vector3.zero;

        AudioSource audioSource = voice.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = true;

        // 3D audio stuff
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0;

        voice.AddComponent<SoundPause>();

        SubtitledAudioSource subtitledAudioSource = voice.AddComponent<SubtitledAudioSource>();
        subtitledAudioSource.subtitles = subtitles;

        return audioSource;
    }
}
