using UnityEngine;

public class invertCollision : MonoBehaviour
{
    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        if (null != filter)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; ++i)
            {
                normals[i] = -normals[i];
            }

            for (int m = 0; m < mesh.subMeshCount; ++m)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    int temp = triangles[j];
                    triangles[j] = triangles[j + 1];
                    triangles[j + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }

            gameObject.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
        }
    }
}