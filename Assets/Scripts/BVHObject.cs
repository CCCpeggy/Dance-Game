using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace BVH {
    public class BVHObject: MonoBehaviour{
        public BVHPartObject Root;
        public BVHMotion Motion;
        public List<Tuple<BVHPartObject, int>> ChannelDatas = new List<Tuple<BVHPartObject, int>>();
        public BVHPartObject[] Part;
        public static GameObject CreateBVHObjectByPath(string filename, bool isTPoseType=false) {
            GameObject gameObject = new GameObject();
            var filenameArr = filename.Split('\\');
            gameObject.name = filenameArr[filenameArr.Length - 1].Split('.')[0];
            var bvhObject = gameObject.AddComponent<BVHObject>();
            string bvhStrData = System.IO.File.ReadAllText(filename);
            bvhObject.Read(bvhStrData, isTPoseType);
            return gameObject;
        }
        public BVHObject Clone(bool isMotion=true){
            GameObject newObj = new GameObject();
            var newBVH = newObj.AddComponent<BVHObject>();
            newBVH.Root = Root.Clone(newBVH);
            newBVH.Root.transform.parent = newObj.transform;
            newBVH.RenamePartCMU();
            if (isMotion) {
                newBVH.Motion = Motion.Clone(newBVH);
            }
            // foreach(var data in ChannelDatas) {
            //     var part = newBVH.Part[Utility.GetPartIdxByName(data.Item1.name)];
            //     newBVH.ChannelDatas.Add(new Tuple<BVHPartObject, int>(part, data.Item2));
            // }
            return newBVH;
        }

        public void Read(string bvhStrData, bool isTPoseType=false) {
            var bvhDataIter = BVH.Utility.SplitString(bvhStrData).GetEnumerator();
            
            bvhDataIter.MoveNext();
            Utility.IterData.CheckAndNext(ref bvhDataIter, "HIERARCHY");
            Utility.IterData.CompareAndNext(ref bvhDataIter, "ROOT");
            Root = BVHPartObject.ReadPart(ref bvhDataIter, this);
            Root.transform.parent = transform;
            RenamePartCMU();
            Utility.IterData.CompareAndNext(ref bvhDataIter, "MOTION");
            Motion = BVHMotion.readMotion(ref bvhDataIter, this, isTPoseType);
        }
        public void UpdateLines(BVHPartObject partObject=null){
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

        public void RenamePart() {
            Part = new BVHPartObject[] {
                null, null, null, null, null, null, null, null, null, 
                null, null, null, null, null, null, null, null, null
            };
            Root.name = "Hips";
            Part[0] = Root;
            bool chest = false, leftLeg = false, rightLeg = false;
            Assert.IsTrue(Root.Child.Count == 3);
            foreach(var part in Root.Child) {
                if (!chest && part.Offset.y > 0 && part.Offset.x <= 0){
                    chest = true;
                    part.name = "Chest";
                    Part[1] = part;
                    Assert.IsTrue(part.Child.Count == 3);
                    bool leftCollar = false, rightCollar = false, neck = false;
                    foreach(var part2 in part.Child) {
                        if (!neck && part2.Offset.x == 0 ) {
                            neck = true;
                            part2.name = "Neck";
                            Part[2] = part2;
                            Assert.IsTrue(part2.Child.Count == 1);
                            part2.Child[0].name = "Head";
                            Part[3] = part2.Child[0];
                            Assert.IsTrue(part2.Child[0].Child.Count == 1);
                            Assert.IsTrue(part2.Child[0].Child[0].name == "End");
                        }
                        else if (!leftCollar && part2.Offset.x > 0){
                            leftCollar = true;
                            part2.name = "LeftCollar";
                            Part[4] = part2;
                            Assert.IsTrue(part2.Child.Count == 1);
                            part2.Child[0].name = "LeftUpArm";
                            Part[5] = part2.Child[0];
                            Assert.IsTrue(part2.Child[0].Child.Count == 1);
                            part2.Child[0].Child[0].name = "LeftLowArm";
                            Part[6] = part2.Child[0].Child[0];
                            Assert.IsTrue(part2.Child[0].Child[0].Child.Count == 1);
                            part2.Child[0].Child[0].Child[0].name = "LeftHand";
                            Part[7] = part2.Child[0].Child[0].Child[0];
                            Assert.IsTrue(part2.Child[0].Child[0].Child[0].Child.Count == 1);
                            Assert.IsTrue(part2.Child[0].Child[0].Child[0].Child[0].name == "End");
                        }
                        else if (!rightCollar && part2.Offset.x < 0){
                            rightCollar = true;
                            part2.name = "RightCollar";
                            Part[8] = part2;
                            Assert.IsTrue(part2.Child.Count == 1);
                            part2.Child[0].name = "RightUpArm";
                            Part[9] = part2.Child[0];
                            Assert.IsTrue(part2.Child[0].Child.Count == 1);
                            part2.Child[0].Child[0].name = "RightLowArm";
                            Part[10] = part2.Child[0].Child[0];
                            Assert.IsTrue(part2.Child[0].Child[0].Child.Count == 1);
                            part2.Child[0].Child[0].Child[0].name = "RightHand";
                            Part[11] = part2.Child[0].Child[0].Child[0];
                            Assert.IsTrue(part2.Child[0].Child[0].Child[0].Child.Count == 1);
                            Assert.IsTrue(part2.Child[0].Child[0].Child[0].Child[0].name == "End");
                        }
                        else{
                            Assert.IsTrue(false);
                        }
                    }
                }
                else if (!leftLeg && part.Offset.x > 0){
                    leftLeg = true;
                    part.name = "LeftUpLeg";
                    Part[12] = part;
                    Assert.IsTrue(part.Child.Count == 1);
                    part.Child[0].name = "LeftLowLeg";
                    Part[13] = part.Child[0];
                    Assert.IsTrue(part.Child[0].Child.Count == 1);
                    part.Child[0].Child[0].name = "LeftFoot";
                    Part[14] = part.Child[0].Child[0];
                    Assert.IsTrue(part.Child[0].Child[0].Child.Count == 1);
                    Assert.IsTrue(part.Child[0].Child[0].Child[0].name == "End");
                }
                else if (!rightLeg && part.Offset.x < 0){
                    rightLeg = true;
                    part.name = "RightUpLeg";
                    Part[15] = part;
                    Assert.IsTrue(part.Child.Count == 1);
                    part.Child[0].name = "RightLowLeg";
                    Part[16] = part.Child[0];
                    Assert.IsTrue(part.Child[0].Child.Count == 1);
                    part.Child[0].Child[0].name = "RightFoot";
                    Part[17] = part.Child[0].Child[0];
                    Assert.IsTrue(part.Child[0].Child[0].Child.Count == 1);
                    Assert.IsTrue(part.Child[0].Child[0].Child[0].name == "End");
                }
                else{
                    Debug.LogError(part.name + "沒有找到對應的部位");
                    Assert.IsTrue(false);
                }
            }
        }
        
         public void RenamePartCMU() {
            Part = new BVHPartObject[31];
            List<BVHPartObject> bfs = new List<BVHPartObject>();
            bfs.Add(Root);
            int queueIdx = 0;
            while (queueIdx < bfs.Count) {
                var cur = bfs[queueIdx++];
                if (cur.name == "End") continue;
                int idx = BVH.Utility.CMUMotion.GetPartIdxByNameCMU(cur.name);
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

        void Update()
        {
            if(Root && gameObject.activeSelf){
                ApplyFrame(Time.time / 5);
            }
        }

    }

}