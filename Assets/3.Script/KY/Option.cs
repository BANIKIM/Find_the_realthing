using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum Channel
{
    Master = 0,
    BGM,
    SFX
}

public class Option : MonoBehaviour
{
    public static Option instance = null;

    [Header("¿Àµð¿À ¹Í¼­")]
    public AudioMixer audioMixer;
    public Slider[] Audio_Sliders;

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

    public void Master_SetVolume()
    {
        audioMixer.SetFloat("Master", Audio_Sliders[(int)Channel.Master].value);
    }

    public void BGM_SetVolume()
    {
        audioMixer.SetFloat("BGM", Audio_Sliders[(int)Channel.BGM].value);
    }

    public void SFX_SetVolume()
    {
        audioMixer.SetFloat("SFX", Audio_Sliders[(int)Channel.SFX].value);
    }

}
