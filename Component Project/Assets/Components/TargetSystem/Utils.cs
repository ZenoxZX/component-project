using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZenoxZX.TargetSystem
{
    public static class Utils
    {
        public static Vector3 ToX0Z(this Vector3 vector3) => new Vector3(vector3.x, 0, vector3.z);
    }
}