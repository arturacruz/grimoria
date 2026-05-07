using UnityEngine;

public class Cooldown
{
    private float timeSeconds;
    private float startTime;

    public Cooldown(float timeSeconds)
    {
        this.timeSeconds = timeSeconds;
        Restart();
    }

    public float ElapsedTimeSec() => Time.time - startTime;

    public bool IsDone() => ElapsedTimeSec() >= timeSeconds; 

    public void Restart() => startTime = Time.time;
}