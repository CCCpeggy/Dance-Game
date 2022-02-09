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
    public Button LeftHandBtn, RightHandBtn, LeftLegBtn, RightLegBtn;
    public Text ScoreText;
    public Text FirstText, SecondText, ThreeText;
    public VideoCapture VideoCapture;
    public GameObject InitPanel, LoadPanel;
    public Material NormalPointMaterial;
    public Material NormalLineMaterial;
    public Material ErrorLineMaterial;
    public Material CorrectLineMaterial;
    private Pose.Object refPose, recordPose, demoRefPose, demoRecPose, retargetedRefPose;

    void Start()
    {
        VideoBtn.onClick.AddListener(ChooseVideo);
        MotionBtn.onClick.AddListener(ChooseMotion);
        DoneBtn.onClick.AddListener(Done);
        LeftHandBtn.onClick.AddListener(delegate{SetAttentionPart(BindPart.Part.LeftHand);});
        RightHandBtn.onClick.AddListener(delegate{SetAttentionPart(BindPart.Part.RightHand);});
        LeftLegBtn.onClick.AddListener(delegate{SetAttentionPart(BindPart.Part.LeftLeg);});
        RightLegBtn.onClick.AddListener(delegate{SetAttentionPart(BindPart.Part.RightLeg);});
        LeftHandBtn.gameObject.SetActive(false);
        RightHandBtn.gameObject.SetActive(false);
        LeftLegBtn.gameObject.SetActive(false);
        RightLegBtn.gameObject.SetActive(false);
        InitPanel.SetActive(true);
        LoadPanel.SetActive(false);
        FirstText.gameObject.SetActive(false);
        SecondText.gameObject.SetActive(false);
        ThreeText.gameObject.SetActive(false);
        // var refPose = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true).GetComponent<Pose.Object>();
        // refPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        // refPose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        // refPose.transform.position = new Vector3(100, 0, 0);
        // SetLinesColor(refPose, NormalLineMaterial);
        // SetPointsColor(refPose, NormalPointMaterial);
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
        DoneBtn.interactable = false;
        VideoCapture.VideoPlayer.url = "file://" + VideoPathField.text;
        VideoCapture.VideoPlayer.Prepare();
        StartCoroutine(CheckVideoPlayer());
        InitPanel.SetActive(false);
        LoadPanel.SetActive(true);
    }

    void CreateRefPose()
    {
        var refPose = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true).GetComponent<Pose.Object>();
        refPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        refPose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        refPose.transform.position = new Vector3(3, 1, 0);
        // EstimateModel.refPose = refPose;
    }

    public void StartVideo() {
        VideoCapture.VideoPlayer.Play();
        LoadPanel.SetActive(false);
    }

    private void SetPartLinesColor(Pose.Object pose, BindPart.Part part, Material material, bool inversePart=false)
    {
        for (int i = 0; i < pose.Part.Length; i++)
        {
            if (pose.Part[i].Parent == null) continue; 
            int parentIdx = pose.Part[i].Parent.PartIdx;
            if (inversePart ^ BindPart.IsAttentionPart(part, (Pose.CMUPartIdx) parentIdx)) {
                var line = pose.Part[i].GetComponent<LineRenderer>();
                if (line) {
                    line.material = material;
                    line.startWidth = 0.5f;
                    line.startWidth = 0.3f;
                }
            }
        }
    }

    public void SetLinesColor(Pose.Object pose, Material material)
    {
        for (int i = 0; i < pose.Part.Length; i++)
        {
            var line = pose.Part[i].GetComponent<LineRenderer>();
            if (line) {
                line.material = material;
                line.startWidth = 0.5f;
                line.startWidth = 0.3f;
            }
        }
    }

    public void SetPointsColor(Pose.Object pose, Material material)
    {
        for (int i = 0; i < pose.Part.Length; i++)
            pose.Part[i].GetComponent<Renderer>().material = material;
    }

    public void SetRecordingObject(Pose.Object pose)
    {
        // pose.transform.rotation = Quaternion.Euler(0, -90, 0);
        pose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        pose.transform.position = new Vector3(200, 0, 0);
        SetLinesColor(pose, NormalLineMaterial);
        SetPointsColor(pose, NormalPointMaterial);
    }


    public void BindRefAndRealPose(Pose.Object _recordPose)
    {
        LeftHandBtn.gameObject.SetActive(true);
        RightHandBtn.gameObject.SetActive(true);
        LeftLegBtn.gameObject.SetActive(true);
        RightLegBtn.gameObject.SetActive(true);
        FirstText.gameObject.SetActive(true);
        SecondText.gameObject.SetActive(true);
        ThreeText.gameObject.SetActive(true);

        recordPose = _recordPose;
        recordPose.name = "錄製 motioin";
        recordPose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        recordPose.transform.position = new Vector3(300, 0, 0);
        SetLinesColor(recordPose, NormalLineMaterial);
        SetPointsColor(recordPose, NormalPointMaterial);


        var refObj = Pose.Object.CreatePoseObjByBVH(MotionPathField.text, true);
        refPose = refObj.GetComponent<Pose.Object>();
        refPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        new Pose.TimeWarping(recordPose, refPose).Do();

        SetAttentionPart(BindPart.Part.LeftLeg);
    }

    private void SetAttentionPart(BindPart.Part part) {
        BindPart bindPart = new BindPart(recordPose, refPose);
        bindPart.Set(part);
        if (retargetedRefPose != null) GameObject.Destroy(retargetedRefPose.gameObject);
        retargetedRefPose = bindPart.Get();
        retargetedRefPose.name = "標準 motion";
        retargetedRefPose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        double score = bindPart.GetGrade();
        ScoreText.gameObject.SetActive(true);
        ScoreText.text = "分數: " + score.ToString("0") + " 分";
        // retargetedRefPose.transform.rotation = Quaternion.Euler(0, -90, 0);
        retargetedRefPose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        retargetedRefPose.transform.position = new Vector3(100, 0, 0);
        SetLinesColor(retargetedRefPose, NormalLineMaterial);
        SetPointsColor(retargetedRefPose, NormalPointMaterial);

        if (demoRefPose != null) GameObject.Destroy(demoRefPose.gameObject);
        demoRefPose = retargetedRefPose.Clone();
        demoRefPose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        demoRefPose.transform.position = new Vector3(200, 0, 0);
        SetLinesColor(demoRefPose, NormalLineMaterial);
        SetPointsColor(demoRefPose, NormalPointMaterial);
        SetPartLinesColor(demoRefPose, part, CorrectLineMaterial);
        demoRefPose.name = "正解 motion";
        
        if (demoRecPose != null) GameObject.Destroy(demoRecPose.gameObject);
        demoRecPose = recordPose.Clone();
        demoRecPose.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        demoRecPose.transform.position = new Vector3(200, 0, 0);
        SetLinesColor(demoRecPose, NormalLineMaterial);
        SetPointsColor(demoRecPose, NormalPointMaterial);
        SetPartLinesColor(demoRecPose, part, ErrorLineMaterial);
        demoRecPose.name = "錯誤 motion";
    }
}
