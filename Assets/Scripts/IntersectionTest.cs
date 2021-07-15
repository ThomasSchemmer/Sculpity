using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionTest : MonoBehaviour
{

    Vector3 a = new Vector3(0, 0, 0), b = new Vector3(1, 0, 0), c = new Vector3(0, 1, 0);
    float u, v, t;
    Ray ray;

    void Start()
    {
        CreateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 e1 = b - a;
        Vector3 e2 = c - a;
        float ree = Det(ray.direction, e1, e2);
        u = -Det(e2, ray.origin - a, ray.direction) / ree;
        v = Det(e1, ray.origin - a, ray.direction) / ree;
        t = -Det(ray.origin - a, e1, e2) / ree;
    }

    private void OnDrawGizmos() {

        if (u >= 0 && v >= 0 && t >= 0 && (u + v) <= 1) {
            Gizmos.DrawSphere(ray.origin + ray.direction * t, 0.2f);
        }
    }



    private float Det(Vector3 a, Vector3 b, Vector3 c) {
        return Vector3.Dot(a, Vector3.Cross(b, c));
    }


    private void CreateMesh() {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer rend = gameObject.AddComponent<MeshRenderer>();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.color = Color.blue;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Mesh mesh = new Mesh();

        vertices.AddRange(new Vector3[] {
            a,
            b,
            c
        });
        triangles.AddRange(new int[] { 0, 1, 2 });
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        filter.mesh = mesh;

    }
}
