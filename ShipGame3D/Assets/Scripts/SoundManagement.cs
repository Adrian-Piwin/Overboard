using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManagement : MonoBehaviour
{
    [Header("Sources and Sliders")]
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceEffects;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    [Header("Effects")]
    [SerializeField] private AudioClip splash;
    [SerializeField] private AudioClip sinkstart;
    [SerializeField] private AudioClip sinkfinish;
    [SerializeField] private AudioClip squidthrow;

    private float musicVolume = 0.3f;
    private float effectsVolume = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("music volume", musicVolume);
        musicSlider.value = musicVolume;
        audioSourceMusic.Play();

        effectsVolume = PlayerPrefs.GetFloat("effects volume", effectsVolume);
        effectsSlider.value = effectsVolume;
    }

    // Update is called once per frame
    void Update()
    {
        audioSourceMusic.volume = musicVolume;
        audioSourceEffects.volume = effectsVolume;
    }

    public void UpdateVolumeMusic(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("music volume", volume);
    }
    
    public void UpdateVolumeEffects(float volume)
    {
        effectsVolume = volume;
        PlayerPrefs.SetFloat("effects volume", volume);
    }

    public void PlayEffect(string effect)
    {
        switch (effect)
        {
            case "splash":
                audioSourceEffects.PlayOneShot(splash);
                break;
            case "sinkstart":
                audioSourceEffects.PlayOneShot(sinkstart);
                break;
            case "sinkfinish":
                audioSourceEffects.PlayOneShot(sinkfinish);
                break;
            case "squidthrow":
                audioSourceEffects.PlayOneShot(squidthrow);
                break;
        }
    }
}
