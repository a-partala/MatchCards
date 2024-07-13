using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private float spacing = 0.2f;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Material baseMaterial;

    public void PrepareLevel(LevelsManager.LevelData data)
    {
        CreateCards(data.PairsAmount);
    }

    public void CreateCards(int pairsAmount)
    {

    }
}
