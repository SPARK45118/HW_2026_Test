using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OceanWave : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveHeight = 0.15f;
    public float waveSpeed = 1f;
    public float waveScale = 0.5f;

    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] displacedVertices;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        baseVertices = mesh.vertices;
        displacedVertices = new Vector3[baseVertices.Length];
    }

    private void Update()
    {
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float offset = Mathf.Sin(Time.time * waveSpeed + (v.x + v.z) * waveScale) * waveHeight;
            displacedVertices[i] = new Vector3(v.x, v.y + offset, v.z);
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
    }
}