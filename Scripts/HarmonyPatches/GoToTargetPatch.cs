using HarmonyLib;
using UnityEngine;


namespace WesV2.Patches;

[HarmonyPatch(typeof(GoToTarget))]
class GoToTargetPatch : DefaultPatch {
    [HarmonyPatch(nameof(GoToTarget.Activate))]
    [HarmonyPrefix]
    static void ActivatePrefix(GoToTarget __instance) {
        if (__instance.activated)
            return;
        Transform transform = __instance.transform;
        if (transform.name == "Fake V2") {
            transform.Find("Voice").SetParent(null);
        }
    }
}
