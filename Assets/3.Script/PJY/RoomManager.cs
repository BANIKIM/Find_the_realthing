using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    // 간격 조절을 위한 변수
    public float spawnInterval = 2f; // 적절한 간격으로 조절하세요.
    private float currentX = 0f;

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
        currentX += spawnInterval; // 현재 위치에 간격을 추가합니다.
        return currentX;
    }
}
