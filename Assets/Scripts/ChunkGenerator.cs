using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkGenerator : MonoBehaviour
{
    public ComputeShader shader;

    public Vector3 size;
    //in ppA
    public Vector3 offset;
    public Vector3Int id;

    private ComputeBuffer pointsBuffer, triangleBuffer, triCountBuffer, debugBuffer, intersectionsBuffer;
    private int mainKernel, intersectKernel, updateKernel;
    private Material mat;
    private int triangleCount;
    private Vector3[] intersections;
    private Vector3 mousePosition;

    struct Triangle {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
    };

    public void Setup(Vector3 size, Vector3 offset, Vector3Int id) {
        this.size = size;
        this.offset = offset;
        this.id = id;
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
        if (!mat)
            mat = GetComponent<MeshRenderer>().material;
        shader.SetInt("ppA", PointGenerator.pointsPerAxis);
        shader.SetVector("size", size);
        shader.SetVector("offset", offset);
        triangleBuffer.SetCounterValue(0);
        shader.SetBuffer(mainKernel, "Triangles", triangleBuffer);

        shader.Dispatch(mainKernel, (int)size.x, (int)size.y, (int)size.z);

        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCount = { 0 };
        triCountBuffer.GetData(triCount);
        triangleCount = triCount[0];
        shader.SetFloat("triangleCount", triangleCount);

        mat.SetBuffer("_Buffer", triangleBuffer);

        //GenerateMesh(buffer);
    }

    public void CheckForCollision(Ray ray) {
        shader.SetVector("origin", ray.origin);
        shader.SetVector("direction", ray.direction);
        shader.SetFloat("triangleCount", triangleCount);
        shader.SetBuffer(intersectKernel, "Intersections", intersectionsBuffer);
        shader.SetBuffer(intersectKernel, "Triangles", triangleBuffer);
        shader.SetBuffer(intersectKernel, "Debug", debugBuffer);
        intersectionsBuffer.SetCounterValue(0);

        shader.Dispatch(intersectKernel, 1, 1, 1); 
        ComputeBuffer.CopyCount(intersectionsBuffer, triCountBuffer, 0);
        int[] intCountArr = { 0 };
        triCountBuffer.GetData(intCountArr);
        intersections = new Vector3[intCountArr[0]];
        intersectionsBuffer.GetData(intersections);
        ChunkManager.AddIntersections(intersections);
    }


    private void OnRenderObject() {
        mat.SetPass(0);
        mat.SetBuffer("_Buffer", triangleBuffer);
        mat.SetVector("center", PointGenerator.instance.center);
        Graphics.DrawProceduralNow(MeshTopology.Points, triangleCount);
    }

    private void SetupShaders() {
        triangleBuffer = new ComputeBuffer(
            GetBufferSize(),
            sizeof(float) * 3 * 3,
            ComputeBufferType.Counter
        );
        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        pointsBuffer = PointGenerator.instance.pointsBuffer;
        debugBuffer = new ComputeBuffer(GetBufferSize(), sizeof(float) * 4);
        intersectionsBuffer = new ComputeBuffer(
            GetBufferSize() / 3,
            sizeof(float) * 3,
            ComputeBufferType.Append
        );

        mainKernel = shader.FindKernel("CSMain");
        intersectKernel = shader.FindKernel("CSIntersections");
        updateKernel = shader.FindKernel("CSUpdate");
        shader.SetBuffer(mainKernel, "Points", pointsBuffer);
        shader.SetBuffer(mainKernel, "Debug", debugBuffer);
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

    public void UpdateChunk(Vector3 brushPos, float brushSize, float brushStrength, int split) {
        shader.SetVector("brushPos", brushPos);
        shader.SetVector("offset", offset);
        shader.SetFloat("brushSize", brushSize);
        shader.SetFloat("brushStrength", brushStrength);
        shader.SetInt("ppA", PointGenerator.pointsPerAxis);
        shader.SetInt("brushImgSize", 256);
        shader.SetTexture(updateKernel, "Brush", ChunkManager.GetBrushTexture());
        shader.SetBuffer(updateKernel, "Points", pointsBuffer);
        shader.SetBuffer(updateKernel, "Debug", debugBuffer);

        Vector3 adds = new Vector3(
            id.x >= split - 1 ? (int)size.x +1 : (int)size.x,      //
            id.y >= split - 1 ? (int)size.y +1 : (int)size.y,      //
            id.z >= split - 1 ? (int)size.z +1 : (int)size.z       //
        );
        shader.Dispatch(updateKernel, (int)adds.x, (int)adds.y, (int)adds.z);

        Generate();
    }

    private void OnDisable() {
        triangleBuffer.Dispose();
        triCountBuffer.Dispose();
        debugBuffer.Dispose();
        intersectionsBuffer.Dispose();
    }

    private int GetBufferSize() {
        return (int)(size.x * size.y * size.z * 8);
    }
}
