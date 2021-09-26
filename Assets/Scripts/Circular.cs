using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circular : MonoBehaviour
{
    /// <summary>
    /// 圆形的图片
    /// </summary>
    private RawImage tex;
    /// <summary>
    /// 碰撞的半径
    /// </summary>
    public float Radius;



    /// <summary>
    /// 第二半径减去第一半径的差值
    /// </summary>
    private float _differenceValue;
    /// <summary>
    /// 第二半径减去第一半径的差值
    /// </summary>
    public float DifferenceValue
    {
        get { return _differenceValue; }

    }

    private Transform _cacheTransform;

    public Transform CacheTransform
    {
        get { return _cacheTransform; }

    }

    void Awake()
    {


        _cacheTransform = this.transform;

        tex = this.GetComponent<RawImage>();

        Vector2 size = tex.rectTransform.sizeDelta;

        tex.rectTransform.sizeDelta = new Vector2(0,size.y);

        Radius = 0f;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        ZoomSize();
    }
    /// <summary>
    /// 放大球体
    /// </summary>
    void ZoomSize()
    {
        Radius = Mathf.Lerp(Radius, 600, Time.deltaTime * 8f);
        Vector2 size = tex.rectTransform.sizeDelta;

        float x = Mathf.Lerp(size.x, 1000, Time.deltaTime * 8f);
        tex.rectTransform.sizeDelta = new Vector2(x,size.y);
    }
}
