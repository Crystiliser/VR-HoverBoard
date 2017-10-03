using UnityEngine;
public class BackgroundMusic : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource = null;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");
        if (objs.Length > 1 && objs[0].GetComponent<BackgroundMusic>().audioSource.clip.name == objs[1].GetComponent<BackgroundMusic>().audioSource.clip.name)
        {
            Destroy(this.gameObject);
        }
        else if (objs.Length > 1 && objs[0].GetComponent<BackgroundMusic>().audioSource.clip.name != objs[1].GetComponent<BackgroundMusic>().audioSource.clip.name)
        {
            Destroy(objs[0]);
        }
        DontDestroyOnLoad(audioSource);
    }
    private void Start()
    {
        UpdateVolume();
    }
    private void OnEnable()
    {
        AudioLevels.Instance.OnBgmVolumeChange += UpdateVolume;
    }
    private void OnDisable()
    {
        try { AudioLevels.Instance.OnBgmVolumeChange -= UpdateVolume; } catch { }
    }
    private void UpdateVolume()
    {
        if (null != audioSource)
            audioSource.volume = AudioLevels.Instance.BgmVolume;
    }
}