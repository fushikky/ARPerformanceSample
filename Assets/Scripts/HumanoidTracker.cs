using System.IO;
using UnityEngine;
using VRM;

public class HumanoidTracker : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        ImportVRMAsync();
    }

    private void Update()
    {
        var origin = FindObjectOfType<BoneController>()?.GetComponent<Animator>();
        if (_animator == null || origin == null)
        {
            return;
        }

        var originalHandler = new HumanPoseHandler(origin.avatar, origin.transform);
        var targetHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);

        HumanPose humanPose = new HumanPose();
        originalHandler.GetHumanPose(ref humanPose);
        targetHandler.SetHumanPose(ref humanPose);

        _animator.rootPosition = origin.rootPosition;
        _animator.rootRotation = origin.rootRotation;
    }

    private void ImportVRMAsync()
    {
        //VRMファイルのパスを指定します
        var path = $"{Application.streamingAssetsPath}/MitchieM_Miku005.vrm";

        //ファイルをByte配列に読み込みます
        var bytes = File.ReadAllBytes(path);

        //VRMImporterContextがVRMを読み込む機能を提供します
        var context = new VRMImporterContext();

        // GLB形式でJSONを取得しParseします
        context.ParseGlb(bytes);

        // VRMのメタデータを取得
        var meta = context.ReadMeta(false); //引数をTrueに変えるとサムネイルも読み込みます

        //読み込めたかどうかログにモデル名を出力してみる
        Debug.LogFormat("meta: title:{0}", meta.Title);

        //非同期処理で読み込みます
        context.LoadAsync(_ => OnLoaded(context));
    }

    private void OnLoaded(VRMImporterContext context)
    {
        //読込が完了するとcontext.RootにモデルのGameObjectが入っています
        var root = context.Root;

        _animator = root.GetComponent<Animator>();
        // root.transform.position = new Vector3(0, -1, 1);
        root.transform.position = Vector3.zero;
        // root.transform.rotation = Quaternion.Euler(0, 180f, 0);
        root.transform.rotation = Quaternion.identity;
        _animator.applyRootMotion = true;

        //メッシュを表示します
        context.ShowMeshes();
    }
}
