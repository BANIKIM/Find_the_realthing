using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : NetworkBehaviour
{
    public Text playerCountText;
    public GameObject Win_Loser;

    private GameObject TimePnal;
    public Text survivalTimeText;
    public Text survivalTimeText2;
    public Text survivalTimeText3;
    public bool isDie_ser = false;
    public bool isDie_clr = false;
    private float winsurTime = 0f;
    private float losesurTiem = 0f;
    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    private int playerCount;
    public bool isGameEnded = false; // ���� ���� ���θ� ��Ÿ���� ����

    private void Start()
    {
        Win_Loser = transform.GetChild(1).gameObject;
        TimePnal = transform.GetChild(2).gameObject;
        SetPlayer();
        StartCoroutine(UpdateTime());
        StartCoroutine(UpdateTime2());
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
        }
    }

    [ClientRpc]
    void RpcDisplayWinPanel()
    {
        Win_Loser.SetActive(true);
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
            losesurTiem = Time.timeScale = 1;
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
            winsurTime += Time.deltaTime;
            survivalTimeText.text = $"���� �ð�: {Mathf.Floor(winsurTime)}��";
            survivalTimeText2.text = $"���� �ð�: {Mathf.Floor(winsurTime)}��";
          
            yield return null;

        }
    }
    IEnumerator UpdateTime2()
    {
        while (!isGameEnded)
        {
            losesurTiem += Time.deltaTime;
            survivalTimeText3.text =$"���� �ð�: {Mathf.Floor(losesurTiem)}��";

            yield return null;

        }
    }
}
