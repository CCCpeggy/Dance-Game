/*
From: Digital- Standard Co., Ltd.
Source: https://github.com/digital-standard/ThreeDPoseUnityBarracuda
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Position index of joint points
/// </summary>

public class EstimateModel : MonoBehaviour
{
    private List<VNectModel.Skeleton> Skeletons = new List<VNectModel.Skeleton>();
    public Material SkeletonMaterial;

    public bool ShowSkeleton;
    private bool useSkeleton;
    public float SkeletonX;
    public float SkeletonY;
    public float SkeletonZ;
    public float SkeletonScale;

    public DancingGameDemo DancingGameDemo;
    
    // Joint position and bone
    private VNectModel.JointPoint[] jointPoints;
    public VNectModel.JointPoint[] JointPoints { get { return jointPoints; } }

    // Pose
    private GameObject poseGameObj;
    private Pose.Object poseObj;

    private void Update()
    {
        if (jointPoints != null)
        {
           PoseUpdate();
        }
    }

    /// <summary>
    /// Initialize joint points
    /// </summary>
    /// <returns></returns>
    public VNectModel.JointPoint[] Init()
    {
        poseGameObj = Pose.Object.CreatePoseObj();
        poseObj = poseGameObj.GetComponent<Pose.Object>();
        jointPoints = new VNectModel.JointPoint[PositionIndex.Count.Int()];
        for (var i = 0; i < PositionIndex.Count.Int(); i++) {
            jointPoints[i] = new VNectModel.JointPoint();
            poseObj.Part[i].name = System.Enum.GetName(typeof(PositionIndex), i);
        }

        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Transform = poseObj.Part[PositionIndex.rShldrBend.Int()].transform;
        jointPoints[PositionIndex.rForearmBend.Int()].Transform = poseObj.Part[PositionIndex.rForearmBend.Int()].transform;
        jointPoints[PositionIndex.rHand.Int()].Transform = poseObj.Part[PositionIndex.rHand.Int()].transform;
        jointPoints[PositionIndex.rThumb2.Int()].Transform = poseObj.Part[PositionIndex.rThumb2.Int()].transform;
        jointPoints[PositionIndex.rMid1.Int()].Transform = poseObj.Part[PositionIndex.rMid1.Int()].transform;
        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Transform = poseObj.Part[PositionIndex.lShldrBend.Int()].transform;
        jointPoints[PositionIndex.lForearmBend.Int()].Transform = poseObj.Part[PositionIndex.lForearmBend.Int()].transform;
        jointPoints[PositionIndex.lHand.Int()].Transform = poseObj.Part[PositionIndex.lHand.Int()].transform;
        jointPoints[PositionIndex.lThumb2.Int()].Transform = poseObj.Part[PositionIndex.lThumb2.Int()].transform;
        jointPoints[PositionIndex.lMid1.Int()].Transform = poseObj.Part[PositionIndex.lMid1.Int()].transform;

        // Face
        jointPoints[PositionIndex.lEar.Int()].Transform = poseObj.Part[PositionIndex.lEar.Int()].transform;
        jointPoints[PositionIndex.lEye.Int()].Transform = poseObj.Part[PositionIndex.lEye.Int()].transform;
        jointPoints[PositionIndex.rEar.Int()].Transform = poseObj.Part[PositionIndex.rEar.Int()].transform;
        jointPoints[PositionIndex.rEye.Int()].Transform = poseObj.Part[PositionIndex.rEye.Int()].transform;
        jointPoints[PositionIndex.Nose.Int()].Transform = poseObj.Part[PositionIndex.Nose.Int()].transform;

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Transform = poseObj.Part[PositionIndex.rThighBend.Int()].transform;
        jointPoints[PositionIndex.rShin.Int()].Transform = poseObj.Part[PositionIndex.rShin.Int()].transform;
        jointPoints[PositionIndex.rFoot.Int()].Transform = poseObj.Part[PositionIndex.rFoot.Int()].transform;
        jointPoints[PositionIndex.rToe.Int()].Transform = poseObj.Part[PositionIndex.rToe.Int()].transform;

        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Transform = poseObj.Part[PositionIndex.lThighBend.Int()].transform;
        jointPoints[PositionIndex.lShin.Int()].Transform = poseObj.Part[PositionIndex.lShin.Int()].transform;
        jointPoints[PositionIndex.lFoot.Int()].Transform = poseObj.Part[PositionIndex.lFoot.Int()].transform;
        jointPoints[PositionIndex.lToe.Int()].Transform = poseObj.Part[PositionIndex.lToe.Int()].transform;

        // etc
        jointPoints[PositionIndex.abdomenUpper.Int()].Transform = poseObj.Part[PositionIndex.abdomenUpper.Int()].transform;
        jointPoints[PositionIndex.hip.Int()].Transform = poseObj.Part[PositionIndex.hip.Int()].transform;
        jointPoints[PositionIndex.head.Int()].Transform = poseObj.Part[PositionIndex.head.Int()].transform;
        jointPoints[PositionIndex.neck.Int()].Transform = poseObj.Part[PositionIndex.neck.Int()].transform;
        jointPoints[PositionIndex.spine.Int()].Transform = poseObj.Part[PositionIndex.spine.Int()].transform;


        poseObj.Root = poseObj.Part[PositionIndex.hip.Int()];
        // Child Settings
        // Right Arm
        poseObj.Part[PositionIndex.neck.Int()].AddChild(poseObj.Part[PositionIndex.rShldrBend.Int()]);
        poseObj.Part[PositionIndex.rShldrBend.Int()].AddChild(poseObj.Part[PositionIndex.rForearmBend.Int()]);
        poseObj.Part[PositionIndex.rForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.rHand.Int()]);
        poseObj.Part[PositionIndex.rHand.Int()].AddChild(poseObj.Part[PositionIndex.rMid1.Int()]);

        // Left Arm
        poseObj.Part[PositionIndex.neck.Int()].AddChild(poseObj.Part[PositionIndex.lShldrBend.Int()]);
        poseObj.Part[PositionIndex.lShldrBend.Int()].AddChild(poseObj.Part[PositionIndex.lForearmBend.Int()]);
        poseObj.Part[PositionIndex.lForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.lHand.Int()]);
        poseObj.Part[PositionIndex.lHand.Int()].AddChild(poseObj.Part[PositionIndex.lMid1.Int()]);

        // Fase

        // Right Leg
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.rThighBend.Int()]);
        poseObj.Part[PositionIndex.rThighBend.Int()].AddChild(poseObj.Part[PositionIndex.rShin.Int()]);
        poseObj.Part[PositionIndex.rShin.Int()].AddChild(poseObj.Part[PositionIndex.rFoot.Int()]);
        poseObj.Part[PositionIndex.rFoot.Int()].AddChild(poseObj.Part[PositionIndex.rToe.Int()]);

        // Left Leg
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.lThighBend.Int()]);
        poseObj.Part[PositionIndex.lThighBend.Int()].AddChild(poseObj.Part[PositionIndex.lShin.Int()]);
        poseObj.Part[PositionIndex.lShin.Int()].AddChild(poseObj.Part[PositionIndex.lFoot.Int()]);
        poseObj.Part[PositionIndex.lFoot.Int()].AddChild(poseObj.Part[PositionIndex.lToe.Int()]);

        // etc
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.spine.Int()]);
        poseObj.Part[PositionIndex.spine.Int()].AddChild(poseObj.Part[PositionIndex.neck.Int()]);
        poseObj.Part[PositionIndex.neck.Int()].AddChild(poseObj.Part[PositionIndex.head.Int()]);

        DancingGameDemo.SetRecordingObject(poseObj);

        jointPoints[PositionIndex.hip.Int()].score3D = 1f;
        jointPoints[PositionIndex.neck.Int()].score3D = 1f;
        jointPoints[PositionIndex.Nose.Int()].score3D = 1f;
        jointPoints[PositionIndex.head.Int()].score3D = 1f;
        jointPoints[PositionIndex.spine.Int()].score3D = 1f;
        return JointPoints;
    }

    public void PoseUpdate()
    {
        if (poseObj.Status == Pose.Object.StatusType.Playing) return;
       
        Vector3 hipPos = jointPoints[PositionIndex.hip.Int()].Pos3D * SkeletonScale;
        foreach (var jointPoint in jointPoints)
        {
            Vector3 pos = jointPoint.Pos3D * SkeletonScale;
            var parent = jointPoint.Transform.GetComponent<Pose.PartObject>().Parent;
            if (parent == null) {
                jointPoint.Transform.localPosition = pos - hipPos;
            }
            else {
                Vector3 parentPos = jointPoints[parent.PartIdx].Pos3D * SkeletonScale;
                jointPoint.Transform.localPosition = pos - parentPos;
            }
        }
        
        // foreach (var sk in Skeletons)
        // {
        //     var s = sk.start;
        //     var e = sk.end;

        //     sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * SkeletonScale + SkeletonX, s.Pos3D.y * SkeletonScale + SkeletonY, s.Pos3D.z * SkeletonScale + SkeletonZ));
        //     sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * SkeletonScale + SkeletonX, e.Pos3D.y * SkeletonScale + SkeletonY, e.Pos3D.z * SkeletonScale + SkeletonZ));
        // }
        if (poseObj.Status == Pose.Object.StatusType.Recording)
            poseObj.Motion.Record();
    }

    public void PoseRecordEnd() {
        if (poseObj.Status == Pose.Object.StatusType.Recording) {
            poseObj.Status = Pose.Object.StatusType.Playing;

            // 將錄製下來的骨架格式轉成跟 CMU 資料集的格式相同 (關節對應是寫死的)
            var recordPose = Pose.VNectToCMU.Convert(poseObj);
            
            // 由於錄製下來的骨架是以關節位置做儲存，為了後續的處理，所以要轉成旋轉角度儲存
            recordPose.Motion.ToRotationType();

            // 後續處理
            DancingGameDemo.BindRefAndRealPose(recordPose);
            GameObject.Destroy(poseObj.gameObject);
        }
    }
    public void PoseRecordStart() {
        poseObj.Status = Pose.Object.StatusType.Recording;
    }
    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(VNectModel.JointPoint p1, VNectModel.JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }

    /// <summary>
    /// Add skelton from joint points
    /// </summary>
    /// <param name="s">position index</param>
    /// <param name="e">position index</param>
    private void AddSkeleton(PositionIndex s, PositionIndex e)
    {
        var sk = new VNectModel.Skeleton()
        {
            LineObject = new GameObject("Line"),
            start = jointPoints[s.Int()],
            end = jointPoints[e.Int()],
        };

        sk.Line = sk.LineObject.AddComponent<LineRenderer>();
        sk.Line.startWidth = 0.04f;
        sk.Line.endWidth = 0.01f;
        
        // define the number of vertex
        sk.Line.positionCount = 2;
        sk.Line.material = SkeletonMaterial;

        Skeletons.Add(sk);
    }
}
