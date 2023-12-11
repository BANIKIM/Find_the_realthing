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
    public bool isGameEnded = false; // 게임 종료 여부를 나타내는 변수

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
            UpdatePlayerCount(); // 플레이어 숫자 갱신
            CheckWinCondition();
        }
    }


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
        Win_Loser.SetActive(true);
        TimePnal.SetActive(false);
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
            losesurTiem = Time.timeScale = 1;
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
    IEnumerator UpdateTime()
    {
        while (!isGameEnded)
        {
            winsurTime += Time.deltaTime;
            survivalTimeText.text = $"생존 시간: {Mathf.Floor(winsurTime)}초";
            survivalTimeText2.text = $"생존 시간: {Mathf.Floor(winsurTime)}초";
          
            yield return null;

        }
    }
    IEnumerator UpdateTime2()
    {
        while (!isGameEnded)
        {
            losesurTiem += Time.deltaTime;
            survivalTimeText3.text =$"생존 시간: {Mathf.Floor(losesurTiem)}초";

            yield return null;

        }
    }
}
