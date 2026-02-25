using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using WesV2.Scripts;

namespace WesV2;
public static class ExtensionMethods {

    public static Type[] GetPatchesOfType(this Assembly assembly, Type type) {
        return assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();
    }

    // ConditionalWeakTable to store V2's additional data since i can't directly add attributes to classes with Harmony
    private static readonly ConditionalWeakTable<V2, V2Voice> additionalDataTable = new ConditionalWeakTable<V2, V2Voice>();

    public static V2Voice GetVoice(this V2 v2) {
        additionalDataTable.TryGetValue(v2, out V2Voice data);
        return data;

    }

    public static void AddVoice(this V2 v2) {
        var voice = v2.gameObject.AddComponent<V2Voice>();
        additionalDataTable.Add(v2, voice);
    }

}
