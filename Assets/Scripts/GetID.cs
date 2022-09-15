using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.VFX;
using UnityEngine.Video;
using Random = System.Random;


public struct Info
{
    public int ID;

    public float Depth;

    public Vector3 Pos;

   
}

public class GetID : MonoBehaviour
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

    private List<Info> infos = new List<Info>();

    private ComputeBuffer argsBuffer;

    private RenderTexture _renderTexture;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private int _screenWidth;
    private int _screenHeight;


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

        ComputeBuffer.SetData(infos);

        ComputeShader.SetBuffer(0, "ComputeBuffer", ComputeBuffer);

        ComputeShader.SetTexture(0, "RenderTexture", _renderTexture);
        ComputeShader.SetTexture(1, "RenderTexture", _renderTexture);

        // indirect args
        uint numIndices = 0;
        args[0] = numIndices;
        args[1] = (uint)datas.Length;
        argsBuffer.SetData(args);

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
        ComputeShader.SetVector("randPos", pos);


        ComputeShader.Dispatch(0, _screenWidth / 8, _screenHeight / 8, 1);


        Info[] datas = new Info[1];

       
        ComputeBuffer.GetData(datas);

        Info info = datas[0];

        if (info.ID == -1) return;

        Debug.Log("value is " + info.ID + "  图片名字是 " + PictureList[info.ID].name + "  位置是：" + datas[0].Pos);

        //Debug.Log("value is " + info.ID + "  图片名字是 " + PictureList[info.ID].name + "  位置是：" + datas[0].Pos);

        float depth = info.Depth;

        float camDepth = depth- MainCamera.transform.position.z ;//得到与相机的距离

        Vector3 worldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDepth));

        Debug.Log("得到的世界位置是："+ worldPos);

        //TargetGameObject.transform.position = worldPos;

        // MainCamera.transform.DOMove(worldPos + new Vector3(0f, 0f, -2f), 1f);


    }

    private Vector3 _preMousePos;
    private Vector3 _delta;
    private float _clikcTimeTemp = 0.0f;

    private bool _isButtonUp = false;
    public float MoveSpeed = 1f;
    /// <summary>
    /// 移动主相机
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
            if (_clikcTimeTemp <= 0.2f)//触发点击事件
            {
                Debug.Log("点击的位置是： " + Input.mousePosition);
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
    /// 检测相机的范围是否越界
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

            Vfx.SetVector3("MoveDelta",pos);


            _clikcTimeTemp += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_clikcTimeTemp <= 0.2f)//触发点击事件
            {
                Debug.Log("点击的位置是： " + Input.mousePosition);
                float clickWidth = _preMousePos.x;
                float clickHeight = _preMousePos.y;

                DisPatch(new Vector2(clickWidth, clickHeight));
            }
        }


      
    }
    /// <summary>
    /// 给RT赋予纯色 
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
        HandleInput();
        MoveCam();
        CheckCamRange();
        //MovePicture();
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
