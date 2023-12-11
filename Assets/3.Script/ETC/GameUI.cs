using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{
    public Text playerCountText;
    public GameObject winPanel;
    public Text Win;

    [SyncVar(hook = nameof(OnGameEndedChanged))]
    private bool isGameEnded = false; // 게임 종료 여부를 나타내는 변수

    [SyncVar(hook = nameof(OnGameResultChanged))]
    private string gameResult = ""; // 게임 결과를 나타내는 변수

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
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        playerCountText.text = $"플레이어 수: {PlayerPrefs.GetInt("Player")}/{playerCount}";
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
            gameResult = "승리";
            isGameEnded = true;
        }
    }

    [ClientRpc]
    void RpcDisplayWinPanel()
    {
        Win.text = gameResult;
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 플레이어가 죽었을 때 호출됩니다.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // 플레이어 수 감소
            PlayerPrefs.SetInt("Player", playerCount); // 플레이어 수 저장
        }
    }

    // isGameEnded가 변경될 때 호출됩니다.
    void OnGameEndedChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            RpcDisplayWinPanel();
        }
    }

    // gameResult가 변경될 때 호출됩니다.
    void OnGameResultChanged(string oldValue, string newValue)
    {
        Win.text = newValue;
    }
}
