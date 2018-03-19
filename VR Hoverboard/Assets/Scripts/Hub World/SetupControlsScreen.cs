using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static KeyInputManager.VR;
public class SetupControlsScreen : MonoBehaviour
{
    [SerializeField] private Sprite loadingImage = null;
    [SerializeField] private Sprite[] controlsImages = null;
    [SerializeField] private Image ImageObject = null;
    private void Start()
    {
        ImageObject.sprite = loadingImage;
        StartCoroutine(WaitForDetection());
    }
    private IEnumerator WaitForDetection()
    {
        yield return new WaitForSeconds(SpatialData.WaitForAttach);
        if (BoardManager.gamepadEnabled)
            if (VRPresent)
                ImageObject.sprite = controlsImages[0];
            else
                ImageObject.sprite = controlsImages[2];
        else
            ImageObject.sprite = controlsImages[1];
    }
}