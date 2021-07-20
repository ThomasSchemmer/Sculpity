using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGenerator : MonoBehaviour
{
    public static PointGenerator instance;

    public ComputeShader shader;
    [Range(0.1f, 10)]
    public float scale = 1;
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector3 center = Vector3.one * 0.5f;
    [HideInInspector]
    public ComputeBuffer pointsBuffer, debugBuffer;
    public static int groupCount = 10;
    public static bool isDirty;
    public static int pointsPerAxis = 10; 


    private float _scale;
    private Vector3 _offset;
    private int pointCount;

    private int kernel, kernelGroupOnly, active;
    private bool onlyGroups = false;
    private Material mat;

    void Start()
    {
        instance = this;
        pointCount = pointsPerAxis * pointsPerAxis * pointsPerAxis;
        SetupShader();
        _scale = scale - 1;
        _offset = new Vector3(offset.x, offset.y, offset.z);
    }

    private void Update() {
        isDirty = false;
        if (scale != _scale || offset != _offset) {
            CalculatePoints();
            _scale = scale;
            _offset = new Vector3(offset.x, offset.y, offset.z);
            isDirty = true;
           // StartCoroutine(Pause());
        }
    }

    private IEnumerator Pause() {
        yield return new WaitForSeconds(0.1f);

        UnityEditor.EditorApplication.isPaused = true; 
    }

    private void OnRenderObject() {

        mat.SetPass(0);
        Vector4[] arr = new Vector4[1];
        pointsBuffer.GetData(arr, 0, 0, 1);
        Vector3[] debug = new Vector3[1];
        debugBuffer.GetData(debug, 0, 0, 1);
        mat.SetBuffer("_Buffer", pointsBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points, pointCount);
    }


    private void SetupShader() {
        kernel = shader.FindKernel("main");
        kernelGroupOnly = shader.FindKernel("mainGroupOnly");
        if (onlyGroups)
            active = kernelGroupOnly;
        else
            active = kernel;
        pointsBuffer = new ComputeBuffer(pointCount, sizeof(float) * 4);
        debugBuffer = new ComputeBuffer(pointCount, sizeof(float) * 3);

        shader.SetBuffer(active, "Points", pointsBuffer);
        shader.SetBuffer(active, "debug", debugBuffer);
        shader.SetInt("groupCount", onlyGroups ? pointsPerAxis : groupCount);
        shader.SetInt("ppA", pointsPerAxis);
        shader.SetVector("offset", offset);
    }



    private void CalculatePoints() {
        if(!mat)
            mat = this.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetBuffer("_Buffer", pointsBuffer);
        shader.SetFloat("scale", scale);
        shader.SetVector("offset", offset);
        shader.SetVector("center", center);
        int c = onlyGroups ? pointsPerAxis : groupCount;
        float now = Time.time;
        shader.Dispatch(active, c, c, c);
        Debug.Log("Diff: " + (Time.time - now));
    }

    private void OnDestroy() {
        pointsBuffer.Release();
        debugBuffer.Release();
    }

    public int GetCount() {
        return (int)Mathf.Pow(pointsPerAxis * 8, 3);
    }

}
