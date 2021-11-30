using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new VNectModel.JointPoint();

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
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.rShldrBend.Int()]);
        poseObj.Part[PositionIndex.rShldrBend.Int()].AddChild(poseObj.Part[PositionIndex.rForearmBend.Int()]);
        poseObj.Part[PositionIndex.rForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.rHand.Int()]);
        // poseObj.Part[PositionIndex.rForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.rShldrBend.Int()]);

        // Left Arm
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.lShldrBend.Int()]);
        poseObj.Part[PositionIndex.lShldrBend.Int()].AddChild(poseObj.Part[PositionIndex.lForearmBend.Int()]);
        poseObj.Part[PositionIndex.lForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.lHand.Int()]);
        // poseObj.Part[PositionIndex.lForearmBend.Int()].AddChild(poseObj.Part[PositionIndex.lShldrBend.Int()]);

        // Fase

        // Right Leg
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.rThighBend.Int()]);
        poseObj.Part[PositionIndex.rThighBend.Int()].AddChild(poseObj.Part[PositionIndex.rShin.Int()]);
        poseObj.Part[PositionIndex.rShin.Int()].AddChild(poseObj.Part[PositionIndex.rFoot.Int()]);
        poseObj.Part[PositionIndex.rFoot.Int()].AddChild(poseObj.Part[PositionIndex.rToe.Int()]);
        // poseObj.Part[PositionIndex.rFoot.Int()].AddChild(poseObj.Part[PositionIndex.rShin.Int()]);

        // Left Leg
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.lThighBend.Int()]);
        poseObj.Part[PositionIndex.lThighBend.Int()].AddChild(poseObj.Part[PositionIndex.lShin.Int()]);
        poseObj.Part[PositionIndex.lShin.Int()].AddChild(poseObj.Part[PositionIndex.lFoot.Int()]);
        poseObj.Part[PositionIndex.lFoot.Int()].AddChild(poseObj.Part[PositionIndex.lToe.Int()]);
        // poseObj.Part[PositionIndex.lFoot.Int()].AddChild(poseObj.Part[PositionIndex.lShin.Int()]);

        // etc
        poseObj.Part[PositionIndex.hip.Int()].AddChild(poseObj.Part[PositionIndex.spine.Int()]);
        poseObj.Part[PositionIndex.spine.Int()].AddChild(poseObj.Part[PositionIndex.neck.Int()]);
        poseObj.Part[PositionIndex.neck.Int()].AddChild(poseObj.Part[PositionIndex.head.Int()]);
        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.Nose.Int()];

        useSkeleton = ShowSkeleton;
        if (useSkeleton)
        {
            // Line Child Settings
            // Right Arm
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.rForearmBend);
            AddSkeleton(PositionIndex.rForearmBend, PositionIndex.rHand);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rThumb2);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rMid1);

            // Left Arm
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.lForearmBend);
            AddSkeleton(PositionIndex.lForearmBend, PositionIndex.lHand);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lThumb2);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lMid1);

            // Fase
            AddSkeleton(PositionIndex.lEar, PositionIndex.Nose);
            AddSkeleton(PositionIndex.rEar, PositionIndex.Nose);

            // Right Leg
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShin);
            AddSkeleton(PositionIndex.rShin, PositionIndex.rFoot);
            AddSkeleton(PositionIndex.rFoot, PositionIndex.rToe);

            // Left Leg
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShin);
            AddSkeleton(PositionIndex.lShin, PositionIndex.lFoot);
            AddSkeleton(PositionIndex.lFoot, PositionIndex.lToe);

            // etc
            AddSkeleton(PositionIndex.spine, PositionIndex.neck);
            AddSkeleton(PositionIndex.neck, PositionIndex.head);
            AddSkeleton(PositionIndex.head, PositionIndex.Nose);
            AddSkeleton(PositionIndex.neck, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.neck, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.rThighBend);
        }

        // Set Inverse
        // var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.lThighBend.Int()].Transform.position, jointPoints[PositionIndex.rThighBend.Int()].Transform.position);
        // foreach (var jointPoint in jointPoints)
        // {
        //     if (jointPoint.Transform != null)
        //     {
        //         jointPoint.InitRotation = jointPoint.Transform.rotation;
        //     }

        //     if (jointPoint.Child != null)
        //     {
        //         jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child, forward);
        //         jointPoint.InverseRotation = jointPoint.Inverse * jointPoint.InitRotation;
        //     }
        // }
        // var hip = jointPoints[PositionIndex.hip.Int()];
        // initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;
        // hip.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        // hip.InverseRotation = hip.Inverse * hip.InitRotation;

        // // For Head Rotation
        // var head = jointPoints[PositionIndex.head.Int()];
        // head.InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
        // var gaze = jointPoints[PositionIndex.Nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
        // head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
        // head.InverseRotation = head.Inverse * head.InitRotation;
        
        // var lHand = jointPoints[PositionIndex.lHand.Int()];
        // var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        // lHand.InitRotation = lHand.Transform.rotation;
        // lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Transform.position - jointPoints[PositionIndex.lMid1.Int()].Transform.position, lf));
        // lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;

        // var rHand = jointPoints[PositionIndex.rHand.Int()];
        // var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        // rHand.InitRotation = jointPoints[PositionIndex.rHand.Int()].Transform.rotation;
        // rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position, rf));
        // rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

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
        // // caliculate movement range of z-coordinate from height
        // var t1 = Vector3.Distance(jointPoints[PositionIndex.head.Int()].Pos3D, jointPoints[PositionIndex.neck.Int()].Pos3D);
        // var t2 = Vector3.Distance(jointPoints[PositionIndex.neck.Int()].Pos3D, jointPoints[PositionIndex.spine.Int()].Pos3D);
        // var pm = (jointPoints[PositionIndex.rThighBend.Int()].Pos3D + jointPoints[PositionIndex.lThighBend.Int()].Pos3D) / 2f;
        // var t3 = Vector3.Distance(jointPoints[PositionIndex.spine.Int()].Pos3D, pm);
        // var t4r = Vector3.Distance(jointPoints[PositionIndex.rThighBend.Int()].Pos3D, jointPoints[PositionIndex.rShin.Int()].Pos3D);
        // var t4l = Vector3.Distance(jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.lShin.Int()].Pos3D);
        // var t4 = (t4r + t4l) / 2f;
        // var t5r = Vector3.Distance(jointPoints[PositionIndex.rShin.Int()].Pos3D, jointPoints[PositionIndex.rFoot.Int()].Pos3D);
        // var t5l = Vector3.Distance(jointPoints[PositionIndex.lShin.Int()].Pos3D, jointPoints[PositionIndex.lFoot.Int()].Pos3D);
        // var t5 = (t5r + t5l) / 2f;
        // var t = t1 + t2 + t3 + t4 + t5;


        // // Low pass filter in z direction
        // tall = t * 0.7f + prevTall * 0.3f;
        // prevTall = tall;

        // if (tall == 0)
        // {
        //     tall = centerTall;
        // }
        // var dz = (centerTall - tall) / centerTall * ZScale;

        // // movement and rotatation of center
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.rThighBend.Int()].Pos3D);
        // jointPoints[PositionIndex.hip.Int()].Transform.position = jointPoints[PositionIndex.hip.Int()].Pos3D * 0.005f + new Vector3(initPosition.x, initPosition.y, initPosition.z + dz);
        // jointPoints[PositionIndex.hip.Int()].Transform.rotation = Quaternion.LookRotation(forward) * jointPoints[PositionIndex.hip.Int()].InverseRotation;

        // rotate each of bones
        foreach (var jointPoint in jointPoints)
        {
            Vector3 vec = jointPoint.Pos3D * SkeletonScale;
            jointPoint.Transform.position = new Vector3(vec.x, vec.y + 1, vec.z);
            if (jointPoint.Transform.GetComponent<Pose.PartObject>().Parent == null) {
                jointPoint.Transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
        // poseObj.UpdateLines();

        // // Head Rotation
        // var gaze = jointPoints[PositionIndex.Nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
        // var f = TriangleNormal(jointPoints[PositionIndex.Nose.Int()].Pos3D, jointPoints[PositionIndex.rEar.Int()].Pos3D, jointPoints[PositionIndex.lEar.Int()].Pos3D);
        // var head = jointPoints[PositionIndex.head.Int()];
        // head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;
        
        // // Wrist rotation (Test code)
        // var lHand = jointPoints[PositionIndex.lHand.Int()];
        // var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        // lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Pos3D - jointPoints[PositionIndex.lMid1.Int()].Pos3D, lf) * lHand.InverseRotation;

        // var rHand = jointPoints[PositionIndex.rHand.Int()];
        // var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        // //rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;
        // rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;

        foreach (var sk in Skeletons)
        {
            var s = sk.start;
            var e = sk.end;

            sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * SkeletonScale + SkeletonX, s.Pos3D.y * SkeletonScale + SkeletonY, s.Pos3D.z * SkeletonScale + SkeletonZ));
            sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * SkeletonScale + SkeletonX, e.Pos3D.y * SkeletonScale + SkeletonY, e.Pos3D.z * SkeletonScale + SkeletonZ));
        }
        if (poseObj.Status == Pose.Object.StatusType.Recording)
            poseObj.Motion.Record();
    }

    public void PoseEnd() {
        poseObj.Status = Pose.Object.StatusType.Playing;
    }
    public void PoseStart() {
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
