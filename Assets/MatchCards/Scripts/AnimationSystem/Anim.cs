using System;
using System.Collections;
using UnityEngine;

public static class Anim
{
    public static Coroutine RotateTo(this MonoBehaviour mono, Vector3 targetEulers, AnimationSettings animSettings, Action finalAction = null)
    {
        var startEulers = mono.transform.position;
        void frameAction(float t)
        {
            mono.transform.localEulerAngles = Vector3.Lerp(startEulers, targetEulers, t);
        }

        return LerpTo(mono, frameAction, animSettings, () =>
        {
            mono.transform.localEulerAngles = targetEulers;
            finalAction?.Invoke();
        });
    }

    public static Coroutine MoveTo(this MonoBehaviour mono, Vector3 targetPos, AnimationSettings animSettings, Action finalAction = null)
    {
        var startPos = mono.transform.position;
        void frameAction(float t)
        {
            mono.transform.position = Vector3.Lerp(startPos, targetPos, t);
        }

        return LerpTo(mono, frameAction, animSettings, () =>
        {
            mono.transform.position = targetPos;
            finalAction?.Invoke();
        });
    }

    public static Coroutine ScaleTo(this MonoBehaviour mono, Vector3 targetScale, AnimationSettings animSettings, Action finalAction = null)
    {
        var startScale = mono.transform.localScale;
        void frameAction(float t)
        {
            mono.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        }

        return LerpTo(mono, frameAction, animSettings, () =>
        {
            mono.transform.localScale = targetScale;
            finalAction?.Invoke();
        });
    }

    public static Coroutine LerpTo(this MonoBehaviour mono, Action<float> frameAction, AnimationSettings animSettings, Action finalAction = null)
    {
        if(mono == null)
        {
            Debug.LogError($"Anim>> Object you are trying to animate is null");
            return null;
        }
        if(animSettings.Speed <= 0)
        {
            Debug.LogError($"Anim>> Incorrect {mono.name} anim's duration");
            return null;
        }
        return mono.StartCoroutine(LerpRoutine(frameAction, animSettings, finalAction));
    }

    private static IEnumerator LerpRoutine(Action<float> frameAction, AnimationSettings animSettings, Action finalAction = null)
    {
        float t = 0;
        float exp = 0;

        while (t < 1f)
        {
            exp += Time.deltaTime;
            t = exp / animSettings.Speed;

            frameAction?.Invoke(animSettings.Ease.Evaluate(t));

            yield return null;
        }

        frameAction?.Invoke(1f);
        finalAction?.Invoke();
    }
}
