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

    private ComputeBuffer pointsBuffer, triangleBuffer, triCountBuffer, debugBuffer, intersectionsBuffer;
    private int mainKernel, intersectKernel;
    private Material mat;
    private int triangleCount;
    private Vector3[] intersections;
    private Vector3 mousePosition;

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
        CheckForCollision();
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

        Vector4[] debug = new Vector4[6];
        debugBuffer.GetData(debug);
        Triangle[] buffer = new Triangle[triangleCount];
        triangleBuffer.GetData(buffer);
        //GenerateMesh(buffer);
    }

    private void CheckForCollision() {
        if (!Input.GetMouseButtonDown(0))
            return;
        mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
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
        Vector4[] debug = new Vector4[20];
        debugBuffer.GetData(debug, 0, 0, 20);
    }

    private void OnDrawGizmos() {
        if (intersections == null)
            return;

        float minDistance = float.MaxValue;
        bool hasPos = false;
        Vector3 minPos = Vector3.zero;
        Vector3 origin = Camera.main.ScreenToWorldPoint(mousePosition);
        foreach(Vector3 pos in intersections) {
            float dis = Vector3.Distance(origin, pos);
            if(dis < minDistance) {
                minDistance = dis;
                minPos = pos;
                hasPos = true;
            }
        }
        if (!hasPos)
            return;

        Gizmos.DrawSphere(minPos, 0.1f);
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
