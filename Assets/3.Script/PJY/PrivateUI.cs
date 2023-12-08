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
            // ����ڷκ��� �Է� ���� IP �ּҷ� RoomManager�� ��Ʈ��ũ �ּ� ����
            RoomManager.singleton.networkAddress = ip.text;
            RoomManager.singleton.StartClient();
        }
        else
        {
            //nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }
}
