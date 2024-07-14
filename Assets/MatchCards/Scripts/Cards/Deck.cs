using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private float spacing = 0.2f;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private TexturesConfig cardFaces;
    [SerializeField] private TexturesConfig cardBacks;

    [HideInInspector] public List<Card> Cards = new();
    private int backID = 0;
    private int groupSize = 2;
    public event Action OnMatch;

    public void CreateCards(int pairsAmount)
    {
        RemoveCards();
        var back = GetBackMaterial(backID);
        for (int i = 0; i < pairsAmount; i++)
        {
            var texture = cardFaces.textures[i];
            var face = new Material(baseMaterial);
            face.mainTexture = texture;

            for(int j = 0; j < groupSize; j++)
            {
                var card = Pool.Get(cardPrefab);
                card.Initialize(back, face);
                Cards.Add(card);
            }
        }
    }

    private void RemoveCards()
    {
        foreach (var card in Cards)
        {
            Pool.Return(card.gameObject);
        }
        Cards.Clear();
    }

    private Material GetBackMaterial(int id)
    {
        if (id < 0 || id >= cardBacks.textures.Length)
        {
            Debug.LogError("Deck>> Card back's id is over the array bounds");
            id = 0;
        }

        var backMaterial = new Material(baseMaterial);
        backMaterial.mainTexture = cardBacks.textures[id];
        return backMaterial;
    }
}
