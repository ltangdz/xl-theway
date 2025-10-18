using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehavior<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = new List<RoomTemplateSO>();
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// 生成随机地牢
    /// </summary>
    /// <param name="currentDungeonLevel"></param>
    /// <returns></returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        
        // Load the Scriptable Object room templates into the Dictionary
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttemmpts)
        {
            dungeonBuildAttempts++;
            
            // 选择随机RoomNodeGraph
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);
            // Debug.Log($"第{dungeonBuildAttempts}次尝试: {roomNodeGraph.name}");

            dungeonBuildSuccessful = false;
            int dungeonRebuildAttemptsForNodeGraph = 0;
            
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph < Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                // Clear dungeon room gameObjects and dungeon room dictionary
                ClearDungeon();
                dungeonRebuildAttemptsForNodeGraph++;
                
                // 对于选中的Graph，尝试构建一个随机地牢
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);

                if (dungeonBuildSuccessful)
                {
                    // 实例化房间
                    InstantiateRoomGameObjects();
                }
            }
        }
        return dungeonBuildSuccessful;
    }
    
    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate room template Key in " + roomTemplateList);
            }
        }
    }
    
    /// <summary>
    /// 尝试以具体的roomNodeGraph构建随机地牢
    /// </summary>
    /// <param name="roomNodeGraph"></param>
    /// <returns></returns>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;
        }
        
        // start with no room overlaps
        bool noRoomOverlaps = true;
        
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // 房间队列为空 且 没有房间重叠 则地牢构建成功
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
                
                room.isPositioned = true;
                
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                // get parent room for node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // Debug.Log("Room Node Type: " + roomNode.roomNodeType.roomNodeTypeName + ", parent: " + parentRoom.roomNodeType.roomNodeTypeName);
             
                // see if room can be placed without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }
        
        return noRoomOverlaps;
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;
        while (roomOverlaps) 
        {
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                return false;
            }
            
            // 在未连接的门道列表中随机选取一个作为父门道
            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);
            
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // 尝试不重叠放置房间 成功返回true
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;
                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取与父doorway对应的随机房间模板
    /// eg: 父doorway在房间的东边 return: 与自身type对应的随机房间模板
    ///     父doorway在走廊的南边 return: 随机corridorEW
    /// </summary>
    /// <param name="roomNode">自身节点</param>
    /// <param name="doorwayParent">父门道</param>
    /// <returns></returns>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;
        
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x=>x.isCorridorEW));
                    break;
                
                case Orientation.none:
                    break;
                
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }
        
        return roomTemplate;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        if (doorway == null)
        {
            doorwayParent.isUnavailable = true;
            return false;
        }
        
        // 计算父门道的世界坐标
        // parentDoorwayPosition: 世界坐标  doorwayParent.position: 相对坐标
        Vector2Int parentDoorwayPosition =
            parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;
        
        // 与父门道的偏移调整
        // eg: 父门道朝北 门道朝南 adjustment.y += -1
        //     父门道朝西 门道朝东 adjustment.x += 1
        Vector2Int adjustment = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            
            case Orientation.none:
                break;
            
            default:
                break;
        }
        
        // 计算房间的世界坐标（上下限）
        // parentDoorwayPosition + adjustment = doorway.position(在模板中的相对坐标) + room.lowerBounds - room.templateLowerBounds
        
        // 25.6.27 我犯了一个错误
        // parentDoorwayPosition: 世界坐标  doorwayParent.position: 相对坐标
        // room.lowerBounds = doorwayParent.position + adjustment + room.templateLowerBounds - doorway.position;
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;
        
        // 检查是否与其他房间重叠
        Room overLappingRoom = CheckForRoomOverlap(room);

        if (overLappingRoom == null)
        {
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;
            
            doorway.isConnected = true;
            doorway.isUnavailable = true;
            // Debug.Log($"Room {room.roomNodeType.roomNodeTypeName} 没有重叠并成功放置");
            return true;
        }
        else
        {
            doorwayParent.isUnavailable = true;
            // Debug.Log($"Room {room.roomNodeType.roomNodeTypeName} 放置失败");
            return false;
        }
    }

    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> roomDoorwayList)
    {
        foreach (Doorway doorwayToCheck in roomDoorwayList)
        {
            if (doorwayParent.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west ||
                doorwayParent.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east ||
                doorwayParent.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north ||
                doorwayParent.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
        }
        
        return null;
    }

    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // 遍历所有房间进行检查
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            
            // 跳过自己以及未放置的房间
            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue;
            }
            
            // 如果产生重叠
            if (IsOverlappingRoom(roomToTest, room))
            {
                return room;
            }
        }
        
        // 无重叠(理想情况)
        return null;
    }

    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        // x轴和y轴都发生重叠 我们认为发生重叠
        return IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x) &&
            IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);
    }

    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            // Overlap!
            return true;
        }
        else
        {
            return false;
        }
    }

    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
        {
            return null;
        }
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }
    
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> parentRoomDoorwayList)
    {
        foreach (Doorway doorway in parentRoomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        // 深拷贝 而不是简单复制
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
            
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
            
            
        }
        
        return room;
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("No room node graph in list");
            return null;
        }
    }
    
    private List<string> CopyStringList(List<string> roomNodeChildRoomNodeIDList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in roomNodeChildRoomNodeIDList)
        {
            newStringList.Add(stringValue);
        }
        
        return newStringList;
    }
    
    
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();
            
            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            
            newDoorwayList.Add(newDoorway);
        }
        
        return newDoorwayList;
    }

    private void InstantiateRoomGameObjects()
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            
            // 计算房间坐标
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x,
                room.lowerBounds.y - room.templateLowerBounds.y, 0f);
            
            // 实例化房间
            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);
            
            // 获取InstantiateRoom组件
            InstantiateRoom instantiateRoom = roomGameObject.GetComponentInChildren<InstantiateRoom>();
            instantiateRoom.room = room;
            
            // 初始化
            instantiateRoom.Initialize(roomGameObject);
            
            // 保存gameObject引用
            room.instantiateRoom = instantiateRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        return null;
    }

    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        return null;
    }
    
    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if (room.instantiateRoom)
                {
                    Destroy(room.instantiateRoom.gameObject);
                }
            }
            
            dungeonBuilderRoomDictionary.Clear();
        }
    }

}
