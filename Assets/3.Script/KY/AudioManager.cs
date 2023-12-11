using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMList
{
    BGMStart = 0,
    BGMLoop
}
public enum SFXList
{
    Stab1 = 0,
    Stab2,
    Stab3,
    Buttonclick1 = 3,
    Buttonclick2,
    Buttonclick3
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    [Header("오디오 클립")]
    public AudioClip[] BGMClip;
    public AudioClip[] SFXClip;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            return;
        }
    }

}
