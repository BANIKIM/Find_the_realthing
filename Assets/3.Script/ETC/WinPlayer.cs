using UnityEngine;
using Mirror;
using System.Collections.Generic;


public class CustomNetworkManager : NetworkManager
{
    private List<PlayerStatus> playerStatusList = new List<PlayerStatus>();


    public void CheckAllPlayersDead()
    {
        bool allPlayersDead = true;
        foreach (PlayerStatus playerStatus in playerStatusList)
        {
            if (!playerStatus.isDie)
            {
                allPlayersDead = false;
                break;
            }
        }

        if (allPlayersDead)
        {
            Debug.Log("게임 종료 - 모든 플레이어 사망");

            // 게임 종료 로직 또는 패배 조건 처리
            // 예를 들어:
            // LoseGame();
        }
    }

    public void PlayerDied(NetworkConnection conn)
    {
        PlayerStatus playerToKill = playerStatusList.Find(player => player.connection == conn);
        playerToKill.isDie = true;

        CheckAllPlayersDead();
    }
}

public class PlayerStatus
{
    public NetworkConnection connection;
    public bool isDie;

    public PlayerStatus(NetworkConnection connection)
    {
        this.connection = connection;
        isDie = false;
    }
}
