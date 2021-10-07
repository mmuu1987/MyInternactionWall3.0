using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 根据图片的像素，转变成屏幕的位置
/// </summary>
public class PixelChangePosition : MonoBehaviour
{

    public Texture2D Texture;

    public float range = 2.7f;

    public Camera _camera;

    public int RealScreenWidth = 9600;

    public int RealScreenHeight = 2160;

    /// <summary>
    /// Z轴的位置
    /// </summary>
    public float Z = 10f;
    /// <summary>
    /// 变换到该父物体下
    /// </summary>
    public Transform Parent;

    List<Vector2> _screenPosition = new List<Vector2>();

    /// <summary>
    /// 缓存物体
    /// </summary>
    private GameObject _temp;

    public List<Vector2> ScreenPosition
    {
        get { return _screenPosition; }
        set { _screenPosition = value; }
    }

    void Awake()
    {
        GetPixelInfo();

        SetTestScreenPosition();

        _temp = new GameObject();
    }

    void Start()
    {

    }
    /// <summary>
    /// 获取特定像素值的信息
    /// </summary>
    private void GetPixelInfo()//现在暂定白色
    {
        Color[] colors = Texture.GetPixels();

        List<int> index = new List<int>();

        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].r + colors[i].g + colors[i].b > range)
            {
                index.Add(i);//添加像素索引,这些索引的信息都是接近白色的颜色信息，下一步把这些索引转换成屏幕上的点的坐标
            }
        }

        _screenPosition = new List<Vector2>();

        int width = Texture.width;

        //根据index转换成屏幕xy坐标
        for (int i = 0; i < index.Count; i++)
        {
            _screenPosition.Add(new Vector2(index[i] / width, index[i] % width));
        }


    }
    /// <summary>
    /// 设置测试的屏幕的像素
    /// </summary>
    public void SetTestScreenPosition()
    {
        float w = Screen.width * 1f / RealScreenWidth;

        float h = Screen.height * 1f / RealScreenHeight;


        for (int i = 0; i < _screenPosition.Count; i++)
        {
            _screenPosition[i] = new Vector2((int)_screenPosition[i].y * h, (int)_screenPosition[i].x * w);
        }

    }
   
    public Vector3 GetWorldPosition()
    {
        int index = Random.Range(1, _screenPosition.Count);

        Vector2 v = _screenPosition[index];//获取到屏幕上的随机一个位置的点
        Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(v.x , v.y , Mathf.Abs(Camera.main.transform.position.z)));//数值是让其能够在屏幕中间显示

        return p;
      
    }
  
}
