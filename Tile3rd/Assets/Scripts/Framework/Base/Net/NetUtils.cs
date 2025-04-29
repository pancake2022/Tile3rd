using System;
using System.Runtime.InteropServices;

namespace CSFramework
{
    public class NetUtils
    {
        public static byte[] GetBytes<T> (T obj) where T : struct
        {
            var size = Marshal.SizeOf(obj);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T ParseBytes<T> (byte[] arr) where T : struct
        {
            var obj_type = typeof(T);
            var obj = obj_type.GetConstructors()[0].Invoke(new object[]{});
            var size = Marshal.SizeOf(obj);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            obj = Marshal.PtrToStructure(ptr, obj_type);
            Marshal.FreeHGlobal(ptr);
            return (T)obj;
        }

        public static object ParseBytes (byte[] arr, Type obj_type)
        {
            var obj = obj_type.GetConstructors()[0].Invoke(new object[]{});
            var size = Marshal.SizeOf(obj);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            obj = Marshal.PtrToStructure(ptr, obj_type);
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
    }
}