using TMPro;
using UnityEngine;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    private void OnValidate()
    {
        if (versionText == null) return;

        versionText.text = string.Format(versionText.text, $"{Application.version}");
    }

    private void Start()
    {
        versionText.text = string.Format(versionText.text, $"{Application.version}");
    }

    public void Exit()
    {
        Debug.Log("Exitted game");

        if (!Application.isEditor)
        {
            Application.Quit();
        }
    }
}