using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace BVH {
    public class BVHMotion{
        private int frameCount;
        public int FrameCount{
            get{
                return frameCount;
            }
        }
        private float frameTime;
        public float FrameTime{
            get{
                return frameTime;
            }
        }
        public class Frame{
            public Vector3 Position = new Vector3();
            public Quaternion[] Rotation;
            public Frame(int count = 18) {
                Rotation = new Quaternion[count];
            }
            public Frame Clone() {
                Frame newFrame = new Frame();
                newFrame.Position = Position;
                newFrame.Rotation = Rotation.Clone() as Quaternion[];
                return newFrame;
            }
        }
        public List<Frame> motionData;
        public Frame TPose = null;
        public BVHObject BvhObj = null;
        public BVHMotion Clone(){
            BVHMotion bVHMotion = new BVHMotion();
            bVHMotion.frameCount = frameCount;
            bVHMotion.frameTime = frameTime;
            bVHMotion.BvhObj = BvhObj;
            bVHMotion.motionData = new List<Frame>();
            for(int i = 0; i < motionData.Count; i++)  
                bVHMotion.motionData.Add(motionData[i].Clone());
            return bVHMotion;
        }
        public static BVHMotion readMotion(ref IEnumerator<string> bvhDataIter, BVHObject obj, bool isTPoseType=false) {
            BVHMotion motion = new BVHMotion();
            motion.BvhObj = obj;
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Frames:");
            motion.frameCount = int.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Frame");
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Time:");
            motion.frameTime = float.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
            motion.motionData = new List<Frame>();
            for (int i = 0; i < motion.frameCount; i++) {
                Frame frame = (isTPoseType && i != 0) ? motion.TPose.Clone() : new Frame(obj.Part.Length);
                for (int j = 0; j < obj.Part.Length; j++) {
                    frame.Rotation[j] = Quaternion.Euler(0, 0, 0);
                }
                for (int j = 0; j < obj.ChannelDatas.Count; j++) {
                    float num = float.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
                    var partObj = obj.ChannelDatas[j].Item1;
                    var infoIdx = obj.ChannelDatas[j].Item2;
                    int partIdx = partObj.PartIdx;
                    if (partIdx == 0 && infoIdx >= 3) {
                        frame.Position[infoIdx - 3] += num;
                    }
                    else {
                        if (infoIdx == 0) frame.Rotation[partIdx] *= Quaternion.Euler(num, 0, 0);
                        else if (infoIdx == 1) frame.Rotation[partIdx] *= Quaternion.Euler(0, num, 0);
                        else if (infoIdx == 2) frame.Rotation[partIdx] *= Quaternion.Euler(0, 0, num);
                    }
                }
                if(isTPoseType && i == 0) motion.TPose = frame;
                else motion.motionData.Add(frame);
            }
            if (isTPoseType) {
                motion.frameCount--;
            }
            return motion;
        }

        public void ApplyFrame(float frameIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % frameCount;
            float alpha = frameIdx - previousFrameIdx;
            var Part = BvhObj.Part;
            for(int i = 0; i < Part.Length; i++){
                Quaternion previousRotValue = motionData[previousFrameIdx].Rotation[i];
                Quaternion nextRotValue = motionData[nextFrameIdx].Rotation[i];
                Quaternion thisRotValue = Utility.GetQuaternionAvg(previousRotValue, nextRotValue, alpha);
                Part[i].transform.localPosition = Part[i].Offset;
                Part[i].transform.localRotation = thisRotValue;
            }

            Vector3 previousPos = motionData[previousFrameIdx].Position;
            Vector3 nextPos = motionData[nextFrameIdx].Position;
            BvhObj.Root.transform.position += previousPos * (1 - alpha) + nextPos * alpha;
            BvhObj.UpdateLines();
        }
        public void ResetMotionInfo(int frameCount, float frameTime) {
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            if (this.motionData == null)
                this.motionData = new List<Frame>();
            else
                this.motionData.Clear();
        }
        public Vector3 getFramePosition(float frameIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % frameCount;
            Vector3 previous = motionData[previousFrameIdx].Position;
            Vector3 next = motionData[nextFrameIdx].Position;
            float alpha = frameIdx - previousFrameIdx;
            return previous * (1 - alpha) + next * alpha;
        }
        public Quaternion getFrameQuaternion(float frameIdx, int partIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % frameCount;
            Quaternion previous = motionData[previousFrameIdx].Rotation[partIdx];
            Quaternion next = motionData[nextFrameIdx].Rotation[partIdx];
            float alpha = frameIdx - previousFrameIdx;
            var angle = Utility.GetQuaternionAvg(previous, next, alpha);
            return angle;
        }
    }
}