using UnityEngine;

namespace Pose
{
    public enum CMUPartIdx : int
    {
        Hip,
        lButtock,
        Left_Thigh,
        Left_Shin,
        Left_Foot,
        lToe,
        rButtock,
        Right_Thigh,
        Right_Shin,
        Right_Foot,
        rToe,
        Waist,
        Abdomen,
        Chest,
        Neck,
        Neck1,
        Head,
        Left_Collar,
        Left_Shoulder,
        Left_Forearm,
        Left_Hand,
        LeftFingerBase,
        LFingers,
        lThumb1,
        Right_Collar,
        Right_Shoulder,
        Right_Forearm,
        Right_Hand,
        RightFingerBase,
        RFingers,
        rThumb1,
        None
    }

    public static partial class EnumExtend
    {
        public static int Int(this CMUPartIdx i)
        {
            return (int)i;
        }
        public static float GetWeight(this CMUPartIdx i) {
            switch((CMUPartIdx) i){
                case CMUPartIdx.lToe:
                case CMUPartIdx.LFingers:
                case CMUPartIdx.LeftFingerBase:
                case CMUPartIdx.lThumb1:
                case CMUPartIdx.rToe:
                case CMUPartIdx.RFingers:
                case CMUPartIdx.RightFingerBase:
                case CMUPartIdx.rThumb1:
                case CMUPartIdx.Hip:
                case CMUPartIdx.Abdomen:
                case CMUPartIdx.Chest:
                    return 0;
                default:
                    return 1/20.0f;
            }
        }
        public static string GetName(this CMUPartIdx i)
        {
            return System.Enum.GetName(typeof(CMUPartIdx), i);
        }
        public static CMUPartIdx GetParent(this CMUPartIdx i)
        {
            switch (i)
            {
                case CMUPartIdx.Hip:
                    return CMUPartIdx.None;
                case CMUPartIdx.lButtock:
                    return CMUPartIdx.Hip;
                case CMUPartIdx.Left_Thigh:
                    return CMUPartIdx.lButtock;
                case CMUPartIdx.Left_Shin:
                    return CMUPartIdx.Left_Thigh;
                case CMUPartIdx.Left_Foot:
                    return CMUPartIdx.Left_Shin;
                case CMUPartIdx.lToe:
                    return CMUPartIdx.Left_Foot;
                case CMUPartIdx.rButtock:
                    return CMUPartIdx.Hip;
                case CMUPartIdx.Right_Thigh:
                    return CMUPartIdx.rButtock;
                case CMUPartIdx.Right_Shin:
                    return CMUPartIdx.Right_Thigh;
                case CMUPartIdx.Right_Foot:
                    return CMUPartIdx.Right_Shin;
                case CMUPartIdx.rToe:
                    return CMUPartIdx.Right_Foot;
                case CMUPartIdx.Waist:
                    return CMUPartIdx.Hip;
                case CMUPartIdx.Abdomen:
                    return CMUPartIdx.Waist;
                case CMUPartIdx.Chest:
                    return CMUPartIdx.Abdomen;
                case CMUPartIdx.Neck:
                    return CMUPartIdx.Chest;
                case CMUPartIdx.Neck1:
                    return CMUPartIdx.Neck;
                case CMUPartIdx.Head:
                    return CMUPartIdx.Neck1;
                case CMUPartIdx.Left_Collar:
                    return CMUPartIdx.Chest;
                case CMUPartIdx.Left_Shoulder:
                    return CMUPartIdx.Left_Collar;
                case CMUPartIdx.Left_Forearm:
                    return CMUPartIdx.Left_Shoulder;
                case CMUPartIdx.Left_Hand:
                    return CMUPartIdx.Left_Forearm;
                case CMUPartIdx.LeftFingerBase:
                    return CMUPartIdx.Left_Hand;
                case CMUPartIdx.LFingers:
                    return CMUPartIdx.LeftFingerBase;
                case CMUPartIdx.lThumb1:
                    return CMUPartIdx.Left_Hand;
                case CMUPartIdx.Right_Collar:
                    return CMUPartIdx.Chest;
                case CMUPartIdx.Right_Shoulder:
                    return CMUPartIdx.Right_Collar;
                case CMUPartIdx.Right_Forearm:
                    return CMUPartIdx.Right_Shoulder;
                case CMUPartIdx.Right_Hand:
                    return CMUPartIdx.Right_Forearm;
                case CMUPartIdx.RightFingerBase:
                    return CMUPartIdx.Right_Hand;
                case CMUPartIdx.RFingers:
                    return CMUPartIdx.RightFingerBase;
                case CMUPartIdx.rThumb1:
                    return CMUPartIdx.Right_Hand;
                default:
                    return CMUPartIdx.None;
            }
        }

    }

}