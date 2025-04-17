using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeManager : MonoBehaviour
{
    // 基础变量
    public GameMode gameMode;
    public float spacing = 0.2f; // 立方体之间的间距，可调整
    private GameObject[,] cubes; // 存储创建的立方体
    public int rows;
    public int columns;
    public bool isWorking = false;
    public float waitTime = 0.3f;

    // 顶部按钮
    public Button Btn_All;
    public Button Btn_EnableAll;
    public Button Btn_DisableAll;
    public Button Btn_EnableRandom;

    // 底部按钮
    public Button Btn_PaoPaoLong;
    public Button Btn_DFS;
    public Button Btn_BFS;

    // PaoPaoLong模式变量
    public CubeColor selectedColor = CubeColor.RED;
    public PaoPaoLongMode paoPaoLongMode = PaoPaoLongMode.DFS;
    
    //DFS or BFS模式变量
    

    // Start is called before the first frame update
    void Start()
    {
        InitButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWorking)
        {
            DisableAllButtons();
        }
        else
        {
            EnableAllButtons();
        }
        switch (gameMode)
        {
            case GameMode.PaoPaoLong:
                PaoPaoLong_Input();
                break;  
            case GameMode.DFS:
                DFS_Input();
                break;
            case GameMode.BFS:
                BFS_Input();
                break;
        }
    }

    // PaoPaoLong模式输入操作
    public void PaoPaoLong_Input()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isWorking) //设置所有Cube随机颜色
        {
            SetCubesColor();
        }
        if (Input.GetKeyDown(KeyCode.R) && !isWorking) //设置Cubes的Root
        {
            SetCubesRoot();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) //切换颜色
        {
            // 创建可选颜色列表（不包括White和Grey）
            List<CubeColor> availableColors = new List<CubeColor> 
            { 
                CubeColor.RED, 
                CubeColor.BLUE, 
                CubeColor.GREEN, 
                CubeColor.Yellow, 
                CubeColor.Purple 
            };
            
            // 获取当前颜色在列表中的索引
            int currentIndex = availableColors.IndexOf(selectedColor);
            
            // 根据按键方向调整索引
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentIndex = (currentIndex + 1) % availableColors.Count;
            }
            else // 左方向键
            {
                currentIndex = (currentIndex - 1 + availableColors.Count) % availableColors.Count;
            }
            
            // 更新选中的颜色
            selectedColor = availableColors[currentIndex];
            Debug.Log($"当前选中颜色: {selectedColor}");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) && !isWorking) //切换检测模式
        {
            // 获取所有PaoPaoLongMode枚举值
            PaoPaoLongMode[] modes = (PaoPaoLongMode[])System.Enum.GetValues(typeof(PaoPaoLongMode));
            
            // 获取当前模式的索引
            int currentIndex = (int)paoPaoLongMode;
            
            // 根据按键方向调整索引
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex + 1) % modes.Length;
            }
            else // 下方向键
            {
                currentIndex = (currentIndex - 1 + modes.Length) % modes.Length;
            }
            
            // 更新当前模式
            paoPaoLongMode = modes[currentIndex];
            Debug.Log($"当前泡泡龙模式: {paoPaoLongMode}");
        }
        if (Input.GetMouseButtonDown(0) && !isWorking) // 点击启用Cube并设置颜色
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (!cube.isEnable)
            {
                cube.SetColor(selectedColor);
                cube.EnableCube();
                PaoPaoLong(cube);
                Debug.Log("已启用" + clickedCube.name);
            }
            else
            {
                Debug.Log("该Cube已启用，无法使用");
            }
        } 
        if (Input.GetMouseButtonDown(1) && !isWorking) // 点击禁用Cube
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (cube.isEnable)
            {
                cube.DisableCube();
                Debug.Log("已禁用" + clickedCube.name);
            }
            else
            {
                Debug.Log("该Cube已禁用，无法再次禁用");
            }
        } 
    }

    // DFS模式输入操作
    public void DFS_Input()
    {
        if (Input.GetMouseButtonDown(1) && !isWorking) // 点击启用DFS
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (!cube.isEnable)
            {
                DFS(cube);
                Debug.Log("从" + clickedCube.name + "开始DFS");
            }
            else
            {
                Debug.Log("该Cube已启用，无法使用");
            }
        } 
        if (Input.GetMouseButtonDown(0)) // 点击切换Cube状态
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (!cube.isEnable)
            {
                cube.SetColor(CubeColor.RED);
                cube.EnableCube();
                Debug.Log("已启用" + clickedCube.name);
            }
            else
            {
                cube.DisableCube();
                Debug.Log("已启用" + clickedCube.name);
            }
        } 
        if (Input.GetKeyDown(KeyCode.R)) //暂停当前DFS
        {
            if (isWorking)
            {
                StopAllCoroutines();
                isWorking = false;
                EnableAllButtons();
                Debug.Log("已暂停当前DFS");
            }
        }
    }

    // BFS模式输入操作
    public void BFS_Input()
    {
        if (Input.GetMouseButtonDown(1) && !isWorking) // 点击启用BFS
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (!cube.isEnable)
            {
                BFS(cube);
                Debug.Log("从" + clickedCube.name + "开始BFS");
            }
            else
            {
                Debug.Log("该Cube已启用，无法使用");
            }
        } 
        if (Input.GetMouseButtonDown(0)) // 点击切换Cube状态
        {
            GameObject clickedCube = GetCubeByClick();
            if (clickedCube == null) return;
            Cube cube = clickedCube.GetComponent<Cube>();
            if (!cube.isEnable)
            {
                cube.SetColor(CubeColor.RED);
                cube.EnableCube();
                Debug.Log("已启用" + clickedCube.name);
            }
            else
            {
                cube.DisableCube();
                Debug.Log("已启用" + clickedCube.name);
            }
        } 
        if (Input.GetKeyDown(KeyCode.R)) //暂停当前BFS
        {
            if (isWorking)
            {
                StopAllCoroutines();
                isWorking = false;
                EnableAllButtons();
                Debug.Log("已暂停当前BFS");
            }
        }
    }

    // 初始化按钮
    public void InitButtons()
    {
        Btn_All.onClick.AddListener(()=>CreateCubesByRows_Columns(rows,columns));
        Btn_EnableAll.onClick.AddListener(()=>SetAllCubesStatus(true));
        Btn_DisableAll.onClick.AddListener(()=>SetAllCubesStatus(false));
        Btn_EnableRandom.onClick.AddListener(()=>SetRandomCubesStatus(true));

        Btn_PaoPaoLong.onClick.AddListener(()=>gameMode = GameMode.PaoPaoLong);
        Btn_DFS.onClick.AddListener(()=>gameMode = GameMode.DFS);
        Btn_BFS.onClick.AddListener(()=>gameMode = GameMode.BFS);
    }

    // 初始化方法，创建行乘列个Cube
    public void CreateCubesByRows_Columns(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        // 清除已有的立方体（如果有）
        ClearCubes();
        
        // 初始化数组
        cubes = new GameObject[rows, columns];
        
        // 加载Cube预制体
        GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        
        if (cubePrefab == null)
        {
            Debug.LogError("无法加载Cube预制体，请确保在Resources/Prefabs/目录下存在Cube预制体");
            return;
        }
        
        // 计算起始位置，使得整个网格以(0,0,0)为中心
        float startX = -(columns - 1) * (1 + spacing) / 2;
        float startZ = (rows - 1) * (1 + spacing) / 2; // 修改为正值，从最大Z值开始
        
        // 创建立方体
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 计算位置，Z轴方向反转
                Vector3 position = new Vector3(
                    startX + col * (1 + spacing),
                    0, // Y轴位置固定为0
                    startZ - row * (1 + spacing) // 使用减法，让row增加时Z值减小
                );
                
                // 实例化立方体，不设置父物体，让它直接存在于场景根级别
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.name = $"Cube_{row}_{col}";
                
                // 存储到数组中
                cubes[row, col] = cube;
                
                // 给Cube组件的row和col属性赋值
                Cube cubeComponent = cube.GetComponent<Cube>();
                if (cubeComponent != null)
                {
                    cubeComponent.row = row;
                    cubeComponent.col = col;
                }
            }
        }
        
        Debug.Log($"成功创建了 {rows}x{columns} 个立方体");
        InitCubesNeighbours();
    }

    // 该方法已经弃用
    // 初始化方法，创建行乘列个Cube，随机决定哪些位置生成立方体
    public void CreateCubesRandomly(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        // 清除已有的立方体（如果有）
        ClearCubes();
        
        // 初始化数组
        cubes = new GameObject[rows, columns];
        
        // 加载Cube预制体
        GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        
        if (cubePrefab == null)
        {
            Debug.LogError("无法加载Cube预制体，请确保在Resources/Prefabs/目录下存在Cube预制体");
            return;
        }
        
        // 计算起始位置，使得整个网格以(0,0,0)为中心
        float startX = -(columns - 1) * (1 + spacing) / 2;
        float startZ = (rows - 1) * (1 + spacing) / 2; // 修改为正值，从最大Z值开始
        
        // 随机决定哪些位置生成立方体
        System.Random random = new System.Random();
        int totalCubes = 0;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 随机决定是否在此位置生成立方体（50%的概率）
                if (random.NextDouble() < 0.5)
                {
                    // 计算位置，Z轴方向反转
                    Vector3 position = new Vector3(
                        startX + col * (1 + spacing),
                        0, // Y轴位置固定为0
                        startZ - row * (1 + spacing) // 使用减法，让row增加时Z值减小
                    );
                    
                    // 实例化立方体，不设置父物体，让它直接存在于场景根级别
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                    cube.name = $"Cube_{row}_{col}";
                    
                    // 存储到数组中
                    cubes[row, col] = cube;
                    
                    // 给Cube组件的row和col属性赋值
                    Cube cubeComponent = cube.GetComponent<Cube>();
                    if (cubeComponent != null)
                    {
                        cubeComponent.row = row;
                        cubeComponent.col = col;
                    }
                    
                    totalCubes++;
                }
            }
        }
        
        Debug.Log($"成功随机创建了 {totalCubes} 个立方体，在 {rows}x{columns} 的网格中");
        InitCubesNeighbours();
    }
    
    // 初始化方法，设置Cube的邻居
    public void InitCubesNeighbours()
    {
        // 确保cubes数组已初始化
        if (cubes == null)
        {
            Debug.LogError("请先调用Init方法初始化立方体网格");
            return;
        }
        
        // 遍历所有立方体
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject currentCubeObj = cubes[row, col];
                if (currentCubeObj == null) continue;
                
                Cube currentCube = currentCubeObj.GetComponent<Cube>();
                if (currentCube == null) continue;
                
                // 创建邻居列表
                List<Cube> neighbours = new List<Cube>();
                
                // 检查八个方向的邻居
                for (int r = -1; r <= 1; r++)
                {
                    for (int c = -1; c <= 1; c++)
                    {
                        // 跳过自身
                        if (r == 0 && c == 0) continue;
                        
                        int neighbourRow = row + r;
                        int neighbourCol = col + c;
                        
                        // 检查邻居是否在网格范围内
                        if (neighbourRow >= 0 && neighbourRow < rows && 
                            neighbourCol >= 0 && neighbourCol < columns)
                        {
                            GameObject neighbourObj = cubes[neighbourRow, neighbourCol];
                            if (neighbourObj != null)
                            {
                                Cube neighbour = neighbourObj.GetComponent<Cube>();
                                // 只有当邻居Cube已启用时才添加到邻居列表
                                if (neighbour != null)
                                {
                                    neighbours.Add(neighbour);
                                }
                            }
                        }
                    }
                }
                
                // 设置当前立方体的邻居
                currentCube.SetNeighbours(neighbours);
            }
        }
        Debug.Log("所有立方体的邻居设置完成");
    }
    
    // 清除所有立方体
    private void ClearCubes()
    {
        if (cubes != null)
        {
            for (int row = 0; row < cubes.GetLength(0); row++)
            {
                for (int col = 0; col < cubes.GetLength(1); col++)
                {
                    if (cubes[row, col] != null)
                    {
                        Destroy(cubes[row, col]);
                    }
                }
            }
        }
        
        // 清除所有子物体（以防万一）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // 设置Cubes状态(启用或禁用所有Cube)
    public void SetAllCubesStatus(bool isEnable)
    {
       if (cubes == null)
        {
            Debug.LogError("立方体数组未初始化，请先创建立方体");
            return;
        }
        int operatedCount = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cubeObj = cubes[row, col];
                if (cubeObj != null)
                {
                    Cube cube = cubeObj.GetComponent<Cube>();
                    if (cube != null)
                    {
                        if (isEnable)
                        {
                            cube.EnableCube();
                        }
                        else
                        {
                            cube.DisableCube();
                        }
                        operatedCount++;
                    }
                }
            }
        }
        Debug.Log($"已操作 {operatedCount} 个立方体"); 
    }
    
    // 随机设置Cubes状态(启用或禁用随机个Cube)
    public void SetRandomCubesStatus(bool isEnable)
    {
        if (cubes == null)
        {
            Debug.LogError("立方体数组未初始化，请先创建立方体");
            return;
        }
        
        System.Random random = new System.Random();
        int operatedCount = 0;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cubeObj = cubes[row, col];
                if (cubeObj != null)
                {
                    Cube cube = cubeObj.GetComponent<Cube>();
                    if (cube != null)
                    {
                        // 随机决定是否操作这个立方体（50%的概率）
                        if (random.NextDouble() < 0.5)
                        {
                            if (isEnable)
                            {
                                cube.EnableCube();
                            }
                            else
                            {
                                cube.DisableCube();
                            }
                            operatedCount++;
                        }
                    }
                }
            }
        }
        
        Debug.Log($"已随机{(isEnable ? "启用" : "禁用")} {operatedCount} 个立方体");
    }

    // 获取鼠标点击的Cube
    public GameObject GetCubeByClick()
    {
        // 创建射线，从摄像机发射到鼠标点击位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // 进行射线检测
        if (Physics.Raycast(ray, out hit))
        {
            // 检查击中的物体是否有Cube组件
            Cube cube = hit.collider.GetComponent<Cube>();
            if (cube != null)
            {
                Debug.Log("点击到了立方体: " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
        }
        
        Debug.Log("未点击到任何立方体");
        // 如果没有击中Cube或者没有击中任何物体，返回null
        return null;
    }
        
    // 设置所有Cube颜色
    public void SetCubesColor(CubeColor color)
    {  
        if (cubes == null)
        {
            Debug.LogError("立方体数组未初始化，请先创建立方体");
            return;
        }
        
        int coloredCount = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cubeObj = cubes[row, col];
                if (cubeObj != null)
                {
                    Cube cube = cubeObj.GetComponent<Cube>();
                    if (cube != null && cube.isEnable)
                    {
                        cube.SetColor(color);
                        coloredCount++;
                    }
                }
            }
        }
        
        Debug.Log($"已将 {coloredCount} 个立方体设置为 {color} 颜色");
    }

    // 设置所有Cube颜色(随机)
    public void SetCubesColor()
    {
        if (cubes == null)
        {
            Debug.LogError("立方体数组未初始化，请先创建立方体");
            return;
        }
        
        int coloredCount = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cubeObj = cubes[row, col];
                if (cubeObj != null)
                {
                    Cube cube = cubeObj.GetComponent<Cube>();
                    if (cube != null && cube.isEnable)
                    {
                        cube.SetRandomColor();
                        coloredCount++;
                    }
                }
            }
        }
        
        Debug.Log($"已随机设置 {coloredCount} 个立方体的颜色");
    }

    // 禁用所有按钮
    public void DisableAllButtons()
    {
        Btn_All.interactable = false;
        Btn_EnableAll.interactable = false;
        Btn_DisableAll.interactable = false;
        Btn_EnableRandom.interactable = false;
        Btn_PaoPaoLong.interactable = false;
        Btn_DFS.interactable = false;
        Btn_BFS.interactable = false;
    }

    // 启用所有按钮
    public void EnableAllButtons()
    {
        Btn_All.interactable = true;
        Btn_EnableAll.interactable = true;
        Btn_DisableAll.interactable = true;
        Btn_EnableRandom.interactable = true;
        Btn_PaoPaoLong.interactable = true;
        Btn_DFS.interactable = true;
        Btn_BFS.interactable = true;
    }

    /*
    以下方法为PaoPaoLong模式下的方法
    */
    // 设置Cubes的Root(默认方法，位于边缘的Cube为Root)
    public void SetCubesRoot()
    {
        List<CubeColor> colors = new List<CubeColor> { CubeColor.RED, CubeColor.BLUE, CubeColor.GREEN, CubeColor.Yellow, CubeColor.Purple };
        int randomIndex = Random.Range(0, colors.Count);
        if (cubes == null)
        {
            Debug.LogError("立方体数组未初始化，请先创建立方体");
            return;
        }
        int rootCount = 0;
        // 遍历所有立方体
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cubeObj = cubes[row, col];
                if (cubeObj == null) continue;
                
                Cube cube = cubeObj.GetComponent<Cube>();
                if (cube == null || !cube.isEnable) continue;
                
                // 检查是否位于边缘
                bool isEdge = row == 0 || row == rows - 1 || col == 0 || col == columns - 1;
                
                if (isEdge)
                {
                    // 设置为Root，这里假设Cube类有一个isRoot属性
                    cube.isRoot = true;
                    // 设置为特殊颜色以标识Root
                    cube.SetColor(colors[randomIndex]);
                    rootCount++;
                }
                else
                {
                    // 非边缘的Cube不是Root
                    cube.isRoot = false;
                }
            }
        }

        Debug.Log($"已将 {rootCount} 个边缘立方体设置为Root");
    }
    // PaoPaoLong检测逻辑
    public void PaoPaoLong(Cube cube)
    {
        switch (paoPaoLongMode)
        {
            case PaoPaoLongMode.DFS:
                StartCoroutine(PaoPaoLong_DFS_Animation(cube));
                break;
            case PaoPaoLongMode.BFS:
                StartCoroutine(PaoPaoLong_BFS_Animation(cube));
                break;
        }
        
    }
    IEnumerator PaoPaoLong_DFS_Animation(Cube cube)
    {
        isWorking = true;
        //以下代码:找出Cube和附近颜色相同的，大于3个就全部消失
        HashSet<Cube> disableSet = new HashSet<Cube>();
        Stack<Cube> stack = new Stack<Cube>();
        stack.Push(cube);
        while (stack.Count > 0)
        {
            Cube currentCube = stack.Pop();
            if (!disableSet.Contains(currentCube))
            {
                disableSet.Add(currentCube);
            }
            foreach (Cube neighbour in currentCube.neighbours)
            {
                if (!disableSet.Contains(neighbour) && neighbour.isEnable && neighbour.cubeColor == cube.cubeColor)
                {
                    stack.Push(neighbour);
                }
            }
        }
        if (disableSet.Count < 3)
        {
            isWorking = false;
            yield break;
        }
        foreach (Cube currentCube in disableSet)
        {
            yield return new WaitForSeconds(waitTime);
            currentCube.DisableCube();
        }
        disableSet.Clear();
        //以下代码:找出所有根，从根出发，遍历所有，不在根的支路的Cube消失
        HashSet<Cube> rootSet = new HashSet<Cube>();
        HashSet<Cube> cubesSet = new HashSet<Cube>();
        HashSet<Cube> visited = new HashSet<Cube>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Cube currentCube = cubes[row, column].GetComponent<Cube>();
                if (currentCube != null && currentCube.isEnable)
                {
                    if (currentCube.isRoot)
                    {
                        rootSet.Add(currentCube);
                    }
                    cubesSet.Add(currentCube);
                }
            }
        }
        foreach (Cube root in rootSet)
        {
            stack = new Stack<Cube>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                Cube currentCube = stack.Pop();
                if (!visited.Contains(currentCube) && currentCube.isEnable)
                {
                    visited.Add(currentCube);
                }
                foreach (Cube neighbour in currentCube.neighbours)
                {
                    if (!visited.Contains(neighbour) && neighbour.isEnable)
                    {
                        stack.Push(neighbour);
                    }
                }
            }
        }
        foreach (Cube currentCube in cubesSet)
        {
            if (!visited.Contains(currentCube))
            {
                disableSet.Add(currentCube);
            }
        }
        foreach (Cube currentCube in disableSet)
        {
            yield return new WaitForSeconds(waitTime);
            currentCube.DisableCube();
        }
        isWorking = false;
        yield break;
    }
    IEnumerator PaoPaoLong_BFS_Animation(Cube cube)
    {
        isWorking = true;
        //以下代码:找出Cube和附近颜色相同的，大于3个就全部消失
        HashSet<Cube> disableSet = new HashSet<Cube>();
        Queue<Cube> queue = new Queue<Cube>();
        queue.Enqueue(cube);
        while (queue.Count > 0)
        {
            Cube currentCube = queue.Dequeue();
            if (!disableSet.Contains(currentCube))
            {
                disableSet.Add(currentCube);
            }
            foreach (Cube neighbour in currentCube.neighbours)
            {
                if (!disableSet.Contains(neighbour) && neighbour.isEnable && neighbour.cubeColor == cube.cubeColor)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
        if (disableSet.Count < 3)
        {
            isWorking = false;
            yield break;
        }
        foreach (Cube currentCube in disableSet)
        {
            yield return new WaitForSeconds(waitTime);
            currentCube.DisableCube();
        }
        disableSet.Clear();
        //以下代码:找出所有根，从根出发，遍历所有，不在根的支路的Cube消失
        HashSet<Cube> rootSet = new HashSet<Cube>();
        HashSet<Cube> cubesSet = new HashSet<Cube>();
        HashSet<Cube> visited = new HashSet<Cube>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Cube currentCube = cubes[row, column].GetComponent<Cube>();
                if (currentCube != null && currentCube.isEnable)
                {
                    if (currentCube.isRoot)
                    {
                        rootSet.Add(currentCube);
                    }
                    cubesSet.Add(currentCube);
                }
            }
        }
        foreach (Cube root in rootSet)
        {
            queue = new Queue<Cube>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                Cube currentCube = queue.Dequeue();
                if (!visited.Contains(currentCube) && currentCube.isEnable)
                {
                    visited.Add(currentCube);
                }
                foreach (Cube neighbour in currentCube.neighbours)
                {
                    if (!visited.Contains(neighbour) && neighbour.isEnable)
                    {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }
        foreach (Cube currentCube in cubesSet)
        {
            if (!visited.Contains(currentCube))
            {
                disableSet.Add(currentCube);
            }
        }
        foreach (Cube currentCube in disableSet)
        {
            yield return new WaitForSeconds(waitTime);
            currentCube.DisableCube();
        }
        isWorking = false;
        yield break;
    }

    /*
    以下方法为DFS模式下的方法
    */
    // DFS动画
    public void DFS(Cube cube)
    {
        StartCoroutine(DFSAnimation(cube));
    }
    IEnumerator DFSAnimation(Cube cube)
    {
        isWorking = true;
        HashSet<Cube> visited = new HashSet<Cube>();
        Stack<Cube> stack = new Stack<Cube>();
        stack.Push(cube);
        while (stack.Count > 0)
        {
            Cube currentCube = stack.Pop();
            if (!visited.Contains(currentCube))
            {
                currentCube.EnableCube();
                currentCube.SetColor(CubeColor.BLUE);
                visited.Add(currentCube);
                yield return new WaitForSeconds(waitTime);
            }
            foreach (Cube neighbour in currentCube.neighbours)
            {
                if (!visited.Contains(neighbour) && !neighbour.isEnable)
                {
                    stack.Push(neighbour);
                }
            }
        }
        isWorking = false;
        yield break;
    }

    /*
    以下方法为BFS模式下的方法
    */
    // BFS动画
    public void BFS(Cube cube)
    {
        StartCoroutine(BFSAnimation(cube));
    }
    IEnumerator BFSAnimation(Cube cube)
    {
        isWorking = true;
        HashSet<Cube> visited = new HashSet<Cube>();
        Queue<Cube> queue = new Queue<Cube>();
        queue.Enqueue(cube);
        while (queue.Count > 0)
        {
            Cube currentCube = queue.Dequeue();
            if (!visited.Contains(currentCube))
            {
                currentCube.EnableCube();
                currentCube.SetColor(CubeColor.BLUE);
                visited.Add(currentCube);
                yield return new WaitForSeconds(waitTime);
            }
            foreach (Cube neighbour in currentCube.neighbours)
            {
                if (!visited.Contains(neighbour) &&!neighbour.isEnable)  
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
        isWorking = false;
        yield break;
    }
}
public enum GameMode
{
    PaoPaoLong,
    DFS,
    BFS
}
public enum PaoPaoLongMode
{
    DFS,
    BFS
}

