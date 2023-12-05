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

        // ���� �÷��̾� ���� maxPlayerCount���� ���� ��쿡�� ����
        if (NetworkServer.connections.Count < roomData.maxPlaayerCount)
        {
            // ������ ���� ���ÿ� Ŭ���̾�Ʈ�� ���� 
            manager.StartHost();
        }
       
    }
}
