using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class RightHand : MonoBehaviour
{
    public static RightHand instance;
    public static RaycastHit lastHit;

    public SteamVR_Action_Pose pose;
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Boolean menu;

    private LineRenderer lr;
    private GameObject canvas;
    private Material mat;
    private MyUIElement lastUIElement;

    private void Start() {
        instance = this;
        canvas = GameObject.Find("Canvas");
        canvas.SetActive(false);
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        mat = lr.material;
    }


    private void DisplayMenu(bool show) {
        Camera cam = Camera.main;
        canvas.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        canvas.transform.position = cam.transform.position + cam.transform.forward * 2;
        canvas.SetActive(show);
    }

    void Update()
    {
        bool showPreview = ChunkManager.PreviewEnabled();
        if (menu.stateDown)
            DisplayMenu(!canvas.activeSelf);

        if (showPreview && !trigger.state) {

        }

        if (trigger.state) {
            if (canvas.activeInHierarchy) {
                ShootUIRay();
            } else {
                ShootSculptRay();
            }
        }
        if(trigger.stateUp && canvas.activeInHierarchy) {
            OnUIUp();
        }
        if(!trigger.state && (!showPreview || canvas.activeInHierarchy)) {
            lr.enabled = false;
        }
    }

    private void OnUIUp() {
        if (!trigger.stateUp)
            return;
        if (!lastUIElement)
            return;

        lastUIElement.OnUp();
        lr.enabled = false;
    }

    private void ShootPreviewRay() {
        Vector3 dir = Quaternion.AngleAxis(60, transform.right) * transform.forward;
        mat.color = Color.blue;
        lr.positionCount = 2;
        lr.SetPosition(0, pose.localPosition);
        lr.SetPosition(1, pose.localPosition + dir * 5);
        lr.enabled = true;

        ChunkManager.CheckForIntersections(pose.localPosition, dir);
    }

    private void ShootUIRay() {
        Vector3 dir = Quaternion.AngleAxis(60, transform.right) * transform.forward;
        mat.color = Color.black;
        lr.positionCount = 2;
        lr.SetPosition(0, pose.localPosition);
        lr.enabled = true;
        RaycastHit[] hits = Physics.RaycastAll(pose.localPosition, dir);
        if (hits.Length == 0) {
            lastUIElement = null;
            lr.SetPosition(1, pose.localPosition + dir * 5);
            return;
        }

        lr.SetPosition(1, hits[0].point);
        MyUIElement ui = hits[0].transform.GetComponent<MyUIElement>();
        if (!ui) {
            lastUIElement = null;
            return;
        }
        lastHit = hits[0];
        lastUIElement = ui;

        ui.Trigger();
    }

    private void ShootSculptRay() {
        Vector3 dir = Quaternion.AngleAxis(60, transform.right) * transform.forward;
        ChunkManager.CheckForIntersections(pose.localPosition, dir);
        lr.positionCount = 2;
        lr.SetPosition(0, pose.localPosition);
        lr.SetPosition(1, pose.localPosition + dir * 5);
        mat.color = Color.green;
        lr.enabled = true;
    }

    public static void CloseMenu() {
        instance.DisplayMenu(false);
    }
}
