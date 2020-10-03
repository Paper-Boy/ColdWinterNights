using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public new bool enabled;
    public List<Light2D> lights;

    private readonly List<LightObject> lightObjects = new List<LightObject>();

    private struct LightObject
    {
        public Light2D light;
        public Vector2 standardPosition;

        public LightObject(Light2D light, Vector2 position)
        {
            this.light = light;
            standardPosition = position;
        }
    }

    public float minIntensity = 0.9f;
    public float maxIntensity = 1.0f;
    [Range(1, 50)]
    public int smoothing = 15;

    private Queue<Vector2> positionQueue;
    private Vector2 lastPosSum = Vector2.zero;

    private Queue<float> smoothQueue;
    private float lastSum = 0;

    private void Awake()
    {
        if (!enabled || !GameManager.instance.light)
            Destroy(this);

        GameManager.instance.fixedUpdate += FixedUpdateC;

        if (lights.Count == 0)
            lights.Add(gameObject.GetComponent<Light2D>());

        foreach (Light2D light in lights)
            lightObjects.Add(new LightObject(light, light.transform.localPosition));

        smoothQueue = new Queue<float>(smoothing);
        positionQueue = new Queue<Vector2>(smoothing * 5);
    }

    private void FixedUpdateC()
    {
        // pop off an item if too big
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calculate new smoothed average
        foreach (LightObject lightObject in lightObjects)
            lightObject.light.intensity = lastSum / smoothQueue.Count;

        // Smooth position
        while (positionQueue.Count >= smoothing * 5)
        {
            lastPosSum -= positionQueue.Dequeue();
        }

        Vector2 newPosVal = new Vector2();
        newPosVal.x = Random.Range(-0.001f, 0.001f);
        newPosVal.y = Random.Range(-0.001f, 0.001f);

        positionQueue.Enqueue(newPosVal);

        lastPosSum += newPosVal;

        foreach (LightObject lightObject in lightObjects)
        {
            Vector2 newPos = lightObject.light.transform.localPosition + (Vector3)(lastPosSum / positionQueue.Count);

            newPos.x = Mathf.Clamp(newPos.x, lightObject.standardPosition.x - 0.0075f, lightObject.standardPosition.x + 0.0075f);
            newPos.y = Mathf.Clamp(newPos.y, lightObject.standardPosition.y - 0.0075f, lightObject.standardPosition.y + 0.0075f);

            lightObject.light.transform.localPosition = newPos;
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.fixedUpdate -= FixedUpdateC;
    }
}
