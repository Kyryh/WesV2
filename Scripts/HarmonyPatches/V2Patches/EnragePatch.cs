using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
[HarmonyPatch(nameof(V2.Enrage), [typeof(string)])]
class Patch5 : DefaultPatch {
    static void Postfix(V2 __instance, string enrageName) {
        if (enrageName == "STOP HITTING YOURSELF") {
            ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();
            data.timesChangedWeapon = 0;
            __instance.PlayVoice(data.enrageAudioClip);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.enrageSubtitle);
        }

        Plugin.LogDebug($"v2 enraged (enrageName=\"{enrageName}\")");
    }
}
