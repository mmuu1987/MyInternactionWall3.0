using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Video;
using Random = System.Random;




public class InternalactionManager : MonoBehaviour
{

    public Camera MainCamera;

    public Camera ColCamera;

    public ComputeShader ComputeShader;

    public ComputeBuffer ComputeBuffer;

    public Color Color;

    public List<Texture2D> PictureList = new List<Texture2D>();

    public GameObject TargetGameObject;

    public VisualEffect Vfx;

    public Vector4 Rect;

    public VisualEffect CreatImgageLeft;

    public VisualEffect CreatImageRigth;

    public Texture2D TexSize;

    public Button LeftExit;

    public Button RightExit;

    private List<Info> infos = new List<Info>();

    private ComputeBuffer argsBuffer;

    private RenderTexture _renderTexture;


    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private int _screenWidth;
    private int _screenHeight;

    private Dictionary<int, Vector2> _dicSize = new Dictionary<int, Vector2>();
    private void Init()
    {

        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0, GraphicsFormat.R32G32B32A32_SFloat);
        _renderTexture.enableRandomWrite = true;

        ColCamera.targetTexture = _renderTexture;


        int stride = Marshal.SizeOf(typeof(Info));
        //Debug.Log("stride byte size is " + stride);  
        ComputeBuffer = new ComputeBuffer(1, stride);//16

