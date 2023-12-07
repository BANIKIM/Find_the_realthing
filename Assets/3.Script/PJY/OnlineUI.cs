using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OnlineUI : MonoBehaviour
{
    [SerializeField]
    private InputField nicknameInputField;
    [SerializeField]
    private InputField ipInputField; // 새로 추가된 InputField
    [SerializeField]
    private GameObject creatRoomUI;

    public void onClickCreateRoomButton()
    {
        if (nicknameInputField.text != "")
        {
            PlayerSettings.ninkname = nicknameInputField.text;
            creatRoomUI.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }

    public void OnClickEnterGameRoomButton()
    {
        if (nicknameInputField.text != "")
        {
            // 사용자로부터 입력 받은 IP 주소로 RoomManager의 네트워크 주소 설정
            RoomManager.singleton.networkAddress = ipInputField.text;
            RoomManager.singleton.StartClient();
        }
        else
        {
            nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }
}
