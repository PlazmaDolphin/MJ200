using UnityEngine;
using UnityEngine.UI;

public class RadialLoader : MonoBehaviour
{
    public Image fillImage;
    private float duration = 1f;

    float timer;
    bool loading;

    public GameObject uiRoot; // parent om te show/hide-en

    public void StartLoading(float duration)
    {
        if (uiRoot) uiRoot.SetActive(true);
        if (fillImage) fillImage.fillAmount = 0f;
    }

    public void SetProgress(float normalized01)
    {
        if (!fillImage) return;
        fillImage.fillAmount = Mathf.Clamp01(normalized01);
    }

    public void CancelLoading()
    {
        SetProgress(0f);
        if (uiRoot) uiRoot.SetActive(false);
    }

    public void CompleteAndHide()
    {
        SetProgress(1f);
        if (uiRoot) uiRoot.SetActive(false);
    }
}