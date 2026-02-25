using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class V2Patches : DefaultPatch {

    [HarmonyPatch(nameof(V2.SwitchWeapon))]
    [HarmonyPostfix]
    static void SwitchWeaponPostfix(V2 __instance) {
        if (!__instance.firstPhase)
            return;

        __instance.GetVoice()?.SwitchWeapons();

        Plugin.LogDebug("v2 changed weapon");
    }
}
