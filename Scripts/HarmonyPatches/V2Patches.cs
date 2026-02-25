using HarmonyLib;
using WesV2.Patches;

namespace WesV2.Scripts.HarmonyPatches {
    [HarmonyPatch(typeof(V2))]
    class V2Patches : DefaultPatch {

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

        [HarmonyPatch(nameof(V2.SwitchWeapon))]
        [HarmonyPostfix]
        static void SwitchWeaponPostfix(V2 __instance) {
            if (!__instance.firstPhase)
                return;

            __instance.GetVoice()?.SwitchWeapons();

            Plugin.LogDebug("v2 changed weapon");
        }

        [HarmonyPatch(nameof(V2.Enrage), [typeof(string)])]
        [HarmonyPrefix]
        static void EnragePrefix(V2 __instance, string enrageName) {
            __instance.GetVoice()?.Enrage(enrageName);

            Plugin.LogDebug($"v2 enraged (enrageName=\"{enrageName}\")");
        }

        [HarmonyPatch(nameof(V2.KnockedOut))]
        [HarmonyPostfix]
        static void KnockedOutPostfix(V2 __instance, string triggerName) {
            __instance.GetVoice()?.KnockedOut(triggerName);
            Plugin.LogDebug("v2 knocked out: " + triggerName);
        }


        [HarmonyPatch(nameof(V2.Die))]
        [HarmonyPrefix]
        static void DiePrefix(V2 __instance, bool ___dead, bool ___bossVersion) {
            __instance.GetVoice()?.Die();

            Plugin.LogDebug("v2 died");
        }
    }
}
