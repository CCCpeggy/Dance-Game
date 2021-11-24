using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class bvh : MonoBehaviour
{
    public List<BVH.BVHObject> BVHObjects = new List<BVH.BVHObject>();
    void Start() {
        BVHObjects.Clear();
        var motion1 = BVH.BVHObject.CreateBVHObjectByPath(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_03b.bvh", true);
        var motion2 = BVH.BVHObject.CreateBVHObjectByPath(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_04b.bvh", true);
        new BVH.TimeWarping(motion1.GetComponent<BVH.BVHObject>(), motion2.GetComponent<BVH.BVHObject>()).Do();
    }
}

// [ExecuteInEditMode]
[CustomEditor(typeof(bvh))]
public class bvhEditor : Editor
{
    string bvhFilePath;
    bool isTPoseType = false;
    public override void OnInspectorGUI()
    {
        bvh myBvh = (bvh)target;
        serializedObject.Update();
        bvhFilePath = EditorGUILayout.TextField("Bvh File Path", bvhFilePath);
        isTPoseType = EditorGUILayout.Toggle("Is T Pose Type", isTPoseType);
        if(GUILayout.Button("Create"))
        {
            try
            {
                var tmp = BVH.BVHObject.CreateBVHObjectByPath(bvhFilePath, isTPoseType);
                myBvh.BVHObjects.Add(tmp.GetComponent<BVH.BVHObject>());
            }
            catch
            {
                Debug.Log("Something Error");
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}