using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class SwitchWeaponPatch : DefaultPatch {

    [HarmonyPatch(nameof(V2.Enrage), [typeof(string)])]
    [HarmonyPrefix]
    static void EnragePrefix(V2 __instance, string enrageName) {
        __instance.GetVoice()?.Enrage(enrageName);

        Plugin.LogDebug($"v2 enraged (enrageName=\"{enrageName}\")");
    }
}
