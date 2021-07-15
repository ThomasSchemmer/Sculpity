using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Material mat;
    public ComputeShader shader;
    private int split = 4;

    // Start is called before the first frame update
    void Start()
    {
        GenerateChunks();
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
                   // offset = Vector3.one * 15;
                   // ppAs = Vector3.one * 14;
                    GameObject chunk = new GameObject();
                    ChunkGenerator gen = chunk.AddComponent<ChunkGenerator>();
                    gen.shader = shader;
                    gen.GetComponent<MeshRenderer>().material = mat;
                    gen.Setup(ppAs, offset);
                    chunk.name = x + "|" + y + "|" + z;
                    chunk.transform.parent = this.transform;
                }
            }
        }
    }
}
