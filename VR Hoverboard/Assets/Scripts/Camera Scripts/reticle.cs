using UnityEngine;
using UnityEngine.UI;
public class reticle : MonoBehaviour
{
    //image object for the selection radial(required to function for radial bar)
    [SerializeField]
    private Image selectionRadial = null;
    public void updateReticle(float ratioOfTimePassed)
    {
        selectionRadial.fillAmount = ratioOfTimePassed;
    }
}