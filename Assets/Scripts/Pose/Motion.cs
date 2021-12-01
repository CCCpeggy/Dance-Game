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
            public Vector3[] JointPosition;
            public Quaternion[] JointRotation;
            public enum JointType {
                Position, Rotation
            }
            public JointType type;
            public Frame(int count = 18, JointType type = JointType.Rotation)
            {
                this.type = type;
                switch(type) {
                    case JointType.Position:
                        JointPosition = new Vector3[count];
                        break;
                    case JointType.Rotation:
                        JointRotation = new Quaternion[count];
                        break;
                }
            }
            public Frame Clone() {
                Frame newFrame = new Frame();
                newFrame.Position = Position;
                newFrame.type = type;
                switch (type)
                {
                    case JointType.Position:
                        newFrame.JointPosition = JointPosition.Clone() as Vector3[];
                        break;
                    case JointType.Rotation:
                        newFrame.JointRotation = JointRotation.Clone() as Quaternion[];
                        break;
                }
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
        public static Motion Create(Object belongObj){
            Motion poseMotion = new Motion();
            poseMotion.FrameCount = 0;
            poseMotion.FrameTime = 0;
            poseMotion.PoseObj = belongObj;
            poseMotion.MotionData = new List<Frame>();
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
            for (int i = 0; i < motion.FrameCount; i++)
            {
                Frame frame;
                if (isTPoseType && i != 0)
                {
                    frame = motion.MotionData[0].Clone();
                }
                else
                {
                    frame = new Frame(obj.Part.Length, Frame.JointType.Rotation);
                    for (int j = 0; j < obj.Part.Length; j++)
                    {
                        frame.JointRotation[j] = Quaternion.Euler(0, 0, 0);
                    }
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
                        if (infoIdx == 0) frame.JointRotation[partIdx] *= Quaternion.Euler(num, 0, 0);
                        else if (infoIdx == 1) frame.JointRotation[partIdx] *= Quaternion.Euler(0, num, 0);
                        else if (infoIdx == 2) frame.JointRotation[partIdx] *= Quaternion.Euler(0, 0, num);
                    }
                }
                motion.MotionData.Add(frame);
            }
            if (isTPoseType) {
                motion.FrameCount--;
                motion.TPose = motion.MotionData[0];
                motion.MotionData.Remove(motion.MotionData[0]);
            }
            return motion;
        }

        public void ApplyFrame(float frameIdx) {
            int previousFrameIdx = (int)frameIdx;
            int nextFrameIdx = (previousFrameIdx + 1) % FrameCount;
            float alpha = frameIdx - previousFrameIdx;
            var Part = PoseObj.Part;
            var previousMotionData = MotionData[previousFrameIdx];
            var nextMotionData = MotionData[nextFrameIdx];
            if (previousMotionData.type != nextMotionData.type) {
                Debug.LogError("frame 型態不相符");
                return;
            }
            for (int i = 0; i < Part.Length; i++)
            {
                switch (previousMotionData.type) {
                    case Frame.JointType.Rotation:
                        Quaternion previousRotValue = previousMotionData.JointRotation[i];
                        Quaternion nextRotValue = nextMotionData.JointRotation[i];
                        Quaternion thisRotValue = Utility.GetQuaternionAvg(previousRotValue, nextRotValue, alpha);
                        Part[i].transform.localPosition = Part[i].Offset;
                        Part[i].transform.localRotation = thisRotValue;
                        break;
                    case Frame.JointType.Position:
                        Vector3 previousPosValue = previousMotionData.JointPosition[i];
                        Vector3 nextPosValue = nextMotionData.JointPosition[i];
                        Vector3 thisPosValue = Utility.GetVectorAvg(previousPosValue, nextPosValue, alpha);
                        // Debug.Log(thisPosValue);
                        Part[i].transform.localPosition = thisPosValue;
                        break;
                }
            }

            //Vector3 previousPos = MotionData[previousFrameIdx].Position;
            //Vector3 nextPos = MotionData[nextFrameIdx].Position;
            //PoseObj.Root.transform.position += previousPos * (1 - alpha) + nextPos * alpha;
            // PoseObj.UpdateLines();
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
            Quaternion previous = MotionData[previousFrameIdx].JointRotation[partIdx];
            Quaternion next = MotionData[nextFrameIdx].JointRotation[partIdx];
            float alpha = frameIdx - previousFrameIdx;
            var angle = Utility.GetQuaternionAvg(previous, next, alpha);
            return angle;
        }
        public Frame getFrame(float frameIdx) {
            Frame frame = new Frame(PoseObj.Part.Length);
            frame.Position = getFramePosition(frameIdx);
            for (int i = 0; i < PoseObj.Part.Length; i++) {
                frame.JointRotation[i] = getFrameQuaternion(frameIdx, i);
            }
            return frame;
        }
        // public void Record() {
        //     var Part = PoseObj.Part;
        //     if (MotionData == null) {
        //         MotionData = new List<Frame>();
        //     }
        //     if (MotionData.Count == 0) {
        //         for (int i = 0; i < Part.Length; i++)
        //         {
        //             Part[i].Offset = Part[i].transform.localPosition;
        //         }
        //         FrameTime = Time.deltaTime;
        //         FrameCount = 0;
        //     }
        //     Frame frame = new Frame(Part.Length);
        //     frame.Position = PoseObj.Root.transform.localPosition;
        //     for (int i = 0; i < Part.Length; i++)
        //     {
        //         frame.Rotation[i] = Part[i].transform.localRotation;
        //         Vector3 oldVec = Part[i].transform.localRotation
        //         Quaternion.FromToRotation()
        //         Debug.Log(frame.Rotation[i]);
        //     }
        //     MotionData.Add(frame);
        //     FrameCount++;
        // }
        public void Record() {
            PoseObj.Status = Object.StatusType.Recording;
            var Part = PoseObj.Part;
            if (MotionData == null) {
                MotionData = new List<Frame>();
            }
            Frame frame = new Frame(Part.Length, Frame.JointType.Position);
            if (MotionData.Count == 0) {
                FrameTime = Time.deltaTime;
                FrameCount = 0;
            }
            // frame.Position = PoseObj.Root.transform.localPosition;
            for (int i = 0; i < Part.Length; i++)
            {
                frame.JointPosition[i] = Part[i].transform.localPosition;
            }
            MotionData.Add(frame);
            FrameCount++;
        }
    }
}