using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private PlayArea playArea;
    private float scaleCoef;
    private Card[,] cardsMatrix;
    public event Action OnAllCardsMatched;
    private Vector3 shiftCenterBy;

    private Vector3 normalizedScale
    {
        get
        {
            if (cardsMatrix == null)
            {
                return Vector3.zero;
            }
            int matrixWidth = cardsMatrix.GetLength(0);
            int matrixHeight = cardsMatrix.GetLength(1);
            return new Vector3(deck.CardScale.x * matrixWidth + deck.Spacing.x * (matrixWidth - 1), deck.CardScale.y * matrixHeight + deck.Spacing.y * (matrixHeight - 1));
        }
    }

    private Vector3 totalScale
    {
        get
        {
            return normalizedScale * scaleCoef;
        }
    }

    public void PrepareLevel(Level level)
    {
        InitMatrix(level.PairsAmount);
        scaleCoef = playArea.GetRescaleCoef(normalizedScale);
        deck.CreateCards(level.PairsAmount, scaleCoef);
        shiftCenterBy = playArea.transform.position - (GetRawGridPos(0, 0) + GetRawGridPos(cardsMatrix.GetLength(0) - 1, cardsMatrix.GetLength(1) - 1)) / 2f;
        ArrangeCards();
        StartCoroutine(GetCardsAnim());
    }

    private void InitMatrix(int pairsAmount)
    {
        var bounds = GetMatrixBounds(pairsAmount);
        cardsMatrix = new Card[bounds.x, bounds.y];
    }

    public void ArrangeCards()
    {
        if (deck.Cards.Count == 0)
        {
            Debug.LogError("Deck>> There are no cards to arrange");
            return;
        }
        List<Card> cards = new(deck.Cards);
        for (int i = 0; i < cardsMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cardsMatrix.GetLength(1); j++)
            {
                var card = cards.GetRandom();
                cards.Remove(card);
                cardsMatrix[i, j] = card;
                card.transform.position = new Vector3(0, -10f, 0);
            }
        }
    }

    private IEnumerator GetCardsAnim()
    {
        Coroutine routine = null;
        for (int j = cardsMatrix.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < cardsMatrix.GetLength(0); i++)
            {
                AnimationSettings anim = new();
                anim.Ease = Easing.Type.Out;
                anim.Speed = 0.04f * Vector3.Distance(new Vector3(0, -10f, 0), GetGridPos(i, j));
                routine = cardsMatrix[i, j].MoveTo(GetGridPos(i, j), anim);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private Vector3 GetGridPos(int i, int j)
    {
        return GetRawGridPos(i, j) + shiftCenterBy;
    }

    private Vector3 GetRawGridPos(int i, int j)
    {
        float widthPerCard = (deck.Spacing.x + deck.CardScale.x) * i;
        float heightPerCard = (deck.Spacing.y + deck.CardScale.y) * j;
        var halfAreaScale = new Vector3(playArea.transform.localScale.x, playArea.transform.localScale.y, 0) / 2f;
        Vector3 halfCardScale = deck.CardScale / 2f;
        return (new Vector3(widthPerCard, heightPerCard, 0) + halfCardScale) * scaleCoef + playArea.transform.position - halfAreaScale;
    }

    private Vector2Int GetMatrixBounds(int pairsAmount)
    {
        int cardsAmount = pairsAmount * deck.GroupSize;
        int width = deck.GroupSize;
        int height = pairsAmount;
        //the closer the squareness is to 0, the closer the ratio is to 1
        float squareness = Mathf.Abs(width / (float)height - height / (float)width);
        for (int i = 1; i <= pairsAmount - deck.GroupSize; i++)
        {
            int curWidth = deck.GroupSize + i;
            if (cardsAmount % curWidth > 0)
            {
                continue;
            }
            int curHeight = cardsAmount / curWidth;
            float curSquareness = Mathf.Abs(curWidth / (float)curHeight - curHeight / (float)curWidth);

            if (curSquareness < squareness)
            {
                squareness = curSquareness;
                width = curWidth;
                height = curHeight;
            }
        }

        return new Vector2Int(width, height);
    }
}
