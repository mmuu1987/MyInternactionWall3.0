using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
///
/// </summary>
public class MyMotionTexture : MotionTextureBase
{
    /// 图片的变化类型，运动类型,update里面每帧判断啥类型，就做出啥类型相关的动作
    /// </summary>
    private MoveType _moveType = MoveType.None;
    /// <summary>
    /// 所在行的最大的屏幕位置
    /// </summary>
    public float MaxScreenPos;

    /// <summary>
    /// 是否跟圆交互，判定不交互的条件为原始点跟图片原点的距离是否小于一个数值
    /// </summary>
    private bool _isCircleInteraction;
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

   

    public override void SetInfo(PictureInfo info,  MaterialPropertyBlock prop,int row,int column )
    {
       base.SetInfo(info,prop, row, column);
        //GetInitDis(15f, true, _originPositions[i]);
        GetInitDis(15f,false);
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
    private float _leftSpeed = 1f;
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


    public List<Circular> CircularList = new List<Circular>();

    //进入了圆形的次数
    private int _enterCirclecount = 0;
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
            _oriniglaPos.x -= 0.001f;
            position = Vector3.Lerp(CacheTransform.position, _oriniglaPos, Time.deltaTime * _leftSpeed);

            //if (this.name == "500")
            //{
            //    Debug.Log("t is:" + Time.deltaTime * _leftSpeed+ "  CacheTransform.position is:" + CacheTransform.position+ "    _oriniglaPos is:" + _oriniglaPos + "    position is:" + position);
            //}
        }

        Vector3 pos = position;

        _enterCirclecount = 0;

        foreach (Circular item in CircularList)
        {
            float d1 = (pos - item.CacheTransform.position).sqrMagnitude;

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
                        CacheTransform.localScale = Vector3.Lerp(CacheTransform.localScale, OrinigalSize * floatTemp, Time.deltaTime * 2f);// Vector3.Lerp(CacheTransform.localScale, Vector3.one * floatTemp, Time.deltaTime * 5f);
                    }
                }
                else if (angle == 0 || angle == 180)//暂写出这个，很小概率出现这个逻辑,可以忽略这段代码
                {
                    //Quaternion rotation = Quaternion.Euler(0, 0, 180f);

                    //Vector3 cirPosition;

                    //cirPosition = rotation * (_vectorMove.normalized * item.Radius) + item.CacheTransform.localPosition;

                    //float d2 = Vector3.Distance(pos, cirPosition);

                    //float angle1 = 180 - ((d2 / (2 * item.Radius)) * 180f);

                    //Quaternion rotation1 = Quaternion.Euler(0, 0, angle1);

                    //Vector3 v = _vectorMove.normalized * item.Radius;

                    //pos = rotation1 * v + item.CacheTransform.localPosition;

                    //float ag = (d2 / (2 * item.Radius)) * 180f;

                    //if (ag > 60 && ag < 120)
                    //    ag = 60f;
                    //ag = Mathf.Deg2Rad * ag;

                    //CacheTransform.localScale = new Vector3(Mathf.Abs(Mathf.Cos(ag)), Mathf.Abs(Mathf.Cos(ag)));
                }
                #endregion

                _enterCirclecount++;//进入到这里，就证明进入到了圆的范围
            }
        }
        //处理图片在多个球体内的逻辑
        if (_enterCirclecount > 0)//有进入圆球范围内
        {
            CacheTransform.position = pos;
        }
        else
        {

            CacheTransform.position = position;

            Scale = Vector3.Lerp(Scale, OrinigalSize, Time.deltaTime * 2f);

            //缩放渐变
            CacheTransform.localScale = Scale;
        }
    }

    /// <summary>
    /// 初始化图片的位置，当初始化完成后，这些数据攻update每帧调用
    /// </summary>
    /// <param name="f">距离系数</param>
    /// <param name="isLeftRigt">是否左右运动</param>
    /// <param name="originPosition">原始位置</param>
    public void GetInitDis(float f, bool isLeftRigt)
    {
        if (isLeftRigt)
        {
            if (CacheTransform.position.y >= 0)
                CacheTransform.position = new Vector3(Mathf.Abs(_oriniglaPos.y * f) + _oriniglaPos.x, _oriniglaPos.y, _oriniglaPos.z);
            else
                CacheTransform.position = new Vector3(-(Mathf.Abs(_oriniglaPos.y * f) - _oriniglaPos.x), _oriniglaPos.y, _oriniglaPos.z);
        }
        else
            CacheTransform.position = new Vector3(Mathf.Abs(_oriniglaPos.y * f) + _oriniglaPos.x, _oriniglaPos.y, _oriniglaPos.z);

        // CacheTransform.position = Vector3.zero;//初始化尺寸的大小为0

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
        if (_isMove && _enterCirclecount<=0)
        {

            Vector3 dir = Vector3.Normalize(CacheTransform.position - _oriniglaPos);

            float dotVal = Vector3.Dot(Vector3.right,dir );

            if (Mathf.Abs(dotVal) < 1f)//判断是否水平跟随运动点
            {
                return;
            }

            //先把坐标变成世界坐标
            Vector3 worldPosition = _oriniglaPos;

            //得到两个位置的X轴的距离差
            float xTemp = _oriniglaPos.x - CacheTransform.position.x;

            //得到两个位置的X轴的屏幕的距离
            float xScreenTemp = GlobalSetting.GetScreenSizePos(xTemp);


            //该方法规定参数必须是世界坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);


            //图片最左边的点的X坐标
            float screenMinLeft = screenPos.x + ScreenSize.x / 2f - xScreenTemp;

            //图片越过左边0线后，重置的X轴位置
            float screenMaxRight = MaxScreenPos + xScreenTemp - ScreenSize.x / 2f;

            //最终回到右边的位置
            float screenRealPos = screenMaxRight + screenMinLeft;

            if (screenMinLeft <= 0f)//证明从左边跃出屏幕,这里加多一个常量，因为是3D空间，测算图片的屏幕空间和大小需要麻烦操作，这里直接人工操作 
            {
                //_widget.transform.localPosition = new Vector3(_widget.transform.localPosition)
                //得到屏幕最右边的坐标
                Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(screenRealPos, screenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

                Vector3 tempPosition = v;//这里必须把转换到得世界坐标变换成某个物体的局部坐标

               // tempPosition = new Vector3(tempPosition.x, tempPosition.y, tempPosition.z);

                CacheTransform.position = tempPosition +new Vector3(xTemp,0f,0f);

                _oriniglaPos = tempPosition;

               // _oriniglaPos.x += xTemp;//复位到左边之后我们要把这个距离差也带上去，否则会出现偏差

                //  ChangeMoveSpeed();

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
