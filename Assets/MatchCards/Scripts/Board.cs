using System;
using static LevelsManager;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Deck deck;
    public event Action OnAllCardsMatched;

    public void PrepareGame(LevelData data)
    {
        deck.CreateCards(data.PairsAmount);
    }
}
