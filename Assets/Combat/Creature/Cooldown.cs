using UnityEngine;

public class Cooldown
{
    private float timeSeconds;
    private float startTime;

    public Cooldown(float timeSeconds)
    {
        this.timeSeconds = timeSeconds;
    }

    public bool IsDone()
    {
        return Time.time - startTime >= timeSeconds; 
    }

    public void Restart()
    {
        startTime = Time.time;
    }
}