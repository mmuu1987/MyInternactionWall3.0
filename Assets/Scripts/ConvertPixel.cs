using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 把quad 的在3d空间的size转为屏幕尺寸,目前用transform.localscale代表像素尺寸
/// 默认Z轴的位置为0
/// </summary>
public class ConvertPixel : MonoBehaviour
{
    public Transform CacheTransform { get; private set; }

    private bool _isInitEnd = false;

    private Material _curMaterial;

    

    private MaterialPropertyBlock _materialPropertyBlock;

    /// <summary>
    /// 初始化的时候的原本的位置
    /// </summary>
    private Vector3 _oriniglaPos;

    private PictureInfo _pictureInfo;

    /// <summary>
    /// 图片所在的行
    /// </summary>
    private int _indexColumn;
    /// <summary>
    /// 图片所在的列
    /// </summary>
    private int _indexRow;

    /// <summary>
    /// 图片所在横的最远图片的位置，分辨率为单位
    /// </summary>
    private float _maxScreenPos;
    public MeshRenderer MeshRenderer { get; private set; }

    public int PictureId;

   
    /// <summary>
    /// 原本的缩放比例
    /// </summary>
    private Vector3 _orinigalSize;

    /// <summary>
    /// 该面片所代表的的屏幕尺寸
    /// </summary>
    public Vector2 ScreenSize { get; private set; }

    /// 图片的变化类型，运动类型,update里面每帧判断啥类型，就做出啥类型相关的动作
    /// </summary>
    private MoveType _moveType = MoveType.None;


    // Start is called before the first frame update
    void Start()
    {
      //  Init();
    }

    // Update is called once per frame
    void Update()
    {
        _vectorMove = Vector3.left;//暂定向左，其实向右也是可以的
    }

    public void LateUpdate()
    {
        if (_moveType == MoveType.ForLeft)
        {

            UpdatePictureState();
        }

        UpdateCheckScreenPosition();
    }

    public void Init(Material mat)
    {
        if (_isInitEnd) return;
        _isInitEnd = true;
        CacheTransform = this.transform;



        if (Math.Abs(CacheTransform.localScale.z - 1f) > Mathf.Epsilon)
        {
            throw new UnityException("目前仅支持z缩放为1");
        }

        MeshRenderer = this.GetComponent<MeshRenderer>();

        _curMaterial = mat;
        MeshRenderer.material = mat;
       
      
    }

    
    /// <summary>
    /// 获取该面片的屏幕尺寸,和屏幕位置
    /// </summary>
    public Rect GetScreenSizePos()
    {
        Vector3 worldPos = CacheTransform.position;

        Vector3 worldSize = CacheTransform.localScale;

        //转化到屏幕空间的Rect
        Vector2 screenCenterPos = Camera.main.WorldToScreenPoint(worldPos);



        Vector2 leftUpPosWorld = worldPos + new Vector3(-worldSize.x / 2, worldSize.y / 2);
        Vector2 leftUpScreenPos = Camera.main.WorldToScreenPoint(leftUpPosWorld);


        Vector2 RightUpPosWorld = worldPos + new Vector3(worldSize.x / 2, worldSize.y / 2);
        Vector2 RightUpScreenPos = Camera.main.WorldToScreenPoint(RightUpPosWorld);


        Vector2 RightDownPosWorld = worldPos + new Vector3(worldSize.x / 2, -worldSize.y / 2);
        Vector2 RightDownScreenPos = Camera.main.WorldToScreenPoint(RightDownPosWorld);



        float width = RightUpScreenPos.x - leftUpScreenPos.x;

        float height = RightUpScreenPos.y - RightDownScreenPos.y;

        Rect screenRect = new Rect(screenCenterPos,new Vector2(width,height));



        return screenRect;





    }
    /// <summary>
    /// 根据屏幕位置设置quad的3D位置
    /// </summary>
    /// <param name="screenPos"></param>
    public void SetScreenPos(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

        CacheTransform.transform.position = worldPos;
    }
    /// <summary>
    /// 根据像素尺寸，设置quad的3D大小
    /// </summary>
    /// <param name="size"></param>

    public void SetScreenSize(Vector2 size)
    {
        //先计算屏幕 0 0点在世界坐标的位置
        Vector2 zeroWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(0f,0f, Mathf.Abs(Camera.main.transform.position.z)));

