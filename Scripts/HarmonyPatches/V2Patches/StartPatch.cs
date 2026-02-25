using HarmonyLib;
using UnityEngine;
using System.Collections;
using WesV2.Scripts;

namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class V2Patches : DefaultPatch {

    [HarmonyPatch(nameof(V2.Start))]
    [HarmonyPostfix]
    static void StartPostfix(V2 __instance) {
        if (__instance.transform.Find("BigJohn") != null)
            return;

        if (__instance.firstPhase) {
            Utils.CreateVoiceObject("Voice", __instance.transform);
        } else {
            // Start of second phase in 4-4, no need to create a new Voice GameObject
            //audioSource = __instance.transform.Find("Voice").GetComponent<AudioSource>();

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

        __instance.AddVoice();

        Plugin.LogDebug("v2 started");
    }

}
