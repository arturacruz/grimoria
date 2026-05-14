using UnityEngine;

public class Cooldown
{
    public float timeSeconds;
    private float startTime;
    public bool started;

    public Cooldown(float timeSeconds)
    {
        this.timeSeconds = timeSeconds;
        Restart();
    }

    public float ElapsedTimeSec() => started ? Time.time - startTime : 0;

    public bool IsDone() => ElapsedTimeSec() >= timeSeconds; 

    public void Restart() => startTime = Time.time;
}