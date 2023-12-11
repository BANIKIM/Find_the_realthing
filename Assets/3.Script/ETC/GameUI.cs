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
    public GameObject losePanel;
    private GameObject TimePnal;
    public Text survivalTimeText;
    public Text survivalTimeText2;
    public bool isDie = false;
    private float surTime = 0f;
    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    private int playerCount;
    public bool isGameEnded = false; // ���� ���� ���θ� ��Ÿ���� ����

    private void Start()
    {
        winPanel = transform.GetChild(1).gameObject;
        losePanel = transform.GetChild(2).gameObject;
        TimePnal = transform.GetChild(3).gameObject;
        SetPlayer();
        StartCoroutine(UpdateTime());
    }

    void Update()
    {
        if (isServer)
        {
            UpdatePlayerCount(); // �÷��̾� ���� ����
            CheckWinCondition();
        }


    }


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

            if (isDie)
            {
                losePanel.SetActive(true);
            }
        }

    }

    [ClientRpc]
    void RpcDisplayWinPanel()
    {


        if (!isDie)
        {
            winPanel.SetActive(true);
        }
        TimePnal.SetActive(false);
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
    IEnumerator UpdateTime()
    {
        while (!isGameEnded)
        {
            surTime += Time.deltaTime;
            survivalTimeText.text = $"���� �ð�: {Mathf.Floor(surTime)}��";
            survivalTimeText2.text = $"���� �ð�: {Mathf.Floor(surTime)}��";
            yield return null;

        }
    }
}
