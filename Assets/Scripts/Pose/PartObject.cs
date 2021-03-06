using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pose {
    public class PartObject: MonoBehaviour{
        public int PartIdx = -1;
        public Vector3 Offset = new Vector3();
        public Object PoseObj = null;
        public List<PartObject> Child = new List<PartObject>();
        public PartObject Parent = null;
        public PartObject Clone(Object belongObj, PartObject parentObject=null){
            PartObject newPart = PartObject.CreateGameObject(name, parentObject, belongObj);
            newPart.PartIdx = PartIdx;
            newPart.Offset = Offset;
            foreach(var child in Child) {
                child.Clone(belongObj, newPart);
            }
            return newPart;
        }
        // public PartObject CreatePartObject(Object belongObj, int partIdx, Vector3 offset) {
        //     PartObject newPart = PartObject.CreateGameObject(name, parentObject, belongObj);
        //     newPart.PartIdx = partIdx;
        //     newPart.Offset = offset;
        //     foreach(var child in Child) {
        //         child.Clone(belongObj, newPart);
        //     }
        //     return newPart;
        // }

        public static PartObject ReadPart(ref IEnumerator<string> bvhDataIter, Object obj, PartObject parentObject=null) {
            List<string> partNameList = new List<string>();
            while (true){
                string tmpPartName = Utility.IterData.GetAndNext(ref bvhDataIter);
                if (tmpPartName == "{") break;
                partNameList.Add(tmpPartName);
            }
            string partName = string.Join("_", partNameList);
            PartObject partObject = PartObject.CreateGameObject(partName, parentObject, obj);
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
                                    Debug.LogError("???????????????");
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
                                    Debug.LogError("???????????????");
                                    break;
                            }
                            var channelData = new Tuple<PartObject, int>(partObject, idx);
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
                        var endObj = PartObject.CreateGameObject("End", partObject, obj);
                        endObj.Offset = Utility.IterData.GetVec3AndNext(ref bvhDataIter);
                        Utility.IterData.CheckAndNext(ref bvhDataIter, "}");
                        break;
                    case "}":
                        return partObject;
                    default:
                        Debug.LogError(input + "??????????????????");
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


        public static PartObject CreateGameObject(string name, PartObject parentObject, Object poseObj){
            GameObject gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gobj.name = name;
            var obj = gobj.AddComponent<PartObject>();
            obj.PoseObj = poseObj;
            if (parentObject){
                parentObject.AddChild(obj);
                // Ellipsoids.CreateEllipsoid(obj.gameObject, parentObject.gameObject);
            }
            return obj;
        }
        public void AddChild(PartObject childObj){
            Child.Add(childObj);
            childObj.Parent = this;
            childObj.transform.parent = transform;
            LineRenderer lr = childObj.gameObject.AddComponent<LineRenderer>();
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.SetPosition(0, childObj.gameObject.transform.position);
            lr.SetPosition(1, gameObject.transform.position);
        }
    
        public void UpdateSingleLine(){
            LineRenderer lr = GetComponent<LineRenderer>();
            if (lr) {
                lr.SetPosition(0, gameObject.transform.position);
                lr.SetPosition(1, Parent.gameObject.transform.position);

                // Ellipsoids.drawEllipsoids(Parent.gameObject.transform.position, gameObject.transform.position, lr);
                // Ellipsoids_Sphere.setEllipsoid(Parent.gameObject.transform.position, gameObject.transform.position);
            }
        }
        void Update()
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            if (lr && Parent) {
                lr.SetPosition(0, gameObject.transform.position);
                lr.SetPosition(1, Parent.gameObject.transform.position);

                // Ellipsoids.drawEllipsoids(Parent.gameObject.transform.position, gameObject.transform.position, lr);
                // Ellipsoids_Sphere.setEllipsoid(Parent.gameObject.transform.position, gameObject.transform.position);
            }
        }

    }
}