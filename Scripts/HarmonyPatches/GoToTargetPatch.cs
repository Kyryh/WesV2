using HarmonyLib;
using UnityEngine;



[HarmonyPatch(typeof(GoToTarget), "Activate")]
class Patch7 {
    static void Prefix(GoToTarget __instance) {
        if (__instance.activated)
            return;
        Transform transform = __instance.transform;
        if (transform.name == "Fake V2") {
            transform.Find("Voice").SetParent(null);
        }
    }
} 