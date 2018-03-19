using UnityEngine;
public class playerCollisionSoundEffects : MonoBehaviour
{
    private AudioSource source = null;
    private PlayerRespawn respawnScript = null;
    [SerializeField] private AudioClip wallCollision = null;
    [SerializeField] private AudioClip ringCollision = null;
    [SerializeField] private AudioClip portalEnter = null;
    private float const_vol = 1.0f;
    private GameObject prevRingObject = null;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        respawnScript = GetComponent<PlayerRespawn>();
        prevRingObject = null;
        AudioManager.OnSfxVolumeChanged += UpdateVolume;
        UpdateVolume();
        const_vol = source.volume;
    }
    private void Update()
    {
        if (source.isPlaying && "HitThud" != source.clip.name)
            source.volume = const_vol * (1.0f - Mathf.Abs(source.clip.length * 0.5f - source.time));
        else
            source.volume = const_vol = AudioManager.SfxVolume;
    }
    private void OnDestroy() => AudioManager.OnSfxVolumeChanged -= UpdateVolume;
    private void UpdateVolume() => source.volume = AudioManager.SfxVolume;
    private void OnCollisionEnter(Collision collision)
    {
        source.clip = wallCollision;
        source.Play();
    }
    public void PlayRingClip(GameObject ringObject)
    {
        if (ringObject != prevRingObject && !respawnScript.IsRespawning)
        {
            source.clip = ringCollision;
            source.Play();
            prevRingObject = ringObject;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!respawnScript.IsRespawning && "Portal" == other.gameObject.tag)
        {
            source.clip = portalEnter;
            source.Play();
        }
    }
}