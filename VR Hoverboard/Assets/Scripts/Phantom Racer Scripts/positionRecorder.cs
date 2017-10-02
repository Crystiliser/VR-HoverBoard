using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class positionRecorder : MonoBehaviour
{
    public Queue<Vector3> positions = new Queue<Vector3>();
    public Queue<Quaternion> rotations = new Queue<Quaternion>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += clear;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= clear;
    }

    void clear(Scene s, LoadSceneMode m)
    {
        positions.Clear();
        rotations.Clear();
    }

    void FixedUpdate()
    {
        positions.Enqueue(gameObject.transform.position);
        rotations.Enqueue(gameObject.transform.rotation);
    }

}
