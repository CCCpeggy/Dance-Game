using UnityEngine;

class BindPart {
    public enum Part{
        LeftHand,
        RightHand,
        LeftLeg,
        RightLeg
    }
    public Part BlendPart = Part.LeftHand;

    Pose.Object basicObj; // 錄影的
    Pose.Object refObj; // 參考的

    public BindPart(Pose.Object basicObj, Pose.Object refObj) {
        this.basicObj = basicObj;
        this.refObj = refObj;
        if (basicObj.Motion.MotionData.Count != refObj.Motion.MotionData.Count){
            Debug.LogError("MotionData 筆數不一致");
        }
    }

    public void Set(Part blendPart) {
        BlendPart = blendPart;
    }

    public Pose.Object Get() {
        Pose.Object newObj = basicObj.Clone();
        for (int j = 0; j < newObj.Motion.MotionData.Count; j++) {
            if (refObj.Motion.MotionData[j].type == Pose.Motion.Frame.JointType.Position) {
                Debug.LogError("Frame 型態錯誤");
                continue;
            }
            refObj.Motion.ApplyFrame(refObj.Motion.MotionData[j]);
            newObj.Motion.ApplyFrame(newObj.Motion.MotionData[j]);
            Vector3[] targetPos = new Vector3[refObj.Part.Length];
            for (int i = 0; i < refObj.Part.Length; i++) {
                targetPos[i] = new Vector3();
                if (refObj.Part[i].Parent) {
                    targetPos[i] = refObj.Part[i].transform.position - refObj.Part[i].Parent.transform.position;
                }
            }
            for (int i = 0; i < refObj.Part.Length; i++) {
                if (newObj.Part[i].Parent == null) continue; 
                int parentIdx = newObj.Part[i].Parent.PartIdx;
                if (needReplace((Pose.CMUPartIdx) parentIdx)) {
                    var fromPos = newObj.Part[i].Offset;
                    var toPos = targetPos[i];
                    newObj.Part[i].transform.localPosition = newObj.Part[i].Offset;
                    newObj.Part[i].Parent.transform.rotation = Quaternion.FromToRotation(fromPos, toPos);
                    newObj.Motion.MotionData[j].JointRotation[parentIdx] = newObj.Part[i].Parent.transform.localRotation;
                }
            }
            newObj.Motion.MotionData[j].type = refObj.Motion.MotionData[j].type;
        }
        return newObj;
    }
    public double GetGrade() {
        double diffRot = 0;
        int partCount = 0;
        for (int frameIdx = 0; frameIdx < basicObj.Motion.MotionData.Count; frameIdx++) {
            var basicFrame = basicObj.Motion.MotionData[frameIdx];
            var refFrame = refObj.Motion.MotionData[frameIdx];
            if (basicFrame.type != Pose.Motion.Frame.JointType.Rotation ||
            refFrame.type != Pose.Motion.Frame.JointType.Rotation){
                Debug.LogError("Frame 型態錯誤");
                return -1;
            }
            refObj.Motion.ApplyFrame(refFrame);
            basicObj.Motion.ApplyFrame(basicFrame);
            for (int i = 0; i < 31; i++) {
                Pose.CMUPartIdx partIdx = (Pose.CMUPartIdx) i;
                if (needReplace(partIdx)) {
                    var basicRotation = basicObj.transform.localRotation;
                    var targetLocalPos = refObj.Part[i].transform.position - refObj.Part[i].Parent.transform.position;
                    var refRotation = Quaternion.FromToRotation(refObj.Part[i].Offset, targetLocalPos);
                    diffRot += (180 - Quaternion.Angle(basicRotation, refRotation)) / 180;
                    partCount++;
                } 
            }
        }
        return diffRot / partCount * 100;
    }

    private bool needReplace(Pose.CMUPartIdx partIdx) {
        switch (BlendPart)
        {
            case Part.LeftHand:
                switch (partIdx)
                {
                    case Pose.CMUPartIdx.Left_Collar:
                    case Pose.CMUPartIdx.Left_Shoulder:
                    case Pose.CMUPartIdx.Left_Forearm:
                    case Pose.CMUPartIdx.Left_Hand:
                    case Pose.CMUPartIdx.LeftFingerBase:
                    case Pose.CMUPartIdx.LFingers:
                    case Pose.CMUPartIdx.lThumb1:
                        return true;
                }
                return false;
            case Part.RightHand:
                switch (partIdx)
                {
                    case Pose.CMUPartIdx.Right_Collar:
                    case Pose.CMUPartIdx.Right_Shoulder:
                    case Pose.CMUPartIdx.Right_Forearm:
                    case Pose.CMUPartIdx.Right_Hand:
                    case Pose.CMUPartIdx.RightFingerBase:
                    case Pose.CMUPartIdx.RFingers:
                    case Pose.CMUPartIdx.rThumb1:
                        return true;
                }
                return false;
            case Part.LeftLeg:
                switch (partIdx)
                {
                    case Pose.CMUPartIdx.lButtock:
                    case Pose.CMUPartIdx.Left_Thigh:
                    case Pose.CMUPartIdx.Left_Shin:
                    case Pose.CMUPartIdx.Left_Foot:
                    case Pose.CMUPartIdx.lToe:
                        return true;
                }
                return false;
            case Part.RightLeg:
                switch (partIdx)
                {
                    case Pose.CMUPartIdx.rButtock:
                    case Pose.CMUPartIdx.Right_Thigh:
                    case Pose.CMUPartIdx.Right_Shin:
                    case Pose.CMUPartIdx.Right_Foot:
                    case Pose.CMUPartIdx.rToe:
                        return true;
                }
                return false;
        }
        return false;
    }
}