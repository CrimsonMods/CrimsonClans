using System;
using System.IO;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Shared;
using Unity.Entities;

namespace CrimsonClans;

public static class Extensions
{
    public unsafe static T Read<T>(this Entity entity) where T : struct
    {
        // Get the ComponentType for T
        var ct = new ComponentType(Il2CppType.Of<T>());

        // Get a pointer to the raw component data
        void* rawPointer = Core.EntityManager.GetComponentDataRawRO(entity, ct.TypeIndex);

        // Marshal the raw data to a T struct
        T componentData = Marshal.PtrToStructure<T>(new IntPtr(rawPointer));

        return componentData;
    }

    public static bool Has<T>(this Entity entity)
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        return Core.EntityManager.HasComponent(entity, ct);
    }

    public static void DestroyWithReason(this Entity entity)
    {
        Core.EntityManager.AddComponent<Disabled>(entity);
        DestroyUtility.CreateDestroyEvent(Core.EntityManager, entity, DestroyReason.Default, DestroyDebugReason.ByScript);
        DestroyUtility.Destroy(Core.EntityManager, entity);
    }

    public static void Dump(this Entity entity, string filePath)
    {
        File.AppendAllText(filePath, $"--------------------------------------------------" + Environment.NewLine);
        File.AppendAllText(filePath, $"Dumping components of {entity.ToString()}:" + Environment.NewLine);
        foreach (var componentType in Core.Server.EntityManager.GetComponentTypes(entity))
        { File.AppendAllText(filePath, $"{componentType.ToString()}" + Environment.NewLine); }
        File.AppendAllText(filePath, $"--------------------------------------------------" + Environment.NewLine);
        File.AppendAllText(filePath, DumpEntity(entity));
    }
    private static string DumpEntity(Entity entity, bool fullDump = true)
    {
        var sb = new Il2CppSystem.Text.StringBuilder();
        EntityDebuggingUtility.DumpEntity(Core.Server, entity, fullDump, sb);
        return sb.ToString();
    }
}