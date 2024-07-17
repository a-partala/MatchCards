using System;
using System.Collections;
using UnityEngine;

public class Timer
{
    public event Action OnTimeout;
    public event Action<float> OnChanged;
    private MonoBehaviour mono;
    private bool isPaused = true;
    private float currentSeconds;
    private Coroutine coroutine;

    public Timer(MonoBehaviour mono)
    {
        this.mono = mono;
    }

    public void Start(float seconds, bool startPaused = false)
    {
        if (coroutine != null)
        {
            mono.StopCoroutine(coroutine);
        }
        isPaused = startPaused;
        currentSeconds = seconds;
        coroutine = mono.StartCoroutine(TimerRoutime());
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Increase(float seconds)
    {
        currentSeconds += seconds;
    }

    private IEnumerator TimerRoutime()
    {
        while(true)
        {
            if(isPaused)
            {
                yield return null;
                continue;
            }
            currentSeconds -= Time.deltaTime;
            OnChanged?.Invoke(currentSeconds);
            if (currentSeconds <= 0)
            {
                currentSeconds = 0;
                OnChanged?.Invoke(currentSeconds);
                OnTimeout?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
}
