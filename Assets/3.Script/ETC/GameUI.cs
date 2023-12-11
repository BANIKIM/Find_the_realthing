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
    private bool isGameEnded = false; // ���� ���� ���θ� ��Ÿ���� ����

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

       /* if (isGameEnded && Input.anyKeyDown) // ���� ���� �� Ű �Է� �� ���ο� ������ �̵�
        {
            SceneManager.LoadScene("GameRoom2");
        }*/
    }

    /*[ClientRpc]
    void RpcUpdatePlayerCount(int playerCount)
    {
        playerCountText.text = $"�÷��̾� ��: {playerCount}";
    }*/

    [Server]
    void UpdatePlayerCount()
    {
        // ��Ʈ��ũ ����� �÷��̾��� ���� �����ɴϴ�.
        playerCount = NetworkServer.connections.Count;
        //RpcUpdatePlayerCount(playerCount); // Ŭ���̾�Ʈ�� �÷��̾� �� ������Ʈ�� RPC�� ����
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
        isGameEnded = true; // ���� ���� ���·� ����
    }

    // �÷��̾ �׾��� �� ȣ��˴ϴ�.
    public void OnPlayerDie()
    {
        if (isServer)
        {
            int playerCount = PlayerPrefs.GetInt("Player");
            playerCount--; // �÷��̾� �� ����
            RpcUpdatePlayerCount(playerCount);
            PlayerPrefs.SetInt("Player", playerCount); // �÷��̾� �� ����

            // �÷��̾� �� ���� ��, �¸� ���� Ȯ��
        }
    }

    [ClientRpc]
    void RpcUpdatePlayerCount(int count)
    {
        // �÷��̾� �� ������Ʈ�� ������ ���ÿ����� UI�� ������Ʈ
        OnPlayerCountChanged(playerCount, count);
    }

    private void OnPlayerCountChanged(int oldCount, int newCount)
    {
        playerCountText.text = $"�÷��̾� ��: {newCount}";
    }
}
