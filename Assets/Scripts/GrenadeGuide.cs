using UnityEngine;
//this was pretty much all vibe coded ngl
public class GrenadeGuide : MonoBehaviour
{
    public LineRenderer line;
    public Transform player;
    public GameObject grenadePrefab;

    [Header("Throw")]
    public float maxDistance = 8f;
    public float chargeTime = 1.2f;

    [Header("Curve")]
    public float arcStrength = 1.5f;
    public int segments = 20;
    private const float travelTime = 1f;

    float holdTimer;
    bool charging;

    void Awake()
    {
        line.enabled = false;
        line.colorGradient = new Gradient();
    }
    public void StartGrenadeCharge()
    {
        charging = true;
        holdTimer = 0f;
        line.enabled = true;
    }
    void Update()
    {
        if (charging && Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;
            DrawArc();
        }

        if (Input.GetMouseButtonUp(0) && charging)
        {
            charging = false;
            LaunchGrenade();
        }
    }

    void DrawArc()
    {
        float t = Mathf.Clamp01(holdTimer / chargeTime);
        float distance = Mathf.Lerp(1f, maxDistance, t);

        Vector3 mouseWorld = GetMouseWorld();
        Vector2 dir = (mouseWorld - player.position).normalized;

        line.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float p = i / (float)(segments - 1);

            // straight throw
            Vector2 point = (Vector2)player.position + dir * distance * p;

            // upward arc (peaks in middle)
            float curve = Mathf.Sin(p * Mathf.PI) * arcStrength;
            point += Vector2.up * curve;

            line.SetPosition(i, point);
        }
    }

    Vector3 GetMouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(m);
    }

    void LaunchGrenade()
    {
        float t = Mathf.Clamp01(holdTimer / chargeTime);
        float distance = Mathf.Lerp(1f, maxDistance, t);

        Vector3 mouseWorld = GetMouseWorld();
        Vector2 dir = (mouseWorld - player.position).normalized;
        Vector2 startPos = player.position;

        GameObject grenade = Instantiate(
            grenadePrefab,
            startPos,
            Quaternion.identity
        );

        StartCoroutine(MoveGrenadeAlongArc(grenade.transform, dir, distance, startPos));
    }
    System.Collections.IEnumerator MoveGrenadeAlongArc(
        Transform grenade, Vector2 dir, float distance, Vector2 startPos) {
        float elapsed = 0f;
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(Color.white, 1f)
        };

        // Cache the current line positions and compute segment lengths (arc-length)
        int pc = Mathf.Max(0, line.positionCount);
        Vector3[] points = new Vector3[pc];
        for (int i = 0; i < pc; i++) points[i] = line.GetPosition(i);

        float[] segLengths = new float[Mathf.Max(0, pc - 1)];
        float totalLength = 0f;
        for (int i = 0; i < segLengths.Length; i++)
        {
            float l = Vector3.Distance(points[i], points[i + 1]);
            segLengths[i] = l;
            totalLength += l;
        }

        bool activated = false;
        while (elapsed < travelTime)
        {
            float t = elapsed / travelTime;
            // if grenade was destroyed, stop moving and reset gradient
            if (grenade == null) 
            {
                line.enabled = false;
                line.colorGradient = new Gradient();
                yield break;
            }

            // call activate() once when progress crosses threshold
            if (!activated && t >= Grenade.ACTIVATION_PROGRESS)
            {
                var gComp = grenade.GetComponent<Grenade>();
                if (gComp != null) gComp.activate();
                activated = true;
            }

            // Straight throw (parameterized by t)
            Vector2 pos = startPos + dir * distance * t;
            float curve = Mathf.Sin(t * Mathf.PI) * arcStrength;
            pos += Vector2.up * curve;
            grenade.position = pos;

            // Compute normalized progress along the line by arc-length (so gradient aligns with world positions)
            float normalized = 0f;
            if (totalLength > 0f && points.Length > 1)
            {
                // Find closest projection of grenade.position onto the polyline and compute arc-length to that point
                Vector2 g = grenade.position;
                float bestDist = float.MaxValue;
                float accumulated = 0f;
                float bestProgress = 0f;

                for (int i = 0; i < points.Length - 1; i++)
                {
                    Vector2 a = points[i];
                    Vector2 b = points[i + 1];
                    Vector2 ab = b - a;
                    float segLen = segLengths[i];
                    if (segLen <= 0f)
                    {
                        // degenerate segment
                        float d = Vector2.Distance(g, a);
                        if (d < bestDist)
                        {
                            bestDist = d;
                            bestProgress = accumulated / totalLength;
                        }
                        continue;
                    }

                    float proj = Vector2.Dot(g - a, ab) / (segLen * segLen);
                    float segT = Mathf.Clamp01(proj);
                    Vector2 closest = a + ab * segT;
                    float dist = Vector2.Distance(g, closest);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestProgress = (accumulated + segLen * segT) / totalLength;
                    }
                    accumulated += segLen;
                }

                normalized = Mathf.Clamp01(bestProgress);
            }

            // Fade line behind grenade based on normalized arc-length progress
            float lead = 0.01f; // tiny visible lead ahead of grenade
            GradientAlphaKey[] alphaKeys = {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0f, normalized),
                new GradientAlphaKey(1f, Mathf.Min(normalized + lead, 1f)),
                new GradientAlphaKey(1f, 1f)
            };

            gradient.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = gradient;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // if grenade still exists, snap to final position and call ground()
        if (grenade != null)
        {
            line.enabled = false;
            line.colorGradient = new Gradient();
            grenade.position = startPos + dir * distance;
            var gComp = grenade.GetComponent<Grenade>();
            if (gComp != null) gComp.ground();
        }

        //OnGrenadeLanded(grenade);
    }


}
