using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    // 간격 조절을 위한 변수
    public float spawnInterval = 3f;

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        // 새로운 플레이어 생성
        var player = Instantiate(spawnPrefabs[0]);

        // 플레이어의 위치를 이동시켜 x 축 간격을 둠
        player.transform.position = new Vector3(GetNextSpawnX(), 0f, 0f);

        // 플레이어를 해당 연결에 소환
        NetworkServer.Spawn(player, conn);
    }

    // 다음 소환 위치의 x 값을 계산하는 함수
    private float GetNextSpawnX()
    {
        // 현재 소환된 플레이어 수에 따라서 x 값을 계산
        int playerCount = NetworkServer.connections.Count;
        return playerCount * spawnInterval;
    }
}
