using HarmonyLib;


namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class V2Patches : DefaultPatch {

    [HarmonyPatch(nameof(V2.Die))]
    [HarmonyPrefix]
    static void DiePrefix(V2 __instance, bool ___dead, bool ___bossVersion) {
        __instance.GetVoice()?.Die();

        Plugin.LogDebug("v2 died");
    }
}
