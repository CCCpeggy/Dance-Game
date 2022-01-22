using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace Pose {
    public class Object: MonoBehaviour{
        public PartObject Root;
        public Motion Motion;
        public List<Tuple<PartObject, int>> ChannelDatas = new List<Tuple<PartObject, int>>();
        public PartObject[] Part;
        public enum StatusType {
            None,
            Recording,
            Playing
        }
        public StatusType Status;
        public static GameObject CreatePoseObjByBVH(string filename, bool isTPoseType=false) {
            GameObject gameObject = new GameObject();
            var filenameArr = filename.Split('\\');
            gameObject.name = filenameArr[filenameArr.Length - 1].Split('.')[0];
            var poseObject = gameObject.AddComponent<Object>();
            string bvhStrData = System.IO.File.ReadAllText(filename);
            poseObject.Read(bvhStrData, isTPoseType);
            poseObject.Status = StatusType.Playing;
            return gameObject;
        }
        public static GameObject CreatePoseObj() {
            GameObject gameObject = new GameObject();
            gameObject.name = "pose";
            var poseObject = gameObject.AddComponent<Object>();
            poseObject.Part = new PartObject[PositionIndex.Count.Int()];
            for (int i = 0; i < PositionIndex.Count.Int(); i++) {
                poseObject.Part[i] = PartObject.CreateGameObject("joint"+i, null, poseObject);
                poseObject.Part[i].PartIdx = i;
                poseObject.Part[i].transform.parent = poseObject.transform;
            }
            poseObject.Status = StatusType.None;
            poseObject.Motion = Motion.Create(poseObject);
            return gameObject;
        }
        public Object Clone(bool isMotion=true){
            GameObject newObj = new GameObject();
            var newPose = newObj.AddComponent<Object>();
            newPose.Status = Status;
            newPose.Root = Root.Clone(newPose);
            newPose.Root.transform.parent = newObj.transform;
            newPose.RenamePartCMU();
            if (isMotion) {
                newPose.Motion = Motion.Clone(newPose);
            }
            // foreach(var data in ChannelDatas) {
            //     var part = newBVH.Part[Utility.GetPartIdxByName(data.Item1.name)];
            //     newBVH.ChannelDatas.Add(new Tuple<BVHPartObject, int>(part, data.Item2));
            // }
            return newPose;
        }

        public void Read(string poseStrData, bool isTPoseType=false) {
            var poseDataIter = Pose.Utility.SplitString(poseStrData).GetEnumerator();
            
            poseDataIter.MoveNext();
            Utility.IterData.CheckAndNext(ref poseDataIter, "HIERARCHY");
            Utility.IterData.CompareAndNext(ref poseDataIter, "ROOT");
            Root = PartObject.ReadPart(ref poseDataIter, this);
            Root.transform.parent = transform;
            RenamePartCMU();
            Utility.IterData.CompareAndNext(ref poseDataIter, "MOTION");
            Motion = Motion.readMotion(ref poseDataIter, this, isTPoseType);
        }
        public void UpdateLines(PartObject partObject=null){
            if (partObject == null) partObject = Root;
            foreach(var childObj in partObject.Child){
                UpdateLines(childObj);
            }
            partObject.UpdateSingleLine();
        }

        public void ApplyFrameByIdx(float frameIdx) {
            Motion.ApplyFrame(frameIdx);
        }
        public void ApplyFrame(float time) {
            //time += deltaTime;
            float frameIdx = time / Motion.FrameTime;
            float frame = frameIdx - ((int)(frameIdx / Motion.FrameCount)) * Motion.FrameCount;
            Motion.ApplyFrame(frame);
        }

        public void RenamePartCMU() {
            Part = new PartObject[31];
            List<PartObject> bfs = new List<PartObject>();
            bfs.Add(Root);
            int queueIdx = 0;
            while (queueIdx < bfs.Count) {
                var cur = bfs[queueIdx++];
                if (cur.name == "End") continue;
                int idx = Pose.Utility.GetPartIdxByCMUName(cur.name);
                if (idx >= 0) {
                    Part[idx] = cur;
                    cur.PartIdx = idx;
                }
                else{
                    Debug.LogError(cur.name + "找不到對應部位");
                    Assert.IsTrue(false);
                }
                foreach (var child in cur.Child) {
                    bfs.Add(child);
                }
            }
        }

        void LateUpdate()
        {
            switch(Status) {
                case StatusType.None:
                    break;
                case StatusType.Recording:
                    Motion.Record();
                    break;
                case StatusType.Playing:
                    if(Root && gameObject.activeSelf){
                        ApplyFrame(Time.time);
                    }
                    break;
            }
        }


    }

}