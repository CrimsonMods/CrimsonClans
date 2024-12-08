using System;
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

    public static void DestroyWithReason(this Entity entity)
    {
        Core.EntityManager.AddComponent<Disabled>(entity);
        DestroyUtility.CreateDestroyEvent(Core.EntityManager, entity, DestroyReason.Default, DestroyDebugReason.ByScript);
        DestroyUtility.Destroy(Core.EntityManager, entity);
    }
}