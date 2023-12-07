using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    // ���� ������ ���� ����
    public float spawnInterval = 3f;

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        // ���ο� �÷��̾� ����
        var player = Instantiate(spawnPrefabs[0]);

        // �÷��̾��� ��ġ�� �̵����� x �� ������ ��
        player.transform.position = new Vector3(GetNextSpawnX(), 0f, 0f);

        // �÷��̾ �ش� ���ῡ ��ȯ
        NetworkServer.Spawn(player, conn);
    }

    // ���� ��ȯ ��ġ�� x ���� ����ϴ� �Լ�
    private float GetNextSpawnX()
    {
        // ���� ��ȯ�� �÷��̾� ���� ���� x ���� ���
        int playerCount = NetworkServer.connections.Count;
        return playerCount * spawnInterval;
    }
}
