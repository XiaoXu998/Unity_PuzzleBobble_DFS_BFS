using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 引入DOTween命名空间

public class Cube : MonoBehaviour
{
    public bool isEnable = false;
    public CubeColor cubeColor;
    public List<Cube> neighbours = new List<Cube>();
    public float riseHeight = 1.0f; // 上升高度
    public float animationDuration = 0.5f; // 动画持续时间
    private Vector3 basePosition; // 记录基础位置
    public int row;
    public int col;

    //PaoPaoLong模式变量
    public bool isRoot = false;

    //DFS模式变量
    
    // Start is called before the first frame update
    void Start()
    {
        SetColor(CubeColor.Grey);
        basePosition = new Vector3(transform.position.x, 0, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    // 启用Cube
    public void EnableCube()
    {
        if (isEnable) return;
        isEnable = true;
        Rise();
        
        // 找到CubeManager并更新邻居关系
        CubeManager cubeManager = Camera.main.GetComponent<CubeManager>();
        if (cubeManager != null)
        {
            cubeManager.InitCubesNeighbours();
        }
    }
    
    // 禁用Cube
    public void DisableCube()
    {
        if (!isEnable) return;
        isEnable = false;
        neighbours.Clear();
        SetColor(CubeColor.Grey);
        Fall();
        
        // 找到CubeManager并更新邻居关系
        CubeManager cubeManager = Camera.main.GetComponent<CubeManager>();
        if (cubeManager != null)
        {
            cubeManager.InitCubesNeighbours();
        }
    }

    // 上升方法
    public void Rise()
    {
        // 停止所有正在进行的动画
        DOTween.Kill(transform);
        
        // 使用绝对位置，确保动画开始和结束位置正确
        Vector3 targetPosition = new Vector3(basePosition.x, riseHeight, basePosition.z);
        
        // 使用DOTween创建上升动画，使用Ease.OutQuad实现非线性效果
        transform.DOMove(targetPosition, animationDuration)
            .SetEase(Ease.OutQuad);
    }
    
    // 下降方法
    public void Fall()
    {
        // 停止所有正在进行的动画
        DOTween.Kill(transform);
        
        // 使用绝对位置，确保动画开始和结束位置正确
        Vector3 targetPosition = new Vector3(basePosition.x, 0, basePosition.z);
        
        // 使用DOTween创建下降动画，使用Ease.InQuad实现非线性效果
        transform.DOMove(targetPosition, animationDuration)
            .SetEase(Ease.InQuad);
    }
    
    // 设置颜色
    public void SetColor(CubeColor color)
    {
        cubeColor = color;
        Color unityColor;
        switch (color)
        {
            case CubeColor.RED:
                unityColor = new Color(0.9f, 0.4f, 0.4f); // 中等饱和度红色
                break;
            case CubeColor.BLUE:
                unityColor = new Color(0.4f, 0.4f, 0.9f); // 中等饱和度蓝色
                break;
            case CubeColor.GREEN:
                unityColor = new Color(0.4f, 0.9f, 0.4f); // 中等饱和度绿色
                break;
            case CubeColor.Yellow:
                unityColor = new Color(0.9f, 0.9f, 0.4f); // 中等饱和度黄色
                break;
            case CubeColor.Purple:
                unityColor = new Color(0.75f, 0.4f, 0.75f); // 中等饱和度紫色
                break;
            case CubeColor.White:
                unityColor = Color.white;
                break;
            case CubeColor.Grey:
            default:
                unityColor = Color.grey;
                break;
        }
        GetComponent<Renderer>().material.color = unityColor;
    }

    // 设置邻居
    public void SetNeighbours(List<Cube> neighbours)
    {
        this.neighbours = neighbours;
    }

    // 设置随机颜色(除了Grey和White)
    public void SetRandomColor()
    {
        List<CubeColor> colors = new List<CubeColor> { CubeColor.RED, CubeColor.BLUE, CubeColor.GREEN, CubeColor.Yellow, CubeColor.Purple };
        int randomIndex = Random.Range(0, colors.Count);
        SetColor(colors[randomIndex]);
    }
    
}
public enum CubeColor
{
    White,
    Grey,
    RED,
    BLUE,
    GREEN, 
    Yellow,
    Purple, 
}