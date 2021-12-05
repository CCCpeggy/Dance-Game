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
        for (int i = 0; i < 31; i++) {
            Pose.CMUPartIdx partIdx = (Pose.CMUPartIdx) i;
            if (needReplace(partIdx)) {
                for (int j = 0; j < newObj.Motion.MotionData.Count; j++) {
                    switch (newObj.Motion.MotionData[j].type)
                    {
                        case Pose.Motion.Frame.JointType.Position:
                            newObj.Motion.MotionData[j].JointPosition[i] = refObj.Motion.MotionData[j].JointPosition[i];
                            break;
                        case Pose.Motion.Frame.JointType.Rotation:
                            newObj.Motion.MotionData[j].JointRotation[i] = refObj.Motion.MotionData[j].JointRotation[i];
                            break;
                    }
                }
            } 
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
            for (int i = 0; i < 31; i++) {
                Pose.CMUPartIdx partIdx = (Pose.CMUPartIdx) i;
                if (needReplace(partIdx)) {
                    var basicRotation = basicFrame.JointRotation[i];
                    var refRotation = refFrame.JointRotation[i];
                    diffRot += Quaternion.Angle(basicRotation, refRotation) / 180;
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