using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace Pose {
    public class Motion{
        public int FrameCount;
        public float FrameTime;
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
        public List<Frame> MotionData;
        public Frame TPose = null;
        public Object PoseObj = null;
        public Motion Clone(Object belongObj){
            Motion poseMotion = new Motion();
            poseMotion.FrameCount = FrameCount;
            poseMotion.FrameTime = FrameTime;
            poseMotion.PoseObj = belongObj;
            poseMotion.MotionData = new List<Frame>();
            for(int i = 0; i < MotionData.Count; i++)  
                poseMotion.MotionData.Add(MotionData[i].Clone());
            return poseMotion;
        }
        public static Motion readMotion(ref IEnumerator<string> bvhDataIter, Object obj, bool isTPoseType=false) {
            Motion motion = new Motion();
            motion.PoseObj = obj;
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Frames:");
            motion.FrameCount = int.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Frame");
            Utility.IterData.CheckAndNext(ref bvhDataIter, "Time:");
            motion.FrameTime = float.Parse(Utility.IterData.GetAndNext(ref bvhDataIter));
            motion.MotionData = new List<Frame>();
            for (int i = 0; i < motion.FrameCount; i++) {
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
                else motion.MotionData.Add(frame);
            }
            if (isTPoseType) {
                motion.FrameCount--;
            }
            return motion;
        }

        public void ApplyFrame(float frameIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % FrameCount;
            float alpha = frameIdx - previousFrameIdx;
            var Part = PoseObj.Part;
            for(int i = 0; i < Part.Length; i++){
                Quaternion previousRotValue = MotionData[previousFrameIdx].Rotation[i];
                Quaternion nextRotValue = MotionData[nextFrameIdx].Rotation[i];
                Quaternion thisRotValue = Utility.GetQuaternionAvg(previousRotValue, nextRotValue, alpha);
                Part[i].transform.localPosition = Part[i].Offset;
                Part[i].transform.localRotation = thisRotValue;
            }

            //Vector3 previousPos = MotionData[previousFrameIdx].Position;
            //Vector3 nextPos = MotionData[nextFrameIdx].Position;
            //PoseObj.Root.transform.position += previousPos * (1 - alpha) + nextPos * alpha;
            PoseObj.UpdateLines();
        }
        public void ResetMotionInfo(int frameCount, float frameTime) {
            this.FrameCount = frameCount;
            this.FrameTime = frameTime;
            if (this.MotionData == null)
                this.MotionData = new List<Frame>();
            else
                this.MotionData.Clear();
        }
        public Vector3 getFramePosition(float frameIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % FrameCount;
            Vector3 previous = MotionData[previousFrameIdx].Position;
            Vector3 next = MotionData[nextFrameIdx].Position;
            float alpha = frameIdx - previousFrameIdx;
            return previous * (1 - alpha) + next * alpha;
        }
        public Quaternion getFrameQuaternion(float frameIdx, int partIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % FrameCount;
            Quaternion previous = MotionData[previousFrameIdx].Rotation[partIdx];
            Quaternion next = MotionData[nextFrameIdx].Rotation[partIdx];
            float alpha = frameIdx - previousFrameIdx;
            var angle = Utility.GetQuaternionAvg(previous, next, alpha);
            return angle;
        }
        public Frame getFrame(float frameIdx) {
            Frame frame = new Frame(PoseObj.Part.Length);
            frame.Position = getFramePosition(frameIdx);
            for (int i = 0; i < PoseObj.Part.Length; i++) {
                frame.Rotation[i] = getFrameQuaternion(frameIdx, i);
            }
            return frame;
        }
    }
}