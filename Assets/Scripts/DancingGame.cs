using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DancingGame : MonoBehaviour
{
    public List<Pose.Object> PoseObjects = new List<Pose.Object>();
    void Start() {
        PoseObjects.Clear();
        //var motion1 = Pose.Object.CreatePoseObjByBVH(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_03b.bvh", true);
        //var motion2 = Pose.Object.CreatePoseObjByBVH(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_04b.bvh", true);
        //new Pose.TimeWarping(motion1.GetComponent<Pose.Object>(), motion2.GetComponent<Pose.Object>()).Do();
    }
}

// [ExecuteInEditMode]
[CustomEditor(typeof(DancingGame))]
public class bvhEditor : Editor
{
    string bvhFilePath;
    bool isTPoseType = false;
    public override void OnInspectorGUI()
    {
        DancingGame poses = (DancingGame)target;
        serializedObject.Update();
        bvhFilePath = EditorGUILayout.TextField("Bvh File Path", bvhFilePath);
        isTPoseType = EditorGUILayout.Toggle("Is T Pose Type", isTPoseType);
        if(GUILayout.Button("Create"))
        {
            try
            {
                var tmp = Pose.Object.CreatePoseObjByBVH(bvhFilePath, isTPoseType);
                poses.PoseObjects.Add(tmp.GetComponent<Pose.Object>());
            }
            catch
            {
                Debug.Log("Something Error");
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}