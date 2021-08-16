using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager instance;

    public Material mat;
    public ComputeShader shader;
    public Texture2D brushTex;
    public float brushSize = 0.1f;
    public float brushStrength = 0.1f;
    public RenderTexture brushTexture;


    private int split = 2;
    private List<Vector3> intersections;
    private Vector3 closestIntersection;
    private bool hasIntersection = false;
    private int lastIntersectionFrame;
    private bool isIntersectionDirty;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        intersections = new List<Vector3>();
        GenerateChunks();
    }

    private void Update() {
        CheckForClosestIntersection();
    }

    private void CheckForClosestIntersection() {
        if (intersections == null || !isIntersectionDirty)
            return;

        float minDistance = float.MaxValue;
        hasIntersection = false;
        closestIntersection = Vector3.zero;
        Vector3 origin = Camera.main.transform.position;
        foreach (Vector3 pos in intersections) {
            float dis = Vector3.Distance(origin, pos);
            if (dis < minDistance) {
                minDistance = dis;
                closestIntersection = pos;
                hasIntersection = true;
            }
        }
        isIntersectionDirty = false;

        if (!hasIntersection)
            return;
        UpdateChunks();
    }

    private void GenerateChunks() {
        int size = PointGenerator.pointsPerAxis / split;
        for (int z = 0; z < split; z++) {
            for (int y = 0; y < split; y++) {
                for (int x = 0; x < split; x++) {
                    Vector3 offset = new Vector3(x, y, z) * size;
                    Vector3 ppAs = new Vector3(
                        x >= split - 1 ? size - 1 : size,
                        y >= split - 1 ? size - 1 : size,
                        z >= split - 1 ? size - 1 : size
                    );
                    GameObject chunk = new GameObject();
                    ChunkGenerator gen = chunk.AddComponent<ChunkGenerator>();
                    gen.shader = shader;
                    gen.GetComponent<MeshRenderer>().material = mat;
                    gen.Setup(ppAs, offset, new Vector3Int(x, y, z));
                    chunk.name = x + "|" + y + "|" + z;
                    chunk.transform.parent = this.transform;
                }
            }
        }
    }

    private void UpdateChunks() {
        foreach(Transform child in this.transform) {
            ChunkGenerator gen = child.GetComponent<ChunkGenerator>();
            float width = PointGenerator.instance.scale / split;
            Vector3 center = 
                gen.offset / PointGenerator.pointsPerAxis * PointGenerator.instance.scale +             
                Vector3.one * width / 2f;
            float dis = Vector3.Distance(center, closestIntersection);
            if (dis > brushSize + width)
                continue;
            gen.UpdateChunk(closestIntersection, brushSize, brushStrength, split);                
        }
    }

    private void _AddIntersections(Vector3[] intersections) {
        if (Time.frameCount - 5 > lastIntersectionFrame) {
            this.intersections = new List<Vector3>();
            lastIntersectionFrame = Time.frameCount;
        }
        this.intersections.AddRange(intersections);
        isIntersectionDirty = true;
    }

    public static void AddIntersections(Vector3[] intersections) {
        instance._AddIntersections(intersections);
    }

    public static RenderTexture GetBrushTexture() {
        if (instance.brushTexture)
            return instance.brushTexture;

        instance.brushTexture = RenderTexture.GetTemporary(256, 256);
        instance.brushTexture.enableRandomWrite = true;
        instance.brushTexture.Create();
        RenderTexture.active = instance.brushTexture;
        Graphics.Blit(instance.brushTex, instance.brushTexture);
        return instance.brushTexture;
    }

    public static void CheckForIntersections(Vector3 origin, Vector3 forward) {
        if (!instance)
            return;

        Ray ray = new Ray(origin, forward);
        foreach(Transform obj in instance.transform) {
            ChunkGenerator gen = obj.GetComponent<ChunkGenerator>();
            gen.CheckForCollision(ray);
        }
    }
}
