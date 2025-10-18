using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(InstantiateRoom))]
[DisallowMultipleComponent]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiateRoom instantiateRoom;

    private void Awake()
    {
        instantiateRoom = GetComponent<InstantiateRoom>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }
    
    // 当房间发生更改 更改后的房间为自己 且 自己没有被点亮过
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (roomChangedEventArgs.room == instantiateRoom.room && !instantiateRoom.room.isLit)
        {
            // Fade in room
            FadeInRoomLighting();
            
            // Fade in doors
            FadeInDoors();
            
            instantiateRoom.room.isLit = true;
        }
    }

    private void FadeInRoomLighting()
    {
        StartCoroutine(FadeInRoomLightingRoutine(instantiateRoom));
    }

    private IEnumerator FadeInRoomLightingRoutine(InstantiateRoom instantiateRoom1)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);
        
        instantiateRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;
        
        for (float i = 0.05f; i <= 1f; i += Time.fixedDeltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Silder", i);
            
            yield return null;
        }
        
        instantiateRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;

    }

    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();
            
            doorLightingControl.FadeInDoor(door);
        }
    }

}
