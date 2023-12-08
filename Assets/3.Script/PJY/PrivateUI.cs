using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PrivateUI : MonoBehaviour
{
    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private InputField ip;
    public void OnClickEnterGameRoomButton()
    {
        if (nicknameInputField.text != "")
        {
            // 사용자로부터 입력 받은 IP 주소로 RoomManager의 네트워크 주소 설정
            RoomManager.singleton.networkAddress = ip.text;
            RoomManager.singleton.StartClient();
        }
        else
        {
            //nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }
}
