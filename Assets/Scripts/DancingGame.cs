using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DancingGame : MonoBehaviour
{
    public List<Pose.Object> PoseObjects = new List<Pose.Object>();
    void Start()
    {
        PoseObjects.Clear();
        var motion1 = Pose.Object.CreatePoseObjByBVH(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_03b.bvh", true);
        // var motion2 = Pose.Object.CreatePoseObjByBVH(@"D:\workplace\3D遊戲\P2\motion cmu data\55b\55_10b.bvh", true);
        var newObj = motion1.GetComponent<Pose.Object>();
        int j = 50;
        newObj.Status = Pose.Object.StatusType.None;
        // BindPart bindPart = new BindPart(obj1, obj1);
        // var obj2 = bindPart.Get();
        // obj2.Status = Pose.Object.StatusType.None;
        // obj2.ApplyFrameByIdx(50);
        newObj.Motion.ApplyFrame(newObj.Motion.MotionData[j]);
        Vector3[] targetPos = new Vector3[31];
        var frame = newObj.Motion.MotionData[j];
        for (int i = 0; i < newObj.Part.Length; i++)
        {
            if (newObj.Part[i].Parent)
            {
                targetPos[i] = newObj.Part[i].transform.position - newObj.Part[i].Parent.transform.position;
                targetPos[i] = targetPos[i];
            }
        }
        for (int i = 0; i < newObj.Part.Length; i++)
        {
            if (newObj.Part[i].Parent == null) continue;
            switch ((Pose.CMUPartIdx)newObj.Part[i].Parent.PartIdx)
            {
                case Pose.CMUPartIdx.Left_Collar:
                case Pose.CMUPartIdx.Left_Shoulder:
                case Pose.CMUPartIdx.Left_Forearm:
                case Pose.CMUPartIdx.Left_Hand:
                case Pose.CMUPartIdx.LeftFingerBase:
                case Pose.CMUPartIdx.LFingers:
                case Pose.CMUPartIdx.lThumb1:
                    frame.JointRotation[i] = Quaternion.Euler(0, 0, 0);
                    if (newObj.Part[i].Parent)
                    {
                        var fromPos = newObj.Part[i].Offset;
                        var toPos = targetPos[i];
                        int parentIdx = newObj.Part[i].Parent.PartIdx;
                        newObj.Part[i].transform.localPosition = newObj.Part[i].Offset;
                        newObj.Part[i].Parent.transform.rotation = Quaternion.FromToRotation(fromPos, toPos);
                        // Debug.Log(fromPos + ", " + toPos + ", " + frame.JointRotation[parentIdx]);
                    }
                    break;
            }
        }
        // for (int i = 0; i < 31; i++) {
        //     switch ((Pose.CMUPartIdx) i)
        //     {
        //         case Pose.CMUPartIdx.Left_Collar:
        //         case Pose.CMUPartIdx.Left_Shoulder:
        //         case Pose.CMUPartIdx.Left_Forearm:
        //         case Pose.CMUPartIdx.Left_Hand:
        //         case Pose.CMUPartIdx.LeftFingerBase:
        //         case Pose.CMUPartIdx.LFingers:
        //         case Pose.CMUPartIdx.lThumb1:
        //         default:
        //             if (newObj.Part[i].Child.Count == 0) continue;
        //             targetPos[i] = newObj.Part[i].Child[0].transform.position - newObj.Part[i].transform.position;
        //             // newObj.Motion.MotionData[j].JointRotation[i] = newObj.Part[i].transform.localRotation;
        //         break;
        //     }
        // }
        // for (int i = 0; i < 31; i++) {
        //     switch ((Pose.CMUPartIdx) i)
        //     {
        //         case Pose.CMUPartIdx.Left_Collar:
        //         case Pose.CMUPartIdx.Left_Shoulder:
        //         case Pose.CMUPartIdx.Left_Forearm:
        //         case Pose.CMUPartIdx.Left_Hand:
        //         case Pose.CMUPartIdx.LeftFingerBase:
        //         case Pose.CMUPartIdx.LFingers:
        //         case Pose.CMUPartIdx.lThumb1:
        //         default:
        //             if (newObj.Part[i].Child.Count == 0) continue;
        //             Debug.Log("frame " + j + ": " + newObj.Part[i].transform.rotation);
        //             Vector3 fromPos = newObj.Part[i].Offset;
        //             Vector3 toPos = targetPos[i];
        //             if (toPos.magnitude < 0.01) continue;
        //             newObj.Part[i].transform.localPosition = newObj.Part[i].Offset;
        //             newObj.Part[i].transform.rotation = Quaternion.FromToRotation(fromPos, toPos);
        //             Debug.Log("frame " + j + ": " + newObj.Part[i].transform.rotation);
        //             // newObj.Motion.MotionData[j].JointRotation[i] = newObj.Part[i].transform.localRotation;
        //         break;
        //     }
        // }
        // new Pose.TimeWarping(motion1.GetComponent<Pose.Object>(), motion2.GetComponent<Pose.Object>()).Do();
    }
}
// }

// // [ExecuteInEditMode]
// [CustomEditor(typeof(DancingGame))]
// public class bvhEditor : Editor
// {
//     string bvhFilePath;
//     bool isTPoseType = false;
//     public override void OnInspectorGUI()
//     {
//         DancingGame poses = (DancingGame)target;
//         serializedObject.Update();
//         bvhFilePath = EditorGUILayout.TextField("Bvh File Path", bvhFilePath);
//         isTPoseType = EditorGUILayout.Toggle("Is T Pose Type", isTPoseType);
//         if(GUILayout.Button("Create"))
//         {
//             try
//             {
//                 var tmp = Pose.Object.CreatePoseObjByBVH(bvhFilePath, isTPoseType);
//                 poses.PoseObjects.Add(tmp.GetComponent<Pose.Object>());
//             }
//             catch
//             {
//                 Debug.Log("Something Error");
//             }
//         }
//         serializedObject.ApplyModifiedProperties();
//     }
// }