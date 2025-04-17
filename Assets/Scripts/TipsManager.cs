using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsManager : MonoBehaviour
{
    private CubeManager cubeManager;
    private TextMeshProUGUI tipText;
    private GameMode currentMode = GameMode.PaoPaoLong;
    private bool isVisible = true; // 添加一个变量来跟踪提示面板的可见性
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取CubeManager组件（假设挂在摄像机上）
        cubeManager = Camera.main.GetComponent<CubeManager>();
        if (cubeManager == null)
        {
            Debug.LogError("无法找到CubeManager组件，请确保它已挂在主摄像机上");
        }
        
        // 获取TextMeshProUGUI组件
        tipText = GetComponentInChildren<TextMeshProUGUI>();
        if (tipText == null)
        {
            Debug.LogError("无法找到TextMeshProUGUI组件，请确保TipPanel上有TMP Text组件");
        }
        
        // 初始化提示文本
        UpdateTipText();
    }

    // Update is called once per frame
    void Update()
    {
        currentMode = cubeManager.gameMode;
        UpdateTipText();
        
        // 检测P键按下，切换提示面板的可见性
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleTipVisibility();
        }
    }
    
    // 切换提示面板的可见性
    private void ToggleTipVisibility()
    {
        isVisible = !isVisible;
        // 不要禁用整个面板，只控制文本的显示
        if (tipText != null)
        {
            tipText.enabled = isVisible;
        }
        
        // 如果面板重新显示，更新提示文本
        if (isVisible)
        {
            UpdateTipText();
        }
    }
    
    // 根据当前游戏模式更新提示文本
    private void UpdateTipText()
    {
        if (tipText == null) return;
        
        switch (currentMode)
        {
            case GameMode.PaoPaoLong:
                tipText.text = "泡泡龙模式\n" +
                               "T键：设置所有立方体随机颜色\n" +
                               "R键：设置边缘立方体为Root\n" +
                               "鼠标左键：点击启用立方体并设置颜色\n" +
                               "鼠标右键：点击禁用立方体\n" +
                               "左右方向键：切换颜色\n" +
                               "当前颜色：" + cubeManager.selectedColor.ToString() + "\n" +
                               "上下方向键：切换模式\n" +
                               "当前模式：" + cubeManager.paoPaoLongMode.ToString() + "\n" +
                               "P键：隐藏/显示提示面板";
                break;
                
            case GameMode.DFS:
                tipText.text = "深度优先搜索模式\n" +
                               "动画操作期间禁用任何操作\n" +
                               "仅作用于启用的立方体\n" +
                               "鼠标左键：切换立方体启用\n" +
                               "鼠标右键：开启DFS\n" +
                               "R键：禁用当前DFS\n" +
                               "P键：隐藏/显示提示面板";
                break;
                
            case GameMode.BFS:
                tipText.text = "广度优先搜索模式\n" +
                               "动画操作期间禁用任何操作\n" +
                               "仅作用于启用的立方体\n" +
                               "鼠标左键：切换立方体启用\n" +
                               "鼠标右键：开启BFS\n" +
                               "R键：禁用当前BFS\n" +
                               "P键：隐藏/显示提示面板";
                break;
            default:
                tipText.text = "未知模式\n" +
                               "P键：隐藏/显示提示面板";
                break;
        }
    }
}
