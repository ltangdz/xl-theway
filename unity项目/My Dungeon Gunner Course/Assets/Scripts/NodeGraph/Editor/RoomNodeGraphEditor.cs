using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;
    
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    
    // Node Layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    
    // Connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // Grid Spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;
    
    // Alt+1快捷键
    // To create a hotkey, use the following special characters:
    // %: Represents Ctrl on Windows and Linux. Cmd on macOS.
    // ^: Represents Ctrl on Windows, Linux, and macOS.
    // #: Represents Shift.
    // &: Represents Alt.
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor &1")]
    private static void OpenWindow()
    {
        // 打开名字为Room Node Graph Editor的窗口
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        
        // Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        // Define selected node layout style
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        
        // Load Room Node Types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// 处理双击RoomNodeGraphSO的逻辑
    /// </summary>
    /// <param name="instanceID">GUI id</param>
    /// <param name="line">文件路径</param>
    /// <returns>true: 用户处理 false: unity处理</returns>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        
        if (roomNodeGraph)
        {
            OpenWindow();
            
            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        // If a scriptable object of type RoomNodeGraphSO has been selected then process
        if (currentRoomNodeGraph)
        {
            // Draw Grid
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);
            
            // Draw line if being dragged
            // 在DrawRoomNodes之前调用 保证线画在节点后面
            DrawDraggedLine();
            
            // Process Events
            ProcessEvents(Event.current);
            
            // Draw Connections Between Room Nodes
            DrawRoomConnections();

            // Draw Room Nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt(position.width + gridSize / gridSize);
        int horizontalLineCount = Mathf.CeilToInt(position.height + gridSize / gridSize);
        
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;
        
        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0f);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + gridOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0f) + gridOffset,
                new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }
        
        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            // Draw line from node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // 重置拖拽偏移
        graphDrag = Vector2.zero;
        
        // 当前节点为空 或者 没有被左键拖动
        if (!currentRoomNode || currentRoomNode.isLeftClickDragging == false)
        {
            // 获取当前房间节点
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // 鼠标上没有节点 或者有起始连线节点 则处理图事件
        if (!currentRoomNode || currentRoomNodeGraph.roomNodeToDrawLineFrom)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        // 鼠标上有节点 则处理节点事件
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// 判断鼠标是否在节点上
    /// </summary>
    /// <param name="currentEvent">no use</param>
    /// <returns>true:返回覆盖的节点 false:返回null</returns>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        // 遍历所有房间节点 如果鼠标的position在任意一个房间节点内 则返回覆盖的节点
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null; 
    }

    /// <summary>
    /// Process Room Node Graph Events
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            
            // Process Mouse Drag Event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    /// <summary>
    /// Process mouse down on the room node graph (not over a node)
    /// 右键：打开菜单 创建新节点
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // 点击左键清除所有选中 并清除拖拽的线
        if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                
                GUI.changed = true;
            }
        }

    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    /// <param name="mousePosition"></param>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        // 按下菜单选项后，调用一次CreateRoomNode方法创建新节点
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        
        // 选择所有节点
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");

        // 删除选中连接
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        // 删除选中节点
        menu.AddItem(new GUIContent("Delete Selected Room Node"), false, DeleteSelectedRoomNode);
        
        menu.ShowAsContext();
    }
    
    /// <summary>
    /// 在鼠标位置创建一个房间节点RoomNode -- 创建新节点时调用
    /// 如果还没有节点被创建 则自动创建一个入口节点
    /// </summary>
    /// <param name="mousePositionObject"></param>
    private void CreateRoomNode(object mousePositionObject)
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// 在鼠标位置创建一个房间节点RoomNode，并把该Node加入roomNodeList -- 传递RoomNodeTypeSO的重载
    /// </summary>
    /// <param name="mousePositionObject"></param>
    /// <param name="roomNodeType"></param>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        
        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        
        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        // set room node values (位置 大小 属于哪张图 房间节点类型)
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);
        
        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        
        AssetDatabase.SaveAssets();
        
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// 选中所有节点
    /// </summary>
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }    
        GUI.changed = true;
    }
    
    /// <summary>
    /// 删除所有选中连接(删除节点的子节点 以及 子节点的父节点)
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        // 遍历所有节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // 如果节点被选中 且节点存在子节点
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                // 遍历所有子节点 如果子节点也被选中
                for (int i = 0; i < roomNode.childRoomNodeIDList.Count; i++)
                {
                    string childRoomNodeID = roomNode.childRoomNodeIDList[i];
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode && childRoomNode.isSelected)
                    {
                        // 删除节点的子节点 以及子节点的父节点
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 删除选中节点(删除节点 删除父节点的子节点 删除子节点的父节点)
    /// Entrance不能删除
    /// </summary>
    private void DeleteSelectedRoomNode()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        
        // 遍历所有节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // 如果节点被选中
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                // 把节点加入删除队列
                roomNodeDeletionQueue.Enqueue(roomNode);
                
                // 删除父节点的子节点
                for (int i = 0; i < roomNode.parentRoomNodeIDList.Count; i++)
                {
                    string parentRoomNodeID = roomNode.parentRoomNodeIDList[i];
                    currentRoomNodeGraph.GetRoomNode(parentRoomNodeID)?.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                }

                // 删除子节点的父节点
                for (int i = 0; i < roomNode.childRoomNodeIDList.Count; i++)
                {
                    string childRoomNodeID = roomNode.childRoomNodeIDList[i];
                    currentRoomNodeGraph.GetRoomNode(childRoomNodeID)?.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                }
            }
        }
        
        // 遍历队列 删除所有节点
        while (roomNodeDeletionQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            
            // 从字典删除id
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            
            // 从列表删除节点
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            
            // 在资源管理器中删除
            DestroyImmediate(roomNodeToDelete, true);
            
            // 保存更改
            AssetDatabase.SaveAssets();
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // 当前为右键 且 有创建连线
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom)
        {
            // 鼠标有节点
            if (currentRoomNode)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(currentRoomNode.id))
                {
                    currentRoomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        
        GUI.changed = true;
    }
    
    private void DrawRoomConnections()
    {
        // 遍历所有房间节点
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // 如果节点的子房间数>0
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // 遍历子房间id
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // 如果房间图的节点字典包含子房间id的键
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        // 从起点画到终点
        Vector2 startPos = parentRoomNode.rect.center;
        Vector2 endPos = childRoomNode.rect.center;
        Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, connectingLineWidth);
        
        // 在中点处画出箭头
        Vector2 midPos = (endPos + startPos) / 2f;
        Vector2 direction = (endPos - startPos).normalized;
        Vector2 arrHeadPoint = midPos + direction * connectingLineArrowSize;
        Vector2 arrTailPoint1 = midPos + new Vector2(-direction.y, direction.x) * connectingLineArrowSize;
        Vector2 arrTailPoint2 = midPos - new Vector2(-direction.y, direction.x) * connectingLineArrowSize;
        Handles.DrawBezier(arrHeadPoint, arrTailPoint1, arrHeadPoint, arrTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrHeadPoint, arrTailPoint2, arrHeadPoint, arrTailPoint2, Color.white, null, connectingLineWidth);
        
        GUI.changed = true;
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // 鼠标左键拖拽画布
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);   
        }
        // 鼠标右键拖拽连线
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }
        
        GUI.changed = true;
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// 在RoomNodeGraph画出RoomNodes
    /// </summary>
    private void DrawRoomNodes()
    {
        // 遍历所有RoomNodes然后画出来
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }
        
        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}


