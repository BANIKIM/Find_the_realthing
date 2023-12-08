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
            UpdatePlayerCount(); // �÷��̾� ���� ����
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
        playerCountText.text = $"�÷��̾� ��: {playerCount}/4";
    }

    [Server]
    void UpdatePlayerCount()
    {
        // ��Ʈ��ũ ����� �÷��̾��� ���� �����ɴϴ�.
        int playerCount = NetworkServer.connections.Count;
        RpcUpdatePlayerCount(playerCount); // Ŭ���̾�Ʈ�� �÷��̾� �� ������Ʈ�� RPC�� ����
    }

    [Server]
    void SetPlayer()
    {
        // ���� �÷��̾� ���� �����մϴ�.
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

    // �÷��̾ �׾��� �� ȣ��˴ϴ�.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // �÷��̾� �� ����
            PlayerPrefs.SetInt("Player", playerCount); // �÷��̾� �� ����

            // �÷��̾� �� ���� ��, �¸� ���� Ȯ��
            CheckWinCondition();
        }
    }
}
