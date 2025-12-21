using UnityEngine;

// If you want spawn particles instead of playing them (good for objects that destroy themselves like coins)
public class UnparentedParticle : MonoBehaviour
{
    private ParticleSystem particle;
    private static Transform particlesParent;

    [SerializeField] private bool useSpawnersRotation = true;

    private void Start()
    {
        if (particlesParent == null)
        {
            GameObject newObj = new GameObject("Unparented Particles"); // Creates a new empty GameObject
            particlesParent = newObj.transform; // Get its transform
        }

        particle = GetComponent<ParticleSystem>();
    }

    public void PlayParticleUnparented(Transform source)
    {
        if (particle == null)
        {
            Debug.LogError($"{gameObject.name} unparented particle's reference is null!", this);
            return;
        }

        if (source == null)
        {
            source = transform;
        }

        Quaternion rotation = useSpawnersRotation ? source.rotation : transform.rotation;
        ParticleSystem newParticle = Instantiate(particle, source.position, rotation, particlesParent);
        newParticle.gameObject.SetActive(true);
        newParticle.Play();

        Destroy(newParticle.gameObject, newParticle.main.duration + 0.05f);
        //Debug.Log("Spawned unparented particle");
    }
}
