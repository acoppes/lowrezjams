using UnityEngine;

namespace Abduction101.Utilities
{
    public static class Vector3Extensions
    {
        public static Vector3 ToY(this Vector3 v)
        {
            return new Vector3(0, v.y, 0);
        }
        
        public static Vector3 NoY(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }
    }
}