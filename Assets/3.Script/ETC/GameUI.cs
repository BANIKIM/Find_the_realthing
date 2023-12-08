using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : NetworkBehaviour
{
    public Text playerCountText;
    public GameObject winPanel;

    private void Start()
    {
        winPanel = transform.GetChild(1).gameObject;
        SetPlayer();
    }

    void Update()
    {
        if (isServer)
        {
            UpdatePlayerCount(); // 플레이어 숫자 갱신
            CheckWinCondition();
        }

        if (winPanel.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        playerCountText.text = $"플레이어 수: {playerCount}/4";
    }

    [Server]
    void UpdatePlayerCount()
    {
        // 네트워크 연결된 플레이어의 수를 가져옵니다.
        int playerCount = NetworkServer.connections.Count;
        RpcUpdatePlayerCount(playerCount); // 클라이언트에 플레이어 수 업데이트를 RPC로 전달
    }

    [Server]
    void SetPlayer()
    {
        // 최초 플레이어 수를 설정합니다.
        PlayerPrefs.SetInt("Player", NetworkServer.connections.Count);
    }

    [Server]
    void CheckWinCondition()
    {
        if (PlayerPrefs.GetInt("Player") == 1)
        {
            RpcDisplayWinPanel();
        }
    }

    [ClientRpc]
    void RpcDisplayWinPanel()
    {
        winPanel.SetActive(true);
    }

    // 플레이어가 죽었을 때 호출됩니다.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // 플레이어 수 감소
            PlayerPrefs.SetInt("Player", playerCount); // 플레이어 수 저장

            // 플레이어 수 감소 후, 승리 조건 확인
            CheckWinCondition();
        }
    }
}
