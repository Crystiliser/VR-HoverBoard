using System.Collections.Generic;
using UnityEngine;

public abstract class SelectedObject : MonoBehaviour
{
    [Multiline]
    public string tooltipText = "";
    [SerializeField, Tooltip("\"Time To Wait\": How long it takes for the reticle to fill up (measured in FixedUpdate ticks)")]
    private int timeToWait = 50;
    private int timeWaited = 0;
    private bool isSelected = false;
    protected AudioClip successSound;
    protected AudioClip selectedSound;
    private const int DEFAULT_DELAY = 20;
    [SerializeField, Tooltip("\"Delay\": How long to stare at the button before it registers that you want to select it (measured in FixedUpdate ticks) [-1 sets it to default 20]")]
    private int delay = DEFAULT_DELAY;
    private int delayTime = 0;
    private float waitTimer = 0.0f;
    [SerializeField, Tooltip("\"Wait Time\": How long to wait before it can select again while staring at the button (measured in seconds)")]
    private float waitTime = 1.5f;
    private bool CanSelect { get { return waitTimer <= 0.0f; } set { waitTimer = value ? 0.0f : waitTime; } }
    private bool isDisabled = false;
    private bool selectsoundplayed = false;
    public bool IsDisabled { set { isDisabled = value; if (isDisabled && null != theReticle) theReticle.updateReticle(0.0f); } }
    protected void Awake()
    {
        if (delay < 0)
            delay = DEFAULT_DELAY;
    }

    //object to update for reticle
    private reticle theReticle;

    //grabs the reticle object to show timer status
    public void selected(reticle grabbedReticle)
    {
        theReticle = grabbedReticle;
        if (!CanSelect || isDisabled)
            return;
        TooltipTextScript.SetText(tooltipText);
        selectedFuntion();
        isSelected = true;
    }

    //What the class actually does with the object while selected(if applicable)
    protected virtual void selectedFuntion() { }

    //deals with leftovers from selecting the object when you look away
    public void deSelected()
    {
        TooltipTextScript.SetText("");
        deSelectedFunction();
        theReticle.updateReticle(0);
        isSelected = false;
        timeWaited = 0;
        delayTime = 0;
        CanSelect = true;
    }

    //Cleans up what the class actually does(if applicable)
    protected virtual void deSelectedFunction() { }

    //what the class actually does when select is successful, inherited class must fill this out
    public abstract void selectSuccessFunction();

    protected void FixedUpdate()
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
                    if (null == successSound)
                        BuildDebugger.WriteLine("successSound is null");
                    AudioSource.PlayClipAtPoint(successSound, transform.position, AudioLevels.Instance.SfxVolume);
                }
            }
            float ratio = (float)timeWaited / timeToWait;
            theReticle.updateReticle(ratio);
            if (selectsoundplayed == false && timeWaited >= 2)
            {
                if (null == selectedSound)
                    BuildDebugger.WriteLine("selectedSound is null");
                AudioSource.PlayClipAtPoint(selectedSound, transform.position, AudioLevels.Instance.SfxVolume);
                selectsoundplayed = true;
            }
        }
        else
        {
            selectsoundplayed = false;
        }
    }

    protected void Start()
    {
        successSound = (AudioClip)Resources.Load("Sounds/Effects/Place_Holder_LoadSuccess");
        selectedSound = (AudioClip)Resources.Load("Sounds/Effects/Place_Holder_ButtonHit");
    }

    protected void Update()
    {
        waitTimer -= Time.deltaTime;
    }
}