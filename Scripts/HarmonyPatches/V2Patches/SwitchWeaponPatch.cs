using HarmonyLib;



[HarmonyPatch(typeof(V2), "SwitchWeapon")]
class Patch4 {
    static void Postfix(V2 __instance) {
        if (!__instance.firstPhase)
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