using UnityEngine;

public abstract class SelectedObject : MonoBehaviour
{
    [SerializeField, Tooltip("\"Time To Wait\": How long it takes for the reticle to fill up (measured in FixedUpdate ticks)")]
    private int timeToWait = 50;
    private int timeWaited = 0;
    private bool isSelected = false;
    protected AudioClip successSound;
    protected AudioClip selectedSound;
    private bool firstSelection = true;
    [SerializeField, Tooltip("\"Delay\": How long to stare at the button before it registers that you want to select it (measured in FixedUpdate ticks) [-1 sets it to default 20]")]
    private int delay = -1;
    private int delayTime = 0;
    private float waitTimer = 0.0f;
    [SerializeField, Tooltip("\"Wait Time\": How long to wait before it can select again while staring at the button (measured in seconds)")]
    private float waitTime = 1.5f;
    private bool CanSelect { get { return waitTimer <= 0.0f; } set { waitTimer = value ? 0.0f : waitTime; } }
    private bool isDisabled = false;
    public bool IsDisabled { set { isDisabled = value; if (isDisabled && null != theReticle) theReticle.updateReticle(0.0f); } }

    private void Awake()
    {
        const int defaultDelay = 20;
        if (delay < 0)
            delay = defaultDelay;
    }

    //object to update for reticle
    private reticle theReticle;

    //to play sound effect attached to object
    private AudioSource audioSource = null;

    //grabs the reticle object to show timer status
    public void selected(reticle grabbedReticle)
    {
        theReticle = grabbedReticle;
        if (!CanSelect || isDisabled)
            return;
        if (firstSelection)
        {
            if (null == audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.clip = selectedSound;
            audioSource.volume = AudioLevels.Instance.SfxVolume;
            audioSource.Play();
            firstSelection = false;
        }
        selectedFuntion();
        isSelected = true;
    }

    //What the class actually does with the object while selected(if applicable)
    public virtual void selectedFuntion() { }

    //deals with leftovers from selecting the object when you look away
    public void deSelected()
    {
        deSelectedFunction();
        theReticle.updateReticle(0);
        isSelected = false;
        timeWaited = 0;
        delayTime = 0;
        firstSelection = true;
        CanSelect = true;
    }

    //Cleans up what the class actually does(if applicable)
    public virtual void deSelectedFunction() { }

    //what the class actually does when select is successful, inherited class must fill this out
    public abstract void selectSuccessFunction();

    private void FixedUpdate()
    {
        if (isSelected && CanSelect && !isDisabled)
        {
            ++delayTime;
            if (delayTime >= delay)
                ++timeWaited;
            if (timeWaited >= timeToWait)
            {
                selectSuccessFunction();
                CanSelect = false;
                theReticle.updateReticle(0);
                timeWaited = 0;
                if (isActiveAndEnabled)
                {
                    if (null == audioSource)
                    {
                        audioSource = gameObject.AddComponent<AudioSource>();
                    }
                    audioSource.clip = successSound;
                    audioSource.volume = AudioLevels.Instance.SfxVolume;
                    audioSource.Play();
                }
            }
            float ratio = (float)timeWaited / timeToWait;
            theReticle.updateReticle(ratio);
        }
    }

    private void Start()
    {
        successSound = (AudioClip)Resources.Load("Sounds/Effects/Place_Holder_LoadSuccess");
        selectedSound = (AudioClip)Resources.Load("Sounds/Effects/Place_Holder_ButtonHit");
    }

    private void Update()
    {
        waitTimer -= Time.deltaTime;
    }
}