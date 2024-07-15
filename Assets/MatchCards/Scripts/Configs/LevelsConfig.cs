using System;
using UnityEngine;

[Serializable]
public struct Level
{
    public int PairsAmount;
    public float Timer;
}

[CreateAssetMenu(fileName = "LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public Level[] levels;
}
