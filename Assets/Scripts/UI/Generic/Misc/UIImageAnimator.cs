using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    public Image targetImage;       // The UI Image component
    public Sprite[] frames;         // All animation frames
    public float frameRate = 0.1f;  // Time between frames

    private int currentFrame;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            targetImage.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
