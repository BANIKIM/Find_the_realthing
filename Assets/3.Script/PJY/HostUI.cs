using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HostUI : MonoBehaviour
{
    [SerializeField]
    private InputField nicknameInputField;
   /* [SerializeField]
    private InputField ipInputField; // ���� �߰��� InputField*/
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

   
}
