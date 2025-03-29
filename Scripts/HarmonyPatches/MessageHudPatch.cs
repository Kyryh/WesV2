using HarmonyLib;

[HarmonyPatch(typeof(HudMessage), "PlayMessage")]
class Patch6 : DefaultPatch {
    static void Prefix(ref string ___message) {
        // Change V1's text message for when V2 flees in 4-4
        if (___message == "You're not getting away this time.")
            ___message = "You get back here right this FUCKING INSTANT.";
    }
}
