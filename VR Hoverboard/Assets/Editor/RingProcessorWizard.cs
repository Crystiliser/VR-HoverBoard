using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
public class RingProcessorWizard : ScriptableWizard
{
    [Header("Bonus Time and Queue Order Settings")]
    [Range(10.0f, 50.0f)]
    public float targetVelocity = 30.0f;
    [Range(-1.0f, 1.0f), Tooltip("Increase or decrease the target bonus time based off of this percentage of the calculated bonus time.")]
    public float timePercentModifier = 0.0f;
    private int nextSceneIndex = 1;
    [Header("Drag Rings Here")]
    public RingSetupScript ringsParent = null;
    private RingProperties[] ringsToProcess = null;
    private Vector3 prevPosition, currPosition;
    private int currQueuePosition = 0;
    private RingProperties previousRing = null, currentRing = null;

    [MenuItem("Cybersurf Tools/Ring Processor Wizard")]
    private static void ProcessRings() => DisplayWizard<RingProcessorWizard>("Ring Processor Wizard", "Update").OnWizardUpdate();
    private void SetProperties()
    {
        currPosition = prevPosition = Vector3.zero;
        currQueuePosition = 1;
        previousRing = ringsToProcess[0];
        prevPosition = previousRing.transform.position;
        for (int i = 1; i < ringsToProcess.Length; ++i)
        {
            currentRing = ringsToProcess[i];
            currPosition = currentRing.transform.position;
            previousRing.bonusTime = Vector3.Distance(prevPosition, currPosition) * (timePercentModifier + 1.0f) / targetVelocity;
            previousRing.positionInOrder = currQueuePosition;
            previousRing.nextScene = -1;
            UnityEditorInternal.ComponentUtility.CopyComponent(previousRing);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(previousRing);
            ++currQueuePosition;
            previousRing = currentRing;
            prevPosition = currPosition;
        }
        previousRing = ringsToProcess[ringsToProcess.Length - 2];
        previousRing.nextScene = nextSceneIndex;
        UnityEditorInternal.ComponentUtility.CopyComponent(previousRing);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(previousRing);
        currentRing.nextScene = LevelManager.HubWorldBuildIndex;
        currentRing.bonusTime = Vector3.Distance(ringsToProcess[0].transform.position, currentRing.transform.position) * (timePercentModifier + 1.0f) / targetVelocity;
        currentRing.positionInOrder = currQueuePosition;
        UnityEditorInternal.ComponentUtility.CopyComponent(currentRing);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(currentRing);
    }
    private void OnWizardUpdate()
    {
        ringsParent = FindObjectOfType<RingSetupScript>();
        isValid = null != ringsParent;
    }
    private void OnWizardCreate()
    {
        nextSceneIndex = LevelManager.LevelBuildOffset;
        int levelIdx = SceneManager.GetActiveScene().buildIndex - nextSceneIndex + 1;
        if (levelIdx < LevelManager.LevelCount) nextSceneIndex += levelIdx;
        for (int i = 0; i < (int)GameDifficulty.GameDifficultiesSize; ++i)
        {
            ringsToProcess = GetRings(ringsParent.GetRingDifficultyParent((GameDifficulty)i).transform);
            if (ringsToProcess.Length > 3)
                SetProperties();
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
    private class RingPropertiesSiblingIndexComparer : IComparer<RingProperties>
    { public int Compare(RingProperties x, RingProperties y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex(); }
    private static RingProperties[] GetRings(Transform parent)
    {
        List<RingProperties> rings = new List<RingProperties>();
        int childCount = parent.childCount;
        for (int i = 0; i < parent.childCount; ++i)
            rings.Add(parent.GetChild(i).GetComponent<RingProperties>());
        rings.Sort(new RingPropertiesSiblingIndexComparer());
        return rings.ToArray();
    }
}