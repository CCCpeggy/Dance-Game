using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BVH {
    public class BVHPartObject: MonoBehaviour{
        public int PartIdx = -1;
        public Vector3 Offset = new Vector3();
        public BVHObject BvhObj = null;
        public List<BVHPartObject> Child = new List<BVHPartObject>();
        public BVHPartObject Parent = null;
        public BVHPartObject Clone(BVHObject belongBVH, BVHPartObject parentObject=null){
            BVHPartObject newPart = BVHPartObject.CreateGameObject(name, parentObject, belongBVH);
            newPart.PartIdx = PartIdx;
            newPart.Offset = Offset;
            foreach(var child in Child) {
                child.Clone(belongBVH, newPart);
            }
            return newPart;
        }
// D:\workplace\3D遊戲\P2\motion cmu data\01-02\01_02b.bvh
        public static BVHPartObject ReadPart(ref IEnumerator<string> bvhDataIter, BVHObject obj, BVHPartObject parentObject=null) {
            List<string> partNameList = new List<string>();
            while (true){
                string tmpPartName = Utility.IterData.GetAndNext(ref bvhDataIter);
                if (tmpPartName == "{") break;
                partNameList.Add(tmpPartName);
            }
            string partName = string.Join("_", partNameList);
            BVHPartObject partObject = BVHPartObject.CreateGameObject(partName, parentObject, obj);
            while (true){
                string input = Utility.IterData.GetAndNext(ref bvhDataIter);
                switch(input) {
                    case "OFFSET":
                        partObject.Offset = Utility.IterData.GetVec3AndNext(ref bvhDataIter);
                        break;
                    case "CHANNELS":
                        int channelAmount = int.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
                        for (int i = 0; i < channelAmount; i++) {
                            string channelName = Utility.IterData.GetAndNext(ref bvhDataIter);
                            int idx = 0;
                            switch(channelName.Substring(1)) {
                                case "rotation":
                                    idx = 0;
                                    break;
                                case "position":
                                    idx = 3;
                                    break;
                                default:
                                    Debug.LogError("非預期輸入");
                                    break;
                            }
                            switch(channelName[0]) {
                                case 'X':
                                    idx += 0;
                                    break;
                                case 'Y':
                                    idx += 1;
                                    break;
                                case 'Z':
                                    idx += 2;
                                    break;
                                default:
                                    Debug.LogError("非預期輸入");
                                    break;
                            }
                            var channelData = new Tuple<BVHPartObject, int>(partObject, idx);
                            obj.ChannelDatas.Add(channelData);
                        }
                        break;
                    case "JOINT":
                        var childObj = ReadPart(ref bvhDataIter, obj, partObject);
                        break;
                    case "End":
                        Utility.IterData.CheckAndNext(ref bvhDataIter, "Site");
                        Utility.IterData.CheckAndNext(ref bvhDataIter, "{");
                        Utility.IterData.CheckAndNext(ref bvhDataIter, "OFFSET");
                        var endObj = BVHPartObject.CreateGameObject("End", partObject, obj);
                        endObj.Offset = Utility.IterData.GetVec3AndNext(ref bvhDataIter);
                        Utility.IterData.CheckAndNext(ref bvhDataIter, "}");
                        break;
                    case "}":
                        return partObject;
                    default:
                        Debug.LogError(input + "為非預期輸入");
                        return null;
                }
            }
        }

        public void setPosOrRot(int idx, float num) {
            if (idx == 0) transform.rotation *= Quaternion.Euler(num, 0, 0);
            else if (idx == 1) transform.rotation *= Quaternion.Euler(0, num, 0);
            else if (idx == 2) transform.rotation *= Quaternion.Euler(0, 0, num);

            else if (idx == 3) transform.position += new Vector3(num, 0, 0);
            else if (idx == 4) transform.position += new Vector3(0, num, 0);
            else if (idx == 5) transform.position += new Vector3(0, 0, num);
        }


        public static BVHPartObject CreateGameObject(string name, BVHPartObject parentObject, BVHObject bvhObj){
            GameObject gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gobj.name = name;
            var obj = gobj.AddComponent<BVHPartObject>();
            obj.BvhObj = bvhObj;
            if (parentObject){
                parentObject.AddChild(obj);
                LineRenderer lr = gobj.AddComponent<LineRenderer>();
                lr.startWidth = 0.5f;
                lr.endWidth = 0.5f;
                lr.SetPosition(0, obj.gameObject.transform.position);
                lr.SetPosition(1, parentObject.gameObject.transform.position);
            }
            return obj;
        }
        public void AddChild(BVHPartObject childObj){
            Child.Add(childObj);
            childObj.Parent = this;
            childObj.transform.parent = transform;
        }
    
        public void UpdateSingleLine(){
            LineRenderer lr = GetComponent<LineRenderer>();
            if (lr) {
                lr.SetPosition(0, gameObject.transform.position);
                lr.SetPosition(1, Parent.gameObject.transform.position);

                //Ellipsoids.drawEllipsoids(Parent.gameObject.transform.position, gameObject.transform.position, lr);
                //Ellipsoids_Sphere.setEllipsoid(Parent.gameObject.transform.position, gameObject.transform.position);
            }
        }
        public void UpdateMutiLines(){
            UpdateSingleLine();
            foreach(var c in Child){
                c.UpdateSingleLine();
            }
        }

    }
}