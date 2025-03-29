using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class V2Patches : DefaultPatch {

    [HarmonyPatch(nameof(V2.SwitchWeapon))]
    [HarmonyPostfix]
    static void SwitchWeaponPostfix(V2 __instance) {
        if (!__instance.firstPhase)
            return;
        if (__instance.name == "Big Johnator")
            return;
        ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();

        data.timesChangedWeapon++;

        // Play the taunt after that V2 changes weapon 7 times
        if (data.timesChangedWeapon >= 7) {
            data.timesChangedWeapon = 0;
            __instance.Taunt();
        }

        Plugin.LogDebug("v2 changed weapon");
    }
}
