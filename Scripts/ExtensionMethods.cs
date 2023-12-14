using System.Runtime.CompilerServices;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using System;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{

    public static Type[] GetPatchesOfType(this Assembly assembly, Type type) {
        return assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();
    }

    // ConditionalWeakTable to store V2's additional data since i can't directly add attributes to classes with Harmony
    private static readonly ConditionalWeakTable<V2, V2AdditionalData> additionalDataTable = new ConditionalWeakTable<V2, V2AdditionalData>();

    public static V2AdditionalData GetAdditionalData(this V2 v2) {
        additionalDataTable.TryGetValue(v2, out V2AdditionalData data);
        return data;

    }   

    public static void SetAdditionalData(this V2 v2, V2AdditionalData data) {
        additionalDataTable.Add(v2, data);
    }

    // Constructor for the additional data
    public class V2AdditionalData(
        AudioSource audioSource,
        AudioClip[] tauntAudioClips, string[] tauntSubtitles,
        AudioClip enrageAudioClip, string enrageSubtitle,
        AudioClip deathAudioClip, string deathSubtitle
    ) {
        public AudioSource audioSource = audioSource;

        public AudioClip[] tauntAudioClips = tauntAudioClips;
        public string[] tauntSubtitles = tauntSubtitles;

        public int[] tauntWeights = Enumerable.Repeat(1, tauntAudioClips.Length).ToArray();
        public int TauntWeightsSum => tauntWeights.Sum();

        public AudioClip enrageAudioClip = enrageAudioClip;
        public string enrageSubtitle = enrageSubtitle;

        public AudioClip deathAudioClip = deathAudioClip;
        public string deathSubtitle = deathSubtitle;
        
        public int timesChangedWeapon = 0;

        // Gets a random taunt based on each taunt's weight
        // Weights are set to 1 to every taunt initially and
        // a taunt's weight is set to 0 whenever it's used
        // (so that no taunt can get used twice in a row)
        // and the rest of the weights are increased by 1
        // so that a taunt that hasn't been used for a long time
        // has a higher chance to get used next
        public int WeightedRandomIndex() {
            int currentSum = 0;
            int index = -1;
            int randomNum = Random.Range(0, TauntWeightsSum);

            while (randomNum >= currentSum) {
                index++;
                currentSum += tauntWeights[index];
            }

            return index;
        }
    }

    // Play a voice clip
    public static void PlayVoice(this V2 v2, AudioClip clip) {
        AudioSource audioSource = v2.GetAdditionalData().audioSource;
        audioSource.pitch = Random.Range(0.95f, 1f);
        audioSource.clip = clip;
		audioSource.Play();
    }

    // Plays a random taunt
    public static void Taunt(this V2 v2) {

        V2AdditionalData data = v2.GetAdditionalData();
        int randomTaunt = data.WeightedRandomIndex();

        // Increases each weight by 1
        for (int i = 0; i < data.tauntWeights.Length; i++) {
            data.tauntWeights[i]++;
        }
        // Sets the chosen taunt's weight to 0
        data.tauntWeights[randomTaunt] = 0;

        v2.PlayVoice(data.tauntAudioClips[randomTaunt]);
        MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.tauntSubtitles[randomTaunt]);
    }

    // Set the subtitles attribute in a SubtitledAudioSource via reflection
    // because it doesn't have a method to do it 
    // (i guess there isn't one since you serialize it in the unity editor)
    public static void SetSubtitles(this SubtitledAudioSource subtitledAudioSource, SubtitledAudioSource.SubtitleData subtitles) {
        Traverse.Create(subtitledAudioSource).Field("subtitles").SetValue(
            subtitles
        );
    }
}

