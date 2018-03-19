using UnityEngine;
public class FadeObjectScript : MonoBehaviour
{
    private Color color, invis;
    private Material objectMaterial = null;
    private float distance;
    private void Start()
    {
        objectMaterial = GetComponent<Renderer>().material;
        color = objectMaterial.color;
        invis = color;
        invis.a = 0.0f;
    }
    private void Update()
    {
        distance = (Camera.main.transform.position - transform.position).magnitude;
        if (distance < 15.0f)
            objectMaterial.color = Color.Lerp(color, invis, 7.5f / distance);
    }
}