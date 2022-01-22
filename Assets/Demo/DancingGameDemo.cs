using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SFB;

public class DancingGameDemo : MonoBehaviour
{
    public Button VideoBtn;
    public InputField VideoPathField;
    public Button MotionBtn;
    public InputField MotionPathField;
    public Button DoneBtn;
    public Text ScoreText;
    public VideoCapture VideoCapture;
    public Material NormalPointMaterial;
    public Material NormalLineMaterial;
    // public EstimateModel EstimateModel;

    void Start()
    {
        VideoBtn.onClick.AddListener(ChooseVideo);
        MotionBtn.onClick.AddListener(ChooseMotion);
        DoneBtn.onClick.AddListener(Done);
    }

    IEnumerator CheckVideoPlayer()
    {
        while (!VideoCapture.VideoPlayer.isPrepared) yield return null;
        VideoCapture.Init(448, 448);
        VideoCapture.VideoPlayer.Pause();
    }


    void Update()
    {
        DoneBtn.interactable = !string.IsNullOrEmpty(VideoPathField.text) && !string.IsNullOrEmpty(MotionPathField.text);
    }

    void ChooseVideo()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Open Video", ".", new[] { new ExtensionFilter("MP4", "mp4") }, false)[0];
        VideoPathField.text = path;
    }

    void ChooseMotion()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Open Motion File", ".", new[] { new ExtensionFilter("BVH", "bvh") }, false)[0];
        MotionPathField.text = path;
    }

    void Done()
    {
        VideoPathField.interactable = MotionPathField.interactable = VideoBtn.interactable = MotionBtn.interactable = false;
        VideoCapture.VideoPlayer.url = "file://" + VideoPathField.text;
        VideoCapture.VideoPlayer.Prepare();
        StartCoroutine(CheckVideoPlayer());

    }

    void CreateRefPose()
    {
        var refPose = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true).GetComponent<Pose.Object>();
        refPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        refPose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        refPose.transform.position = new Vector3(3, 1, 0);
        // EstimateModel.refPose = refPose;
    }

    public void SetLinesColor(Pose.Object pose, Material material)
    {
        for (int i = 0; i < pose.Part.Length; i++)
        {
            var line = pose.Part[i].GetComponent<LineRenderer>();
            if (line) line.material = material;
        }
    }

    public void SetPointsColor(Pose.Object pose, Material material)
    {
        for (int i = 0; i < pose.Part.Length; i++)
            pose.Part[i].GetComponent<Renderer>().material = material;
    }

    public void BindRefAndRealPose(Pose.Object cmuPose)
    {
        var refObj = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true);
        var refPose = refObj.GetComponent<Pose.Object>();
        refPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        refPose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        refPose.transform.position = new Vector3(3, 1, 0);
        SetLinesColor(refPose, NormalLineMaterial);
        SetPointsColor(refPose, NormalPointMaterial);

        new Pose.TimeWarping(cmuPose, refPose).Do();
        BindPart bindPart = new BindPart(cmuPose, refPose);
        bindPart.Set(BindPart.Part.LeftLeg);
        var retargetedRefPose = bindPart.Get();
        retargetedRefPose.name = "標準 motion";
        retargetedRefPose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        double score = bindPart.GetGrade();
        ScoreText.gameObject.SetActive(true);
        ScoreText.text = "分數: " + score.ToString("0") + " 分";
        // GameObject.Destroy(refPose.gameObject);
    }
}
