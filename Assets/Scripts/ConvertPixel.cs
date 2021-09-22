using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 把quad 的在3d空间的size转为屏幕尺寸,目前用transform.localscale代表像素尺寸
/// 默认Z轴的位置为0
/// </summary>
public class ConvertPixel : MonoBehaviour
{

    private Transform _cacheTransform;
    // Start is called before the first frame update
    void Start()
    {
        _cacheTransform = this.transform;



        if (Math.Abs(_cacheTransform.localScale.z - 1f) > Mathf.Epsilon)
        {
            throw new UnityException("目前仅支持z缩放为1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        _cacheTransform = this.transform;



        if (Math.Abs(_cacheTransform.localScale.z - 1f) > Mathf.Epsilon)
        {
            throw new UnityException("目前仅支持z缩放为1");
        }
    }
    /// <summary>
    /// 获取该面片的屏幕尺寸
    /// </summary>
    public Rect GetScreenSize()
    {
        Vector3 worldPos = _cacheTransform.position;

        Vector3 worldSize = _cacheTransform.localScale;

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

    public void SetScreenPos(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

        _cacheTransform.transform.position = worldPos;
    }

    public void SetScreenSize(Vector2 size)
    {
        //先计算屏幕 0 0点在世界坐标的位置
        Vector2 zeroWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(0f,0f, Mathf.Abs(Camera.main.transform.position.z)));

        //Debug.Log("zeroWorldPos is " +zeroWorldPos);
        //再计算这个尺寸在世界的位置
        Vector2 sizeWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(size.x, size.y, Mathf.Abs(Camera.main.transform.position.z)));

        float worldScaleX = Mathf.Abs( sizeWorldPos.x - zeroWorldPos.x);

        float worldScaleY = Mathf.Abs(sizeWorldPos.y - zeroWorldPos.y);

        this._cacheTransform.localScale = new Vector3(worldScaleX,worldScaleY,1);//默认缩放为1

    }

    public void SetPosSize(Rect rect)
    {
        SetScreenPos(rect.position);

        SetScreenSize(rect.size);
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
           // Debug.Log(GetScreenSize().ToString());

           SetScreenPos(new Vector2(1920f,0f));

           SetScreenSize(new Vector2(100, 1080));
        }
    }

#endif
}
