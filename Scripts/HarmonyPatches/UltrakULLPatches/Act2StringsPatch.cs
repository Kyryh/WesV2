using HarmonyLib;
using UltrakULL;


namespace WesV2.Patches;

[HarmonyPatch(typeof(Act2Strings), "Level44")]
class Patch9 : UltrakULLPatch {
    static void Prefix(ref string message) {
        if (message == "You get back here right this FUCKING INSTANT.")
            message = "You're not getting away this time.";
    }
}
