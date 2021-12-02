using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Pose {
    class VNectToCMU {

        public static Object Convert(Object vNectObj) {
            GameObject cmuObj = new GameObject();
            var cmuPose = cmuObj.AddComponent<Object>();
            cmuPose.Status = vNectObj.Status;
            cmuPose.Part = new PartObject[31];
            for (int i = 0; i < 31; i++) {
                CMUPartIdx cmuPart = (CMUPartIdx) i;
                string partObjName = cmuPart.GetName();
                CMUPartIdx parentIdx = cmuPart.GetParent();
                PartObject parentObj = parentIdx != CMUPartIdx.None ? cmuPose.Part[parentIdx.Int()] : null;
                cmuPose.Part[i] = PartObject.CreateGameObject(partObjName, parentObj, cmuPose);
            }
            cmuPose.Root = cmuPose.Part[CMUPartIdx.Hip.Int()];
            cmuPose.Root.transform.parent = cmuObj.transform;
            Motion vNectMotion = vNectObj.Motion;
            Motion cmuMotion = cmuPose.Motion = Motion.Create(cmuPose);
            cmuMotion.FrameCount = vNectObj.Motion.FrameCount;
            cmuMotion.FrameTime = vNectObj.Motion.FrameTime;

            for(int i = 0; i < vNectMotion.MotionData.Count; i++) {
                Motion.Frame cmuFrame = new Motion.Frame(31, Motion.Frame.JointType.Position);
                Motion.Frame vNectFrame = vNectMotion.MotionData[i];
                if (vNectFrame.type != Motion.Frame.JointType.Position) Debug.LogError("轉換型態不符合");
                cmuFrame.Position = vNectFrame.Position;
                for (int j = 0; j < 31; j++) {
                    cmuFrame.JointPosition[j] = getPositionFromVNect(vNectFrame, j);
                }
                cmuMotion.MotionData.Add(cmuFrame);
            }

            return cmuPose;
        }

        public static Vector3 getPositionFromVNect(Motion.Frame vNectFrame, int partIdx) {
            switch((CMUPartIdx) partIdx){
                case CMUPartIdx.Hip:
                    return vNectFrame.JointPosition[PositionIndex.hip.Int()];
                // case CMUPartIdx.lButtock:
                //     return vNectFrame.JointPosition[PositionIndex.hip.Int()];
                case CMUPartIdx.Left_Thigh:
                    return vNectFrame.JointPosition[PositionIndex.lThighBend.Int()];
                case CMUPartIdx.Left_Shin:
                    return vNectFrame.JointPosition[PositionIndex.lShin.Int()];
                case CMUPartIdx.Left_Foot:
                    return vNectFrame.JointPosition[PositionIndex.lFoot.Int()];
                case CMUPartIdx.lToe:
                    return vNectFrame.JointPosition[PositionIndex.lToe.Int()];
                // case CMUPartIdx.rButtock:
                //     return vNectFrame.JointPosition[PositionIndex.hip.Int()];
                case CMUPartIdx.Right_Thigh:
                    return vNectFrame.JointPosition[PositionIndex.rThighBend.Int()];
                case CMUPartIdx.Right_Shin:
                    return vNectFrame.JointPosition[PositionIndex.rShin.Int()];
                case CMUPartIdx.Right_Foot:
                    return vNectFrame.JointPosition[PositionIndex.rFoot.Int()];
                case CMUPartIdx.rToe:
                    return vNectFrame.JointPosition[PositionIndex.rToe.Int()];
                // case CMUPartIdx.Waist:
                //     return vNectFrame.JointPosition[PositionIndex.hip.Int()];
                case CMUPartIdx.Abdomen:
                    return vNectFrame.JointPosition[PositionIndex.abdomenUpper.Int()];
                case CMUPartIdx.Chest:
                    return vNectFrame.JointPosition[PositionIndex.neck.Int()];
                // case CMUPartIdx.Neck:
                //     return vNectFrame.JointPosition[PositionIndex.neck.Int()];
                case CMUPartIdx.Neck1:
                    return (vNectFrame.JointPosition[PositionIndex.head.Int()]
                    + vNectFrame.JointPosition[PositionIndex.neck.Int()]) / 2;
                case CMUPartIdx.Head:
                    return vNectFrame.JointPosition[PositionIndex.head.Int()];
                // case CMUPartIdx.Left_Collar:
                //     return vNectFrame.JointPosition[PositionIndex.neck.Int()];
                case CMUPartIdx.Left_Shoulder:
                    return vNectFrame.JointPosition[PositionIndex.lShldrBend.Int()];
                case CMUPartIdx.Left_Forearm:
                    return vNectFrame.JointPosition[PositionIndex.lForearmBend.Int()];
                case CMUPartIdx.Left_Hand:
                    return vNectFrame.JointPosition[PositionIndex.lHand.Int()];
                case CMUPartIdx.LeftFingerBase:
                    return vNectFrame.JointPosition[PositionIndex.lHand.Int()];
                case CMUPartIdx.LFingers:
                    return vNectFrame.JointPosition[PositionIndex.lMid1.Int()];
                case CMUPartIdx.lThumb1:
                    return vNectFrame.JointPosition[PositionIndex.lHand.Int()];
                // case CMUPartIdx.Right_Collar:
                //     return vNectFrame.JointPosition[PositionIndex.neck.Int()];
                case CMUPartIdx.Right_Shoulder:
                    return vNectFrame.JointPosition[PositionIndex.rShldrBend.Int()];
                case CMUPartIdx.Right_Forearm:
                    return vNectFrame.JointPosition[PositionIndex.rForearmBend.Int()];
                case CMUPartIdx.Right_Hand:
                    return vNectFrame.JointPosition[PositionIndex.rHand.Int()];
                case CMUPartIdx.RightFingerBase:
                    return vNectFrame.JointPosition[PositionIndex.rHand.Int()];
                case CMUPartIdx.RFingers:
                    return vNectFrame.JointPosition[PositionIndex.rMid1.Int()];
                case CMUPartIdx.rThumb1:
                    return vNectFrame.JointPosition[PositionIndex.rHand.Int()];
                default:
                    return new Vector3(0, 0, 0);
            }
            
        }
    }
}