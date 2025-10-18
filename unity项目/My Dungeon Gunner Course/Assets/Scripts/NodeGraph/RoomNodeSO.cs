using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();    // 1个父节点
    public List<string> childRoomNodeIDList = new List<string>();     // n个子节点
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    
    public void Initialize(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;
        
        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle roomNodeStyle)
    {
        // Draw Node Box using Begin Area
        GUILayout.BeginArea(rect, roomNodeStyle);
        
        // Start Region to detect Popup selection changes
        EditorGUI.BeginChangeCheck();
        
        // 如果节点有父节点 或者 节点为入口节点
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // 锁定菜单并展示为标签
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        
            // 设置下拉菜单选项
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];
            
            // 删除父节点后，节点可以更改类型，如果更改类型不合法，需要断开连接
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor
                || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor
                || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // 如果节点存在子节点
                if (childRoomNodeIDList.Count > 0)
                {
                    // 遍历所有子节点
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        string childRoomNodeID = childRoomNodeIDList[i];
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeID);
                        if (childRoomNode)
                        {
                            // 删除节点的子节点 以及子节点的父节点
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        
        GUILayout.EndArea();
    }

    private string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        
        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // 鼠标左键点击: 切换被选中
            // 鼠标右键点击: 创建连线
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            
            // 鼠标左键释放: 取消拖拽
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            
            // 鼠标拖拽: 拖拽
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
        
    }
    
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // 鼠标左键按下
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent(currentEvent);
        }

        // 鼠标右键按下
        if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDownEvent(Event currentEvent)
    {
        // 更改资源管理器中选中的节点
        Selection.activeObject = this;

        // 切换isSelected
        isSelected = !isSelected;
    }
    
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // 鼠标左键释放
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent(currentEvent);
        }
    }

    private void ProcessLeftClickUpEvent(Event currentEvent)
    {
        // 如果在拖拽 则把拖拽设置为false
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // 鼠标左键拖拽
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        // currentEvent.delta: 记录鼠标与上次事件相比的相对移动
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// 为房间节点添加子节点
    /// 需要进行验证
    /// </summary>
    /// <param name="childID"></param>
    /// <returns></returns>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // 检查节点是否能加入到父节点
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    private bool IsChildRoomValid(string childID)
    {
        // 1. 每个节点不能有超过1个父节点，每关只能有一个BossRoom
        // 2. 子节点不能为none
        // 3. childID不能重复
        // 4. 节点不会连接到自身(节点不会为自己的子节点)
        // 5. 子节点不为自己的父节点
        // 6. 子节点不存在父节点
        // 7. 走廊不和走廊相连
        // 8. 房间不和房间相连
        // 9. 子节点为走廊，走廊数量小于最大数量
        // 10. 子节点不为入口
        // 11. 子节点为房间，节点不能有子节点(corridor相连的两个房间分别为父子节点)
        bool isConnectedBossNodeAlready = false;
        // 遍历所有房间节点
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            // 如果一个房间是BossRoom 且 拥有父节点
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }

        // 如果该节点是BossRoom 且 图上已经连接BossRoom 则返回false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            return false;
        }

        // 如果该节点是None 则返回false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }
        
        // 子节点id不能重复
        if (childRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        
        // 节点不能连接到自身
        if (childID == this.id)
        {
            return false;
        }
        
        // 节点不为父节点
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        
        // 节点不存在父节点
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }
        
        // 走廊不和走廊相连
        if (roomNodeType.isCorridor && roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor)
        {
            return false;
        }
        
        // 房间不和房间相连
        if (!roomNodeType.isCorridor && !roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor)
        {
            return false;
        }
        
        // 走廊数量小于最大数量
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor &&
            childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }
        
        // 子节点不为入口
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }
        
        // 子节点为房间，节点不能有子节点(corridor相连的两个房间分别为父子节点)
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }

        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code


}
