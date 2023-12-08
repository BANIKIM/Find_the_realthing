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
            PlayerCount(); // �÷��̾� ���� Ȯ��
            WinPlayer();
        }
    }

    private void PlayerCount()
    {
        // ���� ������ ���� ���� �÷��̾� ���� �����ͼ� �ؽ�Ʈ�� ǥ��
        if (NetworkServer.active)
        {
            int playerCount = NetworkServer.connections.Count;
            winPlayerCount = playerCount;
            playerCountText.text = $"�÷��̾� ��: {playerCount}/4";
        }
        else
        {
            playerCountText.text = "������ ���� ���� �ƴմϴ�.";
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
