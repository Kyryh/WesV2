using HarmonyLib;
using UltrakULL;

[HarmonyPatch(typeof(SubtitleStrings), "GetSubtitle")]
class Patch9 : UltrakULLPatch {
    static bool Prefix(string input, ref string __result) {
        string currentSceneName = SceneHelper.CurrentScene;
        if (currentSceneName.Contains("1-4") || currentSceneName.Contains("4-4")) {
            __result = input;
            return false;
        }
        return true;
    }
}
