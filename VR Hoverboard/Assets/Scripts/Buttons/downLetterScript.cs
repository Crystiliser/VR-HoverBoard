using UnityEngine;
using TMPro;
public class downLetterScript : SelectedObject
{
    [SerializeField]
    private TextMeshPro textMesh;
    public override void selectSuccessFunction()
    {
        textMesh.ForceMeshUpdate();
        char letter = textMesh.GetParsedText()[0];
        --letter;
        if (letter < 'A')
            letter = 'Z';
        textMesh.SetText(letter.ToString());
    }
}