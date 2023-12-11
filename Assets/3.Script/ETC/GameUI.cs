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
    private bool isGameEnded = false; // ���� ���� ���θ� ��Ÿ���� ����

    [SyncVar(hook = nameof(OnGameResultChanged))]
    private string gameResult = ""; // ���� ����� ��Ÿ���� ����

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
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        playerCountText.text = $"�÷��̾� ��: {PlayerPrefs.GetInt("Player")}/{playerCount}";
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
            gameResult = "�¸�";
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

    // �÷��̾ �׾��� �� ȣ��˴ϴ�.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // �÷��̾� �� ����
            PlayerPrefs.SetInt("Player", playerCount); // �÷��̾� �� ����
        }
    }

    // isGameEnded�� ����� �� ȣ��˴ϴ�.
    void OnGameEndedChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            RpcDisplayWinPanel();
        }
    }

    // gameResult�� ����� �� ȣ��˴ϴ�.
    void OnGameResultChanged(string oldValue, string newValue)
    {
        Win.text = newValue;
    }
}
