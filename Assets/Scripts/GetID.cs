using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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

        Debug.Log("value is " + info.ID + "  ͼƬ������ " + PictureList[info.ID].name + "  λ���ǣ�" + datas[0].Pos);

        //Debug.Log("value is " + info.ID + "  ͼƬ������ " + PictureList[info.ID].name + "  λ���ǣ�" + datas[0].Pos);

        float depth = info.Depth;

        float camDepth = depth- MainCamera.transform.position.z ;//�õ�������ľ���

        Vector3 worldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDepth));

        Debug.Log("�õ�������λ���ǣ�"+ worldPos);

        //TargetGameObject.transform.position = worldPos;

         MainCamera.transform.DOMove(worldPos + new Vector3(0f, 0f, -2f), 1f);


    }

    private Vector3 _preMousePos;
    private Vector3 _delta;
    private float _clikcTimeTemp = 0.0f;
    public float MoveSpeed = 1f;
    /// <summary>
    /// �ƶ������
    /// </summary>
    public void MoveMainCam()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _preMousePos = Input.mousePosition;
            _clikcTimeTemp = 0f;
        }

        if (Input.GetMouseButton(0))
        {

            Vector3 minPos = MainCamera.transform.position;

            _delta = _preMousePos - Input.mousePosition;
            _preMousePos = Input.mousePosition;

            Vector3 pos = MoveSpeed * _delta;

            MainCamera.transform.position = pos + minPos;
            

            _clikcTimeTemp += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_clikcTimeTemp <= 0.2f)//��������¼�
            {
                Debug.Log("�����λ���ǣ� " + Input.mousePosition);
                float clickWidth = _preMousePos.x;
                float clickHeight = _preMousePos.y;

                DisPatch(new Vector2(clickWidth, clickHeight));
            }
        }


        ColCamera.transform.position = MainCamera.transform.position + new Vector3(0f, 500f, 0f);
    }

    /// <summary>
    /// ��RT���贿ɫ
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
        MoveMainCam();

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
