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

    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    private int playerCount;
    private bool isGameEnded = false; // 게임 종료 여부를 나타내는 변수

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

       /* if (isGameEnded && Input.anyKeyDown) // 게임 종료 후 키 입력 시 새로운 씬으로 이동
        {
            SceneManager.LoadScene("GameRoom2");
        }*/
    }

    /*[ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        playerCountText.text = $"플레이어 수: {playerCount}";
    }*/

    [Server]
    void UpdatePlayerCount()
    {
        // 네트워크 연결된 플레이어의 수를 가져옵니다.
        playerCount = NetworkServer.connections.Count;
        //RpcUpdatePlayerCount(playerCount); // 클라이언트에 플레이어 수 업데이트를 RPC로 전달
    }

    [Server]
    void SetPlayer()
    {
        playerCount = NetworkServer.connections.Count;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isGameEnded = true; // 게임 종료 상태로 설정
    }

    // 플레이어가 죽었을 때 호출됩니다.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // 플레이어 수 감소
            RpcUpdatePlayerCount(playerCount);
            PlayerPrefs.SetInt("Player", playerCount); // 플레이어 수 저장

            // 플레이어 수 감소 후, 승리 조건 확인
        }
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int count)
    {
        // 플레이어 수 업데이트를 받으면 로컬에서도 UI를 업데이트
        OnPlayerCountChanged(playerCount, count);
    }

    private void OnPlayerCountChanged(int oldCount, int newCount)
    {
        playerCountText.text = $"플레이어 수: {newCount}";
    }
}
