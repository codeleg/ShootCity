using UnityEngine;
using System.Diagnostics;

public class PerformanceDisplay : MonoBehaviour
{
    [Header("Settings")]
    public Color textColor = Color.white;
    public int fontSize = 14;
    public float updateInterval = 0.5f;

    private float fps;
    private float cpuMs;
    private long gcAllocKb;
    private long monoUsedKb;
    private float timePassed;
    private int frames;

    private GUIStyle style;
    private Rect rect;

    void Start()
    {
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        rect = new Rect(10, 10, 300, 100);
    }

    void Update()
    {
        frames++;
        timePassed += Time.unscaledDeltaTime;

        if (timePassed >= updateInterval)
        {
            fps = frames / timePassed;
            cpuMs = (timePassed / frames) * 1000f;

            gcAllocKb = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024;
            monoUsedKb = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / 1024;

            frames = 0;
            timePassed = 0f;
        }
    }

    void OnGUI()
    {
        string text =
            $"FPS: {fps:F1}\n" +
            $"CPU: {cpuMs:F1} ms\n" +
            $"GC Alloc: {gcAllocKb} KB\n" +
            $"Mono Used: {monoUsedKb} KB";

        GUI.Label(rect, text, style);
    }
}
