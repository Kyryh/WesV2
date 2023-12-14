using HarmonyLib;



[HarmonyPatch(typeof(V2), "Enrage")]
class Patch5 : DefaultPatch {
    static void Postfix(V2 __instance) {
        if (__instance.secondEncounter) {
            ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();
            data.timesChangedWeapon = 0;
            __instance.PlayVoice(data.enrageAudioClip);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.enrageSubtitle);
        }

        Plugin.LogDebug("v2 enraged");
    }
} 