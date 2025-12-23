using UnityEngine;

public class SoundWhenEnabled : MonoBehaviour
{
    [SerializeField] private SoundFXData soundFXData;
    [SerializeField] private bool called;

    private void OnEnable()
    {
        if (!called)
        {
            called = true;
            return;
        }

        soundFXData.Play();
    }
}
