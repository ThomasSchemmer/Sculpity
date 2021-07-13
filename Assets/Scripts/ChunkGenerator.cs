using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkGenerator : MonoBehaviour
{
    public ComputeShader shader;

    private ComputeBuffer pointsBuffer, triangleBuffer, triCountBuffer, debugBuffer;
    private int kernel;
    public Vector3 size;
    //in ppA
    public Vector3 offset;

    struct Triangle {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
    };

    public void Setup(Vector3 size, Vector3 offset) {
        this.size = size;
        this.offset = offset;
        SetupShaders();
    }

    public void Update() {
        if (PointGenerator.isDirty || Input.GetKeyDown(KeyCode.Space)) {
            Generate();
        }
    }


    public void Generate() {
        Debug.Log("Generating");
        if (pointsBuffer == null)
            SetupShaders();
        shader.SetInt("ppA", PointGenerator.pointsPerAxis);
        shader.SetVector("size", size);
        shader.SetVector("offset", offset);
        triangleBuffer.SetCounterValue(0);
        shader.SetBuffer(kernel, "Triangles", triangleBuffer);

        shader.Dispatch(kernel, (int)size.x, (int)size.y, (int)size.z);

        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCount = { 0 };
        triCountBuffer.GetData(triCount);
        Triangle[] triangles = new Triangle[triCount[0]];
        triangleBuffer.GetData(triangles, 0, 0, triCount[0]);

        Vector3[] debug = new Vector3[1];
      //  debugBuffer.GetData(debug, 0, 0, 1);

        Vector4[] points = new Vector4[8];
      //  pointsBuffer.GetData(points);

        GenerateMesh(triangles);
    }

    private void SetupShaders() {

        triangleBuffer = new ComputeBuffer(
            GetBufferSize(), 
            sizeof(float) * 3 * 3, 
            ComputeBufferType.Append
        );
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        pointsBuffer = PointGenerator.instance.pointsBuffer;
        debugBuffer = new ComputeBuffer(GetBufferSize(), sizeof(float) * 3);


        kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "triangles", triangleBuffer);
        shader.SetBuffer(kernel, "Points", pointsBuffer);
        shader.SetBuffer(kernel, "Debug", debugBuffer);
    }

    private void GenerateMesh(Triangle[] triangles) {
        List<Vector3> vertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();

        foreach(Triangle t in triangles) {
            int c = vertices.Count;
            vertices.AddRange(new Vector3[] { t.a, t.b, t.c });
            meshTriangles.AddRange(new int[] { c, c + 1, c + 2 });
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateNormals();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDisable() {
        triangleBuffer.Dispose();
        triCountBuffer.Dispose();
        debugBuffer.Dispose();
    }

    private int GetBufferSize() {
        return (int)(size.x * size.y * size.z * 8);
    }
}
