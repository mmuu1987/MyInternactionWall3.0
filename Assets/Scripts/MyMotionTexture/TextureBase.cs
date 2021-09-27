using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 执行一些底层的操作，比如坐标换算，从  3D坐标换算到2D坐标，或者2D坐标换算到3D坐标
/// </summary>
public class TextureBase : MonoBehaviour
{

    public Transform CacheTransform { get; protected set; }
    /// <summary>
    /// 该面片所代表的的屏幕尺寸
    /// </summary>
    public Vector2 ScreenSize { get; protected set; }


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

        Rect screenRect = new Rect(screenCenterPos, new Vector2(width, height));



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
        Vector2 zeroWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Mathf.Abs(Camera.main.transform.position.z)));

        //Debug.Log("zeroWorldPos is " +zeroWorldPos);
        //再计算这个尺寸在世界的位置
        Vector2 sizeWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(size.x, size.y, Mathf.Abs(Camera.main.transform.position.z)));

        float worldScaleX = Mathf.Abs(sizeWorldPos.x - zeroWorldPos.x);

        float worldScaleY = Mathf.Abs(sizeWorldPos.y - zeroWorldPos.y);

        this.CacheTransform.localScale = new Vector3(worldScaleX, worldScaleY, 1);//默认缩放为1


        ScreenSize = size;
    }

    /// <summary>
    /// 设置quad的屏幕位置和尺寸
    /// </summary>
    /// <param name="rect"></param>
    public void SetPosSize(Rect rect, Vector2 space)
    {
        SetScreenPos(rect.position);



        Vector2 size;

        if (rect.size.x > space.x && rect.size.y > space.y)
            size = new Vector2(rect.size.x - space.x, rect.size.y - space.y);
        else size = rect.size;

        SetScreenSize(size);
    }

}
