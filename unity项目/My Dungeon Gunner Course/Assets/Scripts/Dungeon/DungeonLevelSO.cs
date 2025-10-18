using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// levelName: 关卡名称
/// roomTemplateList: 房间模板列表
/// roomNodeGraphList: 房间图列表 根据房间模板生成关卡
/// </summary>
[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip
    
    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)] [Header("ROOM TEMPLATES FOR LEVEL")]
    #endregion Header ROOM TEMPLATES FOR LEVEL
    #region Tooltip
    [Tooltip("Populate the list with the room templates that you want to be part of the level. You need to ensure that room templates are included for all room node types that are specified in the Room Node Graphs for the level")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion Header ROOM NODE GRAPHS FOR LEVEL
    #region Tooltip
    [Tooltip("Populate this list with the room node graphs which should be randomly selected from the level")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }
        
        // 检查房间模板对应房间图的所有节点类型
        
        // 首先检查north/south corridor, east/west corridor and entrance种类被定义
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;
        
        // 遍历所有房间模板检查节点类型被定义
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (!roomTemplateSO)
            {
                return;
            }

            if (roomTemplateSO.roomNodeType.isCorridorEW)
            {
                isEWCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isCorridorNS)
            {
                isNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }

        if (!isEWCorridor)
        {
            // 该等级的关卡不存在东西走廊
            Debug.Log(("In " + this.name.ToString() + " : No E/W Corridor Room Type Specified"));
        }

        if (!isNSCorridor)
        {
            Debug.Log(("In " + this.name.ToString() + " : No N/S Corridor Room Type Specified"));
        }

        if (!isEntrance)
        {
            Debug.Log(("In " + this.name.ToString() + " : No Entrance Room Type Specified"));
        }
        
        // 遍历所有房间图
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (!roomNodeGraph)
            {
                return;
            }

            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (!roomNodeSO)
                {
                    continue;
                }
                
                // 检查房间模板定义每个节点类型
                // CorridorNS/EW Entrance 已经被检查过
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW ||
                    roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor
                    || roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isNone)
                {
                    continue;
                }
                bool isRoomNodeTypeFound = false;
                
                // 遍历所有房间模板来检查该节点类型被定义
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (!roomTemplateSO)
                    {
                        continue;
                    }

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType) // 找到了
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + " No room template " +
                              roomNodeSO.roomNodeType.name.ToString() + " found for node graph " +
                              roomNodeGraph.name.ToString());
                }
            }
        }
    }
#endif 
    #endregion Validation
}
