using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class SwitchWeaponPatch : DefaultPatch {

    [HarmonyPatch(nameof(V2.Enrage), [typeof(string)])]
    [HarmonyPostfix]
    static void EnragePostfix(V2 __instance, string enrageName) {
        if (enrageName == "STOP HITTING YOURSELF") {
            ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();
            data.timesChangedWeapon = 0;
            __instance.PlayVoice(data.enrageAudioClip);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.enrageSubtitle);
        }

        Plugin.LogDebug($"v2 enraged (enrageName=\"{enrageName}\")");
    }
}
