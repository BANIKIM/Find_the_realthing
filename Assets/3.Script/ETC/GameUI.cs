using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{
    public Text playerCountText;
    public GameObject winPanel;

    [SyncVar]
    private int winPlayerCount;

    private void Start()
    {
        if (isClient)
        {
            winPanel = transform.GetChild(1).gameObject;
        }
    }

    void Update()
    {
        if (isServer)
        {
            PlayerCount(); // 플레이어 숫자 확인
            WinPlayer();
        }
    }

    private void PlayerCount()
    {
        // 현재 서버에 접속 중인 플레이어 수를 가져와서 텍스트로 표시
        if (NetworkServer.active)
        {
            int playerCount = NetworkServer.connections.Count;
            winPlayerCount = playerCount;
            playerCountText.text = $"플레이어 수: {playerCount}/4";
        }
        else
        {
            playerCountText.text = "서버가 실행 중이 아닙니다.";
        }
    }

    private void WinPlayer()
    {
        if (winPlayerCount == 1)
        {
            RpcSetActiveWinPanel(true);
        }
    }

    [ClientRpc]
    void RpcSetActiveWinPanel(bool setActive)
    {
        if (isClient)
        {
            winPanel.SetActive(setActive);
        }
    }
}
