using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class CreateGameRoomData
{
    public int maxPlaayerCount;
}
public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private List<Button> maxPlayerCountBtn;

    private CreateGameRoomData roomData;

    private void Start()
    {
        roomData = new CreateGameRoomData() { maxPlaayerCount = 4 };

      
    }
    public void UpdateMaxPlayerCount(int count)
    {
        roomData.maxPlaayerCount = count;
    }

    public void CreateRoom()
    {
        var manager = RoomManager.singleton;

        // 현재 플레이어 수가 maxPlayerCount보다 작은 경우에만 입장
        if (NetworkServer.connections.Count < roomData.maxPlaayerCount)
        {
            // 서버를 여는 동시에 클라이언트로 참가 
            manager.StartHost();
        }
       
    }
}
