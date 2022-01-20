using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SFB;

public class Canvas : MonoBehaviour
{
    public Button VideoBtn;
    public InputField VideoPathField;
    public Button MotionBtn;
    public InputField MotionPathField;
    public Button DoneBtn;
    public VideoCapture VideoCapture;
    public VideoPlayer VideoPlayer;
    bool bideoCaptureIsInit = false;
    
    void Start()
    {
        VideoBtn.onClick.AddListener(ChooseVideo);
        MotionBtn.onClick.AddListener(ChooseMotion);
        DoneBtn.onClick.AddListener(Done);
        VideoPlayer.url = "file://" + "D:/workplace/3D遊戲/P2/Dance Game/Assets/Videos/test.mp4";
        VideoPlayer.Prepare();
        

    }

    void Update()
    {
        if (!bideoCaptureIsInit && VideoPlayer.isPrepared) {
            bideoCaptureIsInit = true;
            VideoCapture.Init(448, 448);
        }
    }

    void ChooseVideo() {
        var path = StandaloneFileBrowser.OpenFilePanel("Open Video", ".",  new []{new ExtensionFilter("MP4", "mp4")}, false)[0];
        VideoPathField.text = path;
    }

    void ChooseMotion() {
        var path = StandaloneFileBrowser.OpenFilePanel("Open Motion File", ".",  new []{new ExtensionFilter("BVH", "bvh")}, false)[0];
        MotionPathField.text = path;
    }

    void Done() {
        // var refMotion = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true);
        VideoPlayer.url = "file://" + VideoPathField.text;
    }
}
