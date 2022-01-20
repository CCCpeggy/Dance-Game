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
    }

    IEnumerator CheckVideoPlayer()
    {
        while(!VideoPlayer.isPrepared) yield return null;
        VideoCapture.Init(448, 448);
        VideoCapture.VideoPlayer.Pause();
    }


    void Update()
    {
        DoneBtn.interactable = !string.IsNullOrEmpty(VideoPathField.text);
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
        VideoPlayer.url = "file://" + VideoPathField.text;
        VideoPlayer.Prepare();
        StartCoroutine(CheckVideoPlayer());
        // var refMotion = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true);
    }
}