        //Debug.Log("zeroWorldPos is " +zeroWorldPos);
        //再计算这个尺寸在世界的位置
        Vector2 sizeWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(size.x, size.y, Mathf.Abs(Camera.main.transform.position.z)));

        float worldScaleX = Mathf.Abs( sizeWorldPos.x - zeroWorldPos.x);

        float worldScaleY = Mathf.Abs(sizeWorldPos.y - zeroWorldPos.y);

        this.CacheTransform.localScale = new Vector3(worldScaleX,worldScaleY,1);//默认缩放为1


        ScreenSize = size;
    }



  
    /// <summary>
    /// 设置quad的屏幕位置和尺寸
    /// </summary>
    /// <param name="rect"></param>
    public void SetPosSize(Rect rect,Vector2 space)
    {
        SetScreenPos(rect.position);



        Vector2 size;

        if (rect.size.x > space.x && rect.size.y > space.y)
            size = new Vector2(rect.size.x - space.x, rect.size.y - space.y);
        else size = rect.size;

        SetScreenSize(size);
    }

    public void SetInfo(PictureInfo info,  MaterialPropertyBlock prop )
    {
        CacheTransform = new GameObject(info.Name).transform;

        CacheTransform.position = this.transform.position;

        _oriniglaPos = this.transform.position;

        CacheTransform.parent = this.transform.parent;

        this.transform.parent = CacheTransform;


        _materialPropertyBlock = prop;
        //默认初始化的时候前面的就是新海报  
       
       
        PictureId = info.Index;




        _orinigalSize = this.transform.localScale;

        _scale = this.transform.localScale;

        //Debug.Log(n);
        _materialPropertyBlock.SetInt("_Index", PictureId);
        _materialPropertyBlock.SetFloat("_Flag", 0);
        _materialPropertyBlock.SetFloat("_Width", _orinigalSize.x);
        _materialPropertyBlock.SetFloat("_Height", _orinigalSize.y);
       
        MeshRenderer.SetPropertyBlock(_materialPropertyBlock);
      

        _pictureInfo = info;



        this.name = PictureId.ToString();
        //GetInitDis(15f, true, _originPositions[i]);
        GetInitDis(15f,false,_oriniglaPos);
    }



    /// <summary>
    /// 是否开启图片移动
    /// </summary>
    private bool _isOpenPictureMove = false;
    /// <summary>
    /// 图片组成字的速度
    /// </summary>
    private float _speed = 1f;//800f/2f
    /// <summary>
    /// 图片向左移动的速度
    /// </summary>
    private float _leftSpeed = 2f;
    /// <summary>
    /// 图片的透明度
    /// </summary>
    private float _aplha = 1f;


    /// <summary>
    /// 图片运动的方向,这个向量的主要作用就是方向
    /// </summary>
    private Vector3 _vectorMove;

    /// <summary>
    /// 是否可以做完自身特效后,图片自身移动
    /// </summary>
    private bool _isMove = false;


    /// <summary>
    /// 缩放系数
    /// </summary>
    private Vector3 _scale = Vector3.zero;


    private List<Circular> _circularList = new List<Circular>();

    /// <summary>
    /// 更新图片向左流动
    /// </summary>
    void UpdatePictureState()
    {
        if (!_isOpenPictureMove || _vectorMove == Vector3.zero) return;

        Vector3 position;
        if (!_isMove)
            position = Vector3.Lerp(CacheTransform.position, _oriniglaPos, Time.deltaTime * _leftSpeed);
        else
        {
            _oriniglaPos.x -= 0.01f;
            position = Vector3.Lerp(CacheTransform.position, _oriniglaPos, Time.deltaTime * _leftSpeed);
        }

        Vector3 pos = position;

        //进入了圆形的次数
        int count = 0;

        foreach (Circular item in _circularList)
        {
            float d1 = (pos - item.CacheTransform.localPosition).sqrMagnitude;

            if (d1 <= item.Radius * item.Radius)
            {
                #region 进入圆的半径的时候,处理图片的位置和大小

                float angle = Vector3.Angle(_vectorMove, pos - item.CacheTransform.localPosition);

                Vector3 temp = Vector3.Cross(_vectorMove, pos - item.CacheTransform.localPosition);

                if (angle > 0 && angle < 180)
                {
                    if (temp.z <= 0)
                        angle = 360 - angle;
                    //Debug.Log("旋转的欧拉角是：" + angle);
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);

                    Vector3 v = _vectorMove.normalized * item.Radius;

                    pos = rotation * v + item.CacheTransform.localPosition;

                    float disTemp = Vector3.Distance(_oriniglaPos, item.CacheTransform.localPosition);

                    if (disTemp < item.Radius)//根据物体的距离与半径比，得到物体的缩放系数
                    {
                        float floatTemp = disTemp / item.Radius;
                        if (floatTemp > 1f) floatTemp = 1f;
                        //Debug.Log(floatTemp);
                        CacheTransform.localScale = Vector3.Lerp(CacheTransform.localScale, Vector3.one * floatTemp, Time.deltaTime * 2f);// Vector3.Lerp(CacheTransform.localScale, Vector3.one * floatTemp, Time.deltaTime * 5f);
                    }
                }
                else if (angle == 0 || angle == 180)//暂写出这个，很小概率出现这个逻辑,读者可以忽略这段代码
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, 180f);

                    Vector3 cirPosition;

                    cirPosition = rotation * (_vectorMove.normalized * item.Radius) + item.CacheTransform.localPosition;

                    float d2 = Vector3.Distance(pos, cirPosition);

                    float angle1 = 180 - ((d2 / (2 * item.Radius)) * 180f);

                    Quaternion rotation1 = Quaternion.Euler(0, 0, angle1);

                    Vector3 v = _vectorMove.normalized * item.Radius;

                    pos = rotation1 * v + item.CacheTransform.localPosition;

                    float ag = (d2 / (2 * item.Radius)) * 180f;

                    if (ag > 60 && ag < 120)
                        ag = 60f;
                    ag = Mathf.Deg2Rad * ag;

                    CacheTransform.localScale = new Vector3(Mathf.Abs(Mathf.Cos(ag)), Mathf.Abs(Mathf.Cos(ag)));
                }
                #endregion

                count++;//进入到这里，就证明进入到了圆的范围
            }
        }







        //处理图片在多个球体内的逻辑
        if (count > 0)//有进入圆球范围内
        {
            CacheTransform.position = pos;
        }
        else
        {
            CacheTransform.position = position;

            _scale = Vector3.Lerp(_scale, Vector3.one, Time.deltaTime * 2f);

            //缩放渐变
            CacheTransform.localScale = Vector3.Lerp(CacheTransform.localScale,  _scale, Time.deltaTime * 2f);
        }
    }

    /// <summary>
    /// 初始化图片的位置，当初始化完成后，这些数据攻update每帧调用
    /// </summary>
    /// <param name="f">距离系数</param>
    /// <param name="isLeftRigt">是否左右运动</param>
    /// <param name="originPosition">原始位置</param>
    public void GetInitDis(float f, bool isLeftRigt, Vector3 originPosition)
    {
        //_oriniglaPos = originPosition;

      

        //if (isLeftRigt)
        //{
        //    if (CacheTransform.localPosition.y >= 0)
        //        CacheTransform.localPosition = new Vector3(Mathf.Abs(_oriniglaPos.y * f) + _oriniglaPos.x, _oriniglaPos.y, _oriniglaPos.z);
        //    else
        //        CacheTransform.localPosition = new Vector3(-(Mathf.Abs(_oriniglaPos.y * f) - _oriniglaPos.x), _oriniglaPos.y, _oriniglaPos.z);
        //}
        //else
        //    CacheTransform.localPosition = new Vector3(Mathf.Abs(_oriniglaPos.y * f) + _oriniglaPos.x, _oriniglaPos.y, _oriniglaPos.z);

        CacheTransform.localScale = Vector3.zero;//初始化尺寸的大小为0

        _isOpenPictureMove = true;

       

       // _leftSpeed = 0.1f;//运动的速度

        _aplha = 1f;//图片的透明度

        _moveType = MoveType.ForLeft;//图片的运动方式为向左运动

        _isMove = false;

        LateUpdateMovePicture(4f);
    }
    private void LateUpdateMovePicture(float time)
    {
        StartCoroutine(GlobalSetting.WaitTime(time, () =>
        {
            _isMove = true;
        }));


    }

    /// <summary>
    /// 检查图片是否越出屏幕外边，如果跃出，则重新布局到右边屏幕外边
    /// </summary>
    private void UpdateCheckScreenPosition()
    {
        if (_isMove)
        {
            Vector3 worldPosition = CacheTransform.position;//先把坐标变成世界坐标

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);//该方法规定参数必须是世界坐标

            if (screenPos.x+ScreenSize.x/2f  <= 0f)//证明从左边跃出屏幕,这里加多一个常量，因为是3D空间，测算图片的屏幕空间和大小需要麻烦操作，这里直接人工操作 
            {
                //_widget.transform.localPosition = new Vector3(_widget.transform.localPosition)
                //得到屏幕最右边的坐标
                Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width+ ScreenSize.x/2f, screenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

                Vector3 tempPosition = v;//这里必须把转换到得世界坐标变换成某个物体的局部坐标

               // tempPosition = new Vector3(tempPosition.x, tempPosition.y, tempPosition.z);

                CacheTransform.position = tempPosition;

                _oriniglaPos = tempPosition;


            }
        }
    }

#if UNITY_EDITOR
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
    //    {
    //       // Debug.Log(GetScreenSize().ToString());

    //       SetScreenPos(new Vector2(1920f,0f));

    //       SetScreenSize(new Vector2(100, 1080));
    //    }
    //}

#endif
}
