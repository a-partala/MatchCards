using AYellowpaper.SerializedCollections;
using UnityEngine;
public class Easing : MonoBehaviour
{
    public enum Type
    {
        Linear,
        In,
        Out,
        InOut,
        Bounce,
    }
    private static Easing instance = null;
    public static Easing Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<Easing>();
            }
            return instance;
        }
    }
    [SerializedDictionary("Ease type", "Animation Curve")]
    [SerializeField] private SerializedDictionary<Type, AnimationCurve> easeMap = new SerializedDictionary<Type, AnimationCurve>();

    public static AnimationCurve GetEasing(Type type)
    {
        if (!Instance.easeMap.ContainsKey(type))
        {
            return null;
        }

        return Instance.easeMap[type];
    }
}

public static class EasingExtensions
{
    public static float Evaluate(this Easing.Type type, float t)
    {
        var e = Easing.GetEasing(type);
        var v = e.Evaluate(t);
        return v;
    }
}
