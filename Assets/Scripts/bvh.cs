using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class bvh : MonoBehaviour
{
    public List<BVH.BVHObject> BVHObjects = new List<BVH.BVHObject>();
    void Start() {
        BVHObjects.Clear();
        BVH.BVHObject.CreateBVHObjectByPath(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_03b.bvh", true);
        BVH.BVHObject.CreateBVHObjectByPath(@"D:\workplace\3D遊戲\P2\motion cmu data\08-09\09_04b.bvh", true);
    }
}

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
        if (GUILayout.Button("Registrationi Curves"))
        {
            BVH.Blend.Do(myBvh.BVHObjects);
        }
        serializedObject.ApplyModifiedProperties();
    }
}