using System;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pose
{
    class TimeWarping
    {
        public class Data
        {
            public int ContinuesI = 0;
            public int ContinuesJ = 0;
            public int PreviousI = -1;
            public int PreviousJ = -1;
            public double SumDistance = -1;
            public float Length = 0;
            public float Distance = -1;
            public float Theta = 0;
            public float X0 = 0;
            public float Z0 = 0;
        }
        Data[,] twTable;
        Pose.Object basicObj; // 時間以這個為基礎
        Pose.Object refObj; // 對應的
        Pose.Object tmpBasicObj, tmpRefObj;
        public float[] Warping;
        // public List<float[]> Alinement;
        public TimeWarping(Pose.Object basicObj, Pose.Object refObj)
        {
            Assert.IsNotNull(basicObj);
            Assert.IsNotNull(refObj);
            this.basicObj = basicObj;
            this.refObj = refObj;
        }
        public void Do() {
            tmpBasicObj = basicObj.Clone();
            tmpBasicObj.name += "_tmp";
            tmpBasicObj.gameObject.SetActive(false);
            tmpRefObj = refObj.Clone();
            tmpRefObj.name += "_tmp";
            tmpRefObj.gameObject.SetActive(false);

            var endFrame = createTimeWarpingTable();
            doTimeWarping(endFrame.Item1, endFrame.Item2);
            refToBasic();
            
            tmpBasicObj.gameObject.SetActive(false);
            GameObject.Destroy(tmpBasicObj.gameObject);
            //GameObject.Destroy(tmpRefObj.gameObject);
        }
        private static bool isLess(double num1, double num2) {
            // -1 代表無限大
            if (num1 <= 0) return false;
            if (num2 <= 0) return true;
            return num1 < num2;
        }
        private Tuple<int, int> createTimeWarpingTable()
        {
            int o1Count = basicObj.Motion.FrameCount;
            int o2Count = refObj.Motion.FrameCount;
            twTable = new Data[o1Count, o2Count];
            int maxLenI = -1, maxLenJ = -1;
            for (int i = 0; i < o1Count; i++)
            {
                int jStart = Utility.Clip(i - 50, 0, o2Count - 1);
                int jEnd = Utility.Clip(i + 50, 0, o2Count - 1);
                for (int j = jStart; j <= jEnd; j++)
                {
                    var twData = Distance(tmpBasicObj, tmpRefObj, i, j);
                    if (!isLess(twData.Distance, 5)) continue;
                    twTable[i, j] = twData;
                    Data left = j > 0 ? twTable[i, j - 1] : null;
                    Data up = i > 0 ? twTable[i - 1, j] : null;
                    Data leftup = i > 0 && j > 0 ? twTable[i - 1, j - 1] : null;
                    double leftDis = left != null ? (left.SumDistance / left.Length) : -1;
                    double upDis = up != null ? (up.SumDistance / up.Length) : -1;
                    double leftupDis = leftup != null ? (leftup.SumDistance / leftup.Length) : -1;

                    // 限制斜率
                    if (left != null && left.ContinuesJ >= 3) leftDis = -1;
                    if (up != null && up.ContinuesJ >= 3) upDis = -1;

                    if (isLess(leftupDis, upDis) && isLess(leftupDis, leftDis))
                    {
                        twData.PreviousI = i - 1;
                        twData.PreviousJ = j - 1;
                        twData.ContinuesI = 0;
                        twData.ContinuesJ = 0;
                        twData.SumDistance = leftupDis + twData.Distance;
                        twData.Length = leftup.Length + 1;
                    }
                    else if (isLess(leftDis, upDis))
                    {
                        twData.PreviousI = i;
                        twData.PreviousJ = j - 1;
                        twData.ContinuesI = 0;
                        twData.ContinuesJ = left.ContinuesJ + 1;
                        twData.SumDistance = leftDis + twData.Distance;
                        twData.Length = left.Length + 1;
                    }
                    else if (isLess(upDis, -1))
                    {
                        twData.PreviousI = i - 1;
                        twData.PreviousJ = j;
                        twData.ContinuesI = up.ContinuesI + 1;
                        twData.ContinuesJ = 0;
                        twData.SumDistance = upDis + twData.Distance;
                        twData.Length = up.Length + 1;
                    }
                    // SumDistance 
                    if (twData.Length <= 0)
                    {
                        twData.ContinuesI = 0;
                        twData.ContinuesJ = 0;
                        twData.Length = 1;
                        twData.SumDistance = twData.Distance;
                    }
                    
                    if (maxLenI <= 0) {
                        maxLenI = i;
                        maxLenJ = j;
                    }
                    else {
                        var maxLenTwData = twTable[maxLenI, maxLenJ];
                        var avgDis = twData.SumDistance / twData.Length;
                        var maxLenAvgDis = maxLenTwData.SumDistance / maxLenTwData.Length;
                        if (twData.Length > maxLenTwData.Length ||
                            (twData.Length == maxLenTwData.Length && avgDis > maxLenAvgDis))
                        {
                            maxLenI = i;
                            maxLenJ = j;
                        }
                    }
                }
            }
            return new Tuple<int, int>(maxLenI, maxLenJ);
        }
        public static Data Distance(Pose.Object o1, Pose.Object o2, int o1Idx, int o2Idx)
        {
            int partLen = o1.Part.Length;
            float tan11 = 0, tan12 = 0, tan21 = 0, tan22 = 0;
            float x1 = 0, x2 = 0, z1 = 0, z2 = 0;
            float w = (float)1.0 / partLen / 5;
            Vector3[] vi1 = new Vector3[partLen], vi2 = new Vector3[partLen];
            for (int i = 0; i < 5; i++)
            {
                int o1IdxPlusI = Utility.Clip(o1Idx + i - 2, 0, o1.Motion.FrameCount - 1);
                int o2IdxPlusI = Utility.Clip(o2Idx + i - 2, 0, o2.Motion.FrameCount - 1);
                o1.ApplyFrameByIdx(o1IdxPlusI);
                o2.ApplyFrameByIdx(o2IdxPlusI);
                for (int j = 0; j < partLen; j++)
                {
                    Vector3 v1 = o1.Part[i].transform.position;
                    Vector3 v2 = o2.Part[i].transform.position;
                    if (i == 2)
                    {
                        vi1[j] = v1;
                        vi2[j] = v2;
                    }
                    tan11 += w * (v1.x * v2.z - v2.x * v1.z);
                    tan21 += w * (v1.x * v2.x + v1.z * v2.z);
                    x1 += w * v1.x;
                    x2 += w * v2.x;
                    z1 += w * v1.z;
                    z2 += w * v2.z;
                }
            }
            tan12 = x1 * z2 - x2 * z1;
            tan22 = x1 * x2 + z1 * z2;
            Data data = new Data();
            data.Theta = Mathf.Atan((tan11 - tan12) / (tan21 - tan22));
            // data.Theta = Utility.ConvertVecToAngle(new Vector2(tan21 - tan22, tan11 - tan12)) * Mathf.Deg2Rad;
            data.X0 = x1 - x2 * Mathf.Cos(data.Theta) - z2 * Mathf.Sin(data.Theta);
            data.Z0 = z1 + x2 * Mathf.Sin(data.Theta) - z2 * Mathf.Cos(data.Theta);
            for (int i = 0; i < partLen; i++)
            {
                Vector3 v2 = Quaternion.Euler(0, data.Theta, 0) * (vi2[i] + new Vector3(data.X0, 0, data.Z0));
                data.Distance += Vector3.Distance(vi1[i], v2);
            }
            return data;
        }
        private void doTimeWarping(int endIFrame, int endJFrame)
        {
            List<Tuple<int, int>> tmpWarping = new List<Tuple<int, int>>();
            int i = endIFrame, j = endJFrame;
            while (i >= 0 && j >= 0)
            {
                tmpWarping.Add(new Tuple<int, int>(i, j));
                var tmp_timewarp = twTable[i, j];
                i = tmp_timewarp.PreviousI;
                j = tmp_timewarp.PreviousJ;
            }
            tmpWarping.Reverse();
            Warping = new float[basicObj.Motion.FrameCount];
            int lastHasValIdx = -1;
            for (i = 0; i < basicObj.Motion.FrameCount; i++)
            {
                var idics = tmpWarping.Where(x => x.Item1 == i).Select(x => x.Item2).ToArray();
                if (idics.Length > 0) {
                    Warping[i] = (float) idics.Average();
                    if (lastHasValIdx >= 0) {
                        for (j = lastHasValIdx + 1; j < i; j++) {
                            Warping[j] = Warping[lastHasValIdx] + (Warping[i] - Warping[lastHasValIdx]) / (i - lastHasValIdx);
                        }
                    }
                    lastHasValIdx = i;
                }
                else {
                    Warping[i] = -1;
                }
            }
            // for (i = 0; i < basicObj.Motion.FrameCount; i++) Debug.Log(i + ": " + Warping[i]);
        }
        private void refToBasic() {
            refObj.Motion.MotionData.Clear();
            for (int i = 0; i < basicObj.Motion.FrameCount; i++) {
                float lastFrameIdx = 0;
                if (Warping[i] >= 0 && Warping[i] <= tmpRefObj.Motion.FrameCount) {
                    refObj.Motion.MotionData.Add(tmpRefObj.Motion.getFrame(Warping[i]));
                    lastFrameIdx = Warping[i];
                }
                else {
                    refObj.Motion.MotionData.Add(tmpRefObj.Motion.getFrame(lastFrameIdx));
                }
            }
            refObj.Motion.FrameCount = refObj.Motion.MotionData.Count;
            refObj.Motion.FrameTime = basicObj.Motion.FrameTime;
        }
    }
}