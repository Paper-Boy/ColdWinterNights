using UnityEngine;

public class TemperatureMap
{
    private readonly int width, height;
    private readonly float minTemp, maxTemp;

    private readonly float[,] map;

    public TemperatureMap(float minTemp, float maxTemp)
    {
        this.minTemp = minTemp;
        this.maxTemp = maxTemp;

        width = GameManager.instance.mapGenerator.width;
        height = GameManager.instance.mapGenerator.height;
        map = GameManager.instance.mapGenerator.noiseMap;
    }

    public float GetTemp(Vector2 position)
    {
        Vector2 buf = new Vector2();

        buf.x = Mathf.RoundToInt(0.5f * (50.0f * position.x + width));
        buf.y = Mathf.RoundToInt(0.5f * (height - 50 * position.y));

        buf.x = Mathf.Clamp(buf.x, 0, width - 1);
        buf.y = Mathf.Clamp(buf.y, 0, width - 1);

        float value = minTemp + map[(int)buf.x, (int)buf.y] * (maxTemp - minTemp);

        value -= Mathf.Clamp(GameManager.instance.GameTime / 240.0f * 0.1f, 0.0f, 0.8f);

        return value;
    }
}
