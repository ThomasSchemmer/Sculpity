using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    private Image currentBrushImage;
    
    public void SetBrushAs(Image target) {
        ChunkManager.SetBrushAs(target.sprite.texture);

        if (currentBrushImage)
            currentBrushImage.color = Color.black;
        currentBrushImage = target;
        currentBrushImage.color = Color.blue;
    }

    public void SetStrengthAs(GameObject sliderObj) {
        Slider slider = sliderObj.GetComponent<Slider>();
        Vector3 hitPos = RightHand.lastHit.point;
        float distance = Vector3.Distance(hitPos, sliderObj.transform.position);
        float dir = Vector3.Dot(sliderObj.transform.right, hitPos - sliderObj.transform.position);
        if (dir < 0)
            distance *= -1;
        float value = Mathf.Clamp(distance / 0.11f, -1, 1);
        value = 0.5f + value / 2f;
        int iv = (int)(value * 10);
        slider.value = iv;

        sliderObj.transform.GetChild(4).GetComponent<TMP_Text>().text = "" + (iv / 5f);
        ChunkManager.SetBrushStrength(iv / 5f);
    }

    public void SetSizeAs(GameObject sliderObj) {
        Slider slider = sliderObj.GetComponent<Slider>();
        Vector3 hitPos = RightHand.lastHit.point;
        float distance = Vector3.Distance(hitPos, sliderObj.transform.position);
        float dir = Vector3.Dot(sliderObj.transform.right, hitPos - sliderObj.transform.position);
        if (dir < 0)
            distance *= -1;
        float value = Mathf.Clamp(distance / 0.11f, -1, 1);
        value = 0.5f + value / 2f;
        int iv = (int)(value * 10);
        slider.value = iv;
        value = (iv / 10f + 0.05f);
        sliderObj.transform.GetChild(4).GetComponent<TMP_Text>().text = "" + value;
        ChunkManager.SetBrushSize(value);
    }

    public void ShowPreview(Toggle toggle) {
        toggle.isOn = !toggle.isOn;
        ChunkManager.ShowPreview(toggle.isOn);
    }

    public void CloseMenu() {
        RightHand.CloseMenu();
    }
}
