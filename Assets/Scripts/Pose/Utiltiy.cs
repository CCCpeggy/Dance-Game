using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pose
{
    public class Utility
    {
       
        public static int GetPartIdxByCMUName(string name){
            return ((CMUPartIdx)System.Enum.Parse(typeof(CMUPartIdx), name)).Int();
        }
        public static int Clip(int value, int min, int max)
        {
            if (value <= min) return min;
            if (value >= max) return max;
            return value;
        }
        public static float Clip(float value, float min, float max)
        {
            if (value <= min) return min;
            if (value >= max) return max;
            return value;
        }
        public static IEnumerable<string> SplitString(string data)
        {
            var components = data.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in components)
            {
                yield return c;
            }
        }

        public static Vector2 ConvertAngleToVec(float a)
        {
            return new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad));
        }
        public static Vector3 ConvertAngleToScaleVec(float theta, float x, float z)
        {
            Vector3 newVector = new Vector3();
            newVector.x = x * Mathf.Cos(theta) + z * Mathf.Sin(theta);
            newVector.z = x * Mathf.Sin(theta) + z * Mathf.Cos(theta);
            return newVector;
        }
        public static float ConvertVecToAngle(Vector2 v)
        {
            float angle = Vector2.Angle(v, new Vector2(1, 0));
            if (v.y < 0) angle = -angle;
            return angle;
        }

        public static float GetAngleAvg(float a1, float a2, float alpha)
        {
            Vector2 v1 = new Vector2(Mathf.Cos(a1 * Mathf.Deg2Rad), Mathf.Sin(a1 * Mathf.Deg2Rad));
            Vector2 v2 = new Vector2(Mathf.Cos(a2 * Mathf.Deg2Rad), Mathf.Sin(a2 * Mathf.Deg2Rad));
            Vector2 v3 = v1 * (1 - alpha) + v2 * alpha;
            float angle = Vector2.Angle(v3, new Vector2(1, 0));
            if (v3.y < 0) angle = -angle;
            return angle;
        }

        public static Quaternion GetQuaternionAvg(Quaternion a1, Quaternion a2, float alpha)
        {
            return Quaternion.Lerp(a1, a2, alpha);
        }

        public static Vector3 GetVectorAvg(Vector3 a1, Vector3 a2, float alpha)
        {
            return a1 * (1 - alpha) + a2 * alpha;
        }

        public class IterData
        {

            public static string GetAndNext(ref IEnumerator<string> iter)
            {
                string tmp = iter.Current;
                iter.MoveNext();
                return tmp;
            }

            public static void CheckAndNext(ref IEnumerator<string> iter, string str)
            {
                if (iter.Current != str)
                {
                    Debug.LogError("預期是 " + str + "，但解析到的是 " + iter.Current);
                }
                // Debug.Assert(iter.Current == str);
                iter.MoveNext();
            }
            public static bool CompareAndNext(ref IEnumerator<string> iter, string str)
            {
                return GetAndNext(ref iter) == str;
            }

            public static Vector3 GetVec3AndNext(ref IEnumerator<string> iter)
            {
                float x = float.Parse(Utility.IterData.GetAndNext(ref iter));
                float y = float.Parse(Utility.IterData.GetAndNext(ref iter));
                float z = float.Parse(Utility.IterData.GetAndNext(ref iter));
                return new Vector3(x, y, z);
            }
        }
    }
}