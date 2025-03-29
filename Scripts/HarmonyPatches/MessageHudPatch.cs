using HarmonyLib;

namespace WesV2.Patches;

[HarmonyPatch(typeof(HudMessage))]
class HudMessagePatches : DefaultPatch {
    [HarmonyPatch(nameof(HudMessage.PlayMessage))]
    [HarmonyPrefix]
    static void PlayMessagePrefix(ref string ___message) {
        // Change V1's text message for when V2 flees in 4-4
        if (___message == "You're not getting away this time.")
            ___message = "You get back here right this FUCKING INSTANT.";
    }
}
