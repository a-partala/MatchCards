using System;
using UnityEngine;


[Serializable]
public class AnimationSettingsBase
{
    public float Duration = 1f;
}

[Serializable]
public class AnimationSettingsCurve : AnimationSettingsBase
{
    public AnimationCurve Curve;
}

[Serializable]
public class AnimationSettingsShake : AnimationSettingsBase
{
    public AnimationCurve Curve;
    public Vector3 EulerCoef;
}

[Serializable]
public class AnimationSettings : AnimationSettingsBase
{
    public Easing.Type Ease = Easing.Type.Linear;
}

[Serializable]
public class AnimationSettingsPaused : AnimationSettings
{
    //public float Pause = 0;
}