        Info[] datas = new Info[ComputeBuffer.count];

        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);

        // infos.Add(new Info());

        ComputeBuffer.SetData(infos);

        ComputeShader.SetBuffer(0, "ComputeBuffer", ComputeBuffer);

        ComputeShader.SetTexture(0, "RenderTexture", _renderTexture);
        ComputeShader.SetTexture(1, "RenderTexture", _renderTexture);

        // indirect args  
        uint numIndices = 0;
        args[0] = numIndices;
        args[1] = (uint)datas.Length;
        argsBuffer.SetData(args);

        LoadTexSizes();

        LeftExit.onClick.AddListener((() =>
        {
            if (CreatImgageLeft.enabled)
            {
                CreatImgageLeft.SetBool("IsExit", true);

            }
        }));

        RightExit.onClick.AddListener((() =>
        {
            if (CreatImageRigth.enabled)
            {
                CreatImageRigth.SetBool("IsExit", true);
                RightExit.enabled = false;
            }
        }));
    }

    public void LoadTexSizes()
    {


        for (int i = 0; i < TexSize.width; i++)
        {

            Color col = TexSize.GetPixel(i, 1, 0);
            Vector2 size = new Vector2(col.r, col.g) * 512f;

            _dicSize.Add(i, size);
        }
    }
    public void DisPatch()
    {

        Vector2 randPos = new Vector2(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height));


        ComputeShader.SetVector("randPos", randPos);


        ComputeShader.Dispatch(0, _screenWidth / 8, _screenHeight / 8, 1);


        Info[] datas = new Info[1];
        ComputeBuffer.GetData(datas);

        Debug.Log("value is " + datas[0].ID);


    }

    public void DisPatch(Vector2 pos)
    {



        if (pos.x <= 1920f)
        {
            if (LeftExit.enabled)
            {
                return;
            }
        }

        if (pos.x > 1920f)
        {
            if (RightExit.enabled)
            {
                return;
            }
        }
        ComputeShader.SetVector("randPos", pos);


        ComputeShader.Dispatch(0, _screenWidth / 8, _screenHeight / 8, 1);


        Info[] datas = new Info[1];


        ComputeBuffer.GetData(datas);

        Info info = datas[0];

        if (info.ID == -1) return;



        if (info.ID >= 999)
        {

            Debug.Log("超过范围" + info.ID);
            return;
        }

        Debug.Log("value is " + info.ID + "  图片名字 " + PictureList[info.ID].name + "  点击位置" + datas[0].Pos);



        float depth = info.Depth;

        float camDepth = depth - MainCamera.transform.position.z;//

        Vector3 worldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDepth));

        Debug.Log("世界坐标位置" + worldPos);

        VisualEffect temp = null;

        float xPos = 0f;

        float dir = 1;

        if (Input.mousePosition.x < 1920f)
        {
            temp = CreatImgageLeft;
            xPos = -350f;
            dir = -1f;
        }
        else
        {
            temp = CreatImageRigth;
            xPos = 350f;
            dir = 1f;
        }

        temp.enabled = false;

        temp.SetBool("IsExit", false);

        temp.SetVector3("ClickPos", worldPos);

        temp.SetFloat("Dir", dir);

        Vector2 size = GlobalSetting.ScaleImageSize(new Vector2(PictureList[info.ID].width, PictureList[info.ID].height), new Vector2(512f, 512f));

        Debug.Log("从图中获取的尺寸是：" + _dicSize[info.ID] + " 从资源中获取的尺寸是 " + size);



        temp.SetVector3("PictureSize", _dicSize[info.ID]);

        temp.SetVector3("PictureCenter", new Vector3(xPos, 0f, 600f));

        temp.SetInt("PictureID", info.ID);

        temp.enabled = true;

        //TargetGameObject.transform.position = worldPos;

        // MainCamera.transform.DOMove(worldPos + new Vector3(0f, 0f, -2f), 1f);


    }

    private Vector3 _preMousePos;
    private Vector3 _delta;
    private float _clikcTimeTemp = 0.0f;

    private bool _isButtonUp = false;
    public float MoveSpeed = 1f;
    /// <summary>
    /// 
    /// </summary>
    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _preMousePos = Input.mousePosition;
            _clikcTimeTemp = 0f;
            _isButtonUp = false;
        }

        if (Input.GetMouseButton(0))
        {
            _delta = _preMousePos - Input.mousePosition;
            _preMousePos = Input.mousePosition;

            _clikcTimeTemp += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isButtonUp = true;
            if (_clikcTimeTemp <= 0.2f)//
            {
                Debug.Log("" + Input.mousePosition);
                float clickWidth = _preMousePos.x;
                float clickHeight = _preMousePos.y;

                DisPatch(new Vector2(clickWidth, clickHeight));
            }
        }



    }

    public void MoveCam()
    {

        Vector3 minPos = MainCamera.transform.position;

        Vector3 pos;
        if (_isButtonUp)
            pos = MoveSpeed * _delta * 0.1f;
        else
            pos = MoveSpeed * _delta;

        MainCamera.transform.position = pos + minPos;

        ColCamera.transform.position = MainCamera.transform.position + new Vector3(0f, 500f, 0f);

    }

    /// <summary>
    /// 
    /// </summary>
    public void CheckCamRange()
    {
        Vector3 pos = MainCamera.transform.position;

        if (pos.x <= Rect.x || pos.x > Rect.y || pos.y <= Rect.z || pos.y > Rect.w)
        {

            _delta = -1 * _delta;
        }
    }
    public void MovePicture()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _preMousePos = Input.mousePosition;
            _clikcTimeTemp = 0f;
        }

        if (Input.GetMouseButton(0))
        {



            _delta = _preMousePos - Input.mousePosition;
            _preMousePos = Input.mousePosition;

            Vector3 pos = MoveSpeed * _delta;

            Vfx.SetVector3("MoveDelta", pos);


            _clikcTimeTemp += Time.deltaTime;
        }
        //
        if (Input.GetMouseButtonUp(0))
        {
            if (_clikcTimeTemp <= 0.2f)
            {
                Debug.Log("点击的屏幕坐标 " + Input.mousePosition);
                float clickWidth = _preMousePos.x;
                float clickHeight = _preMousePos.y;



                DisPatch(new Vector2(clickWidth, clickHeight));
            }
        }



    }
    /// <summary>
    /// 
    /// </summary>
    public void DrawRt()
    {
        ComputeShader.SetVector("Col", Color);

        ComputeShader.Dispatch(1, _screenWidth / 8, _screenHeight / 8, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //HandleInput();
        //MoveCam();
        CheckCamRange();
        MovePicture();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
            MainCamera.transform.DOMove(new Vector3(0f, 0f, 0f), 1f);
        }

        //if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "test"))
        //{
        //    DrawRt(); 
        //}
    }
#endif
}