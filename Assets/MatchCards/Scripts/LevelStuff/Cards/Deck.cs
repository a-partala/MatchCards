using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Vector2 CardScale = new Vector2();
    public Vector2 Spacing;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private TexturesConfig cardFaces;
    [SerializeField] private TexturesConfig cardBacks;

    [HideInInspector] public List<Card> Cards = new();
    private int backID = 0;
    public readonly int GroupSize = 2;
    public event Action OnMatchEvent;
    private List<Card> cardsBuffer = new();

    public void CreateCards(int pairsAmount, float scale = 1f)
    {
        DeleteCards();
        var back = GetBackMaterial();//1 back material for all cards
        for (int i = 0; i < pairsAmount; i++)
        {
            var texture = cardFaces.textures[i];
            var face = new Material(baseMaterial);//1 face material for "linked" cards
            face.mainTexture = texture;

            for(int j = 0; j < GroupSize; j++)
            {
                var card = Pool.Get(cardPrefab);
                card.transform.localScale = new Vector3(CardScale.x, CardScale.y, 1) * scale;
                card.Initialize(back, face);
                card.OnFlippedToFace += () =>
                {
                    AddToBuffer(card);
                };
                Cards.Add(card);
            }
        }
    }

    /// <summary>
    /// Resets turn cards
    /// </summary>
    public void ClearBuffer()
    {
        cardsBuffer.Clear();
    }

    /// <summary>
    /// Add card to the turn
    /// </summary>
    /// <param name="card"></param>
    private void AddToBuffer(Card card)
    {
        if(cardsBuffer.Count >= GroupSize)
        {
            return;
        }
        cardsBuffer.Add(card);

        if(cardsBuffer.Count == GroupSize)
        {
            TouchController.Clear();
            TouchController.TryAddPauseReason(gameObject);//because of next animations
            if (CheckMatch())
            {
                Invoke(nameof(CompleteBufferCards), 0.66f);
            }
            else
            {
                Invoke(nameof(FlipBufferCardsToBack), 0.66f);
            }
        }
    }

    /// <summary>
    /// Hides a turn's cards
    /// </summary>
    private void FlipBufferCardsToBack()
    {
        SetBuffer(Card.State.Back);
    }


    /// <summary>
    /// Completes the turn's cards
    /// </summary>
    private void CompleteBufferCards()
    {
        SetBuffer(Card.State.Hidden);
        OnMatchEvent?.Invoke();
    }

    private void SetBuffer(Card.State state)
    {
        foreach (var item in cardsBuffer)
        {
            item.SetState(state);
        }
        cardsBuffer.Clear();
        TouchController.RemovePauseReason(gameObject);
    }

    private bool CheckMatch()
    {
        var firstMaterial = cardsBuffer[0].GetFaceMat();

        for (int i = 1; i < cardsBuffer.Count; i++)
        {
            if (cardsBuffer[i].GetFaceMat() != firstMaterial)
            {
                return false;
            }
        }

        return true;
    }

    private void DeleteCards()
    {
        foreach (var card in Cards)
        {
            Pool.Return(card.gameObject);
        }
        Cards.Clear();
    }

    /// <summary>
    /// Allows you to use many back skins
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private Material GetBackMaterial()
    {
        if(backID == -1)
        {
            backID = SaveService.GetInt(nameof(backID), 0);
        }

        var backMaterial = new Material(baseMaterial);
        backMaterial.mainTexture = cardBacks.textures[backID];
        return backMaterial;
    }
    //
    public void SetBack(int n)
    {
        if(n < 0 || n >= cardBacks.textures.Length)
        {
            n = 0;
        }
        backID = n;
        SaveService.SetInt(nameof(backID), 0);
    }
}
