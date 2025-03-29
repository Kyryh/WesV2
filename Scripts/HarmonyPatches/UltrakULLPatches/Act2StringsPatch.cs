using HarmonyLib;
using UltrakULL;


namespace WesV2.Patches;

[HarmonyPatch(typeof(Act2Strings))]
class Act2StringsPatch : UltrakULLPatch {
    [HarmonyPatch("Level44")]
    [HarmonyPrefix]
    static void Level44Prefix(ref string message) {
        if (message == "You get back here right this FUCKING INSTANT.")
            message = "You're not getting away this time.";
    }
}
