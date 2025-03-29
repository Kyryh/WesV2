using HarmonyLib;
using UnityEngine;
using System.Collections;

[HarmonyPatch(typeof(V2), "Start")]
class Patch1 : DefaultPatch {
    static void Postfix(V2 __instance, Animator ___anim) {
        if (__instance.name == "Big Johnator")
            return;
        AudioSource audioSource;

        if (__instance.firstPhase) {
            audioSource = Utils.CreateVoiceObject("Voice", __instance.transform);
        } else {
            // Start of second phase in 4-4, no need to create a new Voice GameObject
            audioSource = __instance.transform.Find("Voice").GetComponent<AudioSource>();

            Utils.CreateOneTimeVoiceObject(
                "SlidingVoice",
                V2VoicelinesData.GetAudioClip("v2_2/fleeing2"),
                Utils.CreateSubtitleData(
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_fleeing2_1")),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_fleeing2_2"), 3f)
                ),
                __instance.transform
            ).Play();

        }


        if (!__instance.secondEncounter) {
            // FIRST ENCOUNTER (1-4)

            // Setting additional data so that i can keep track about V2's audioclips and other stuff
            __instance.SetAdditionalData(
                new ExtensionMethods.V2AdditionalData(
                    audioSource,
                    [
                        V2VoicelinesData.GetAudioClip("v2_1/taunt1"),
                        V2VoicelinesData.GetAudioClip("v2_1/taunt2"),
                        V2VoicelinesData.GetAudioClip("v2_1/taunt3"),
                        V2VoicelinesData.GetAudioClip("v2_1/taunt4"),
                        V2VoicelinesData.GetAudioClip("v2_1/taunt5"),
                        V2VoicelinesData.GetAudioClip("v2_1/taunt6"),
                    ],
                    [
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt1"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt2"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt3"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt4"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt5"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2_taunt6")
                    ],
                    null,
                    null,
                    V2VoicelinesData.GetAudioClip("v2_1/death"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_death")
                )
            );
        } else {
            // SECOND ENCOUNTER (4-4)

            __instance.SetAdditionalData(
                new ExtensionMethods.V2AdditionalData(
                    audioSource,
                    [
                        V2VoicelinesData.GetAudioClip("v2_2/taunt1"),
                        V2VoicelinesData.GetAudioClip("v2_2/taunt2"),
                        V2VoicelinesData.GetAudioClip("v2_2/taunt3"),
                        V2VoicelinesData.GetAudioClip("v2_2/taunt4"),
                    ],
                    [
                        V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt1"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt2"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt3"),
                        V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt4")
                    ],
                    V2VoicelinesData.GetAudioClip("v2_2/enrage"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2Second_enrage"),
                    V2VoicelinesData.GetAudioClip("v2_1/death"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_death")
                )
            );
            // If the player has died during the first phase of the second encounter
            // play a Taunt at the restart of the fight
            if (__instance.secondEncounter && __instance.firstPhase && !Plugin.secondEncounterIntro)
                __instance.Taunt();
        }

        // V2's 1-4 intro, when they come out from the window (fucking sick btw)
        if (__instance.intro) {
            __instance.PlayVoice(V2VoicelinesData.GetAudioClip("v2_1/windowbreak"));
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(V2VoicelinesData.GetSubtitle("subtitles_v2_windowsbreak"));
        }

        // V2's 1-4 long intro, when they bow (also fucking sick btw)
        if (__instance.longIntro) {
            __instance.StartCoroutine(LongIntro(__instance, ___anim));
        }


        Plugin.LogDebug("v2 started");
    }

    static IEnumerator LongIntro(V2 __instance, Animator ___anim) {
        // Wait until the Animator's Intro is set to true, so when anim.SetTrigger("Intro") is called inside the Update method
        yield return new WaitUntil(() => ___anim.GetBool("Intro"));

        AudioSource audioSource = Utils.CreateOneTimeVoiceObject(
            "IntroVoice",
            V2VoicelinesData.GetAudioClip("v2_1/intro"),
            Utils.CreateSubtitleData(
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_1"), 1.2f),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_2"), 3.2f),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_3"), 5.2f)
            ),
            __instance.transform
        );

        audioSource.Play();
    }
}
