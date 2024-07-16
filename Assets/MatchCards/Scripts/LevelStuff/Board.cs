using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField] private AnimationSettings getCardsAnim;
    [SerializeField] private Vector3 cardsStartPoint = new Vector3(0, -10f, 0);
    [SerializeField] private Deck deck;
    [SerializeField] private PlayArea playArea;
    [SerializeField] private TextMeshProUGUI timerTMP;
    private float scaleCoef;
    private Card[,] cardsMatrix;
    private Vector3 centerOffset;
    private int pairsLeft;
    private Timer timer;
    public event Action OnAllCardsMatched;
    public event Action OnFailed;

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

    public void Initialize()
    {
        timer = new(this);
        timer.OnChanged += UpdateTime;
        timer.OnTimeout += () => OnFailed?.Invoke();
        deck.OnMatchEvent += OnMatch;
    }
    
    private void UpdateTime(float seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);
        string template;
        if(seconds > 10)
        {
            template = "{0:D2}:{1:D2}";
        }
        else
        {
            template = "<color=red>{0:D2}:{1:D2}</color>";
        }
        timerTMP.text = string.Format(template, (int)time.Minutes, (int)time.Seconds);
    }

    public void PrepareLevel(Level level)
    {
        PrepareTimer(level.TimerInSeconds);
        PrepareCards(level);
        StartCoroutine(GetCardsAnim());
    }

    private void PrepareCards(Level level)
    {
        pairsLeft = level.PairsAmount;
        InitMatrix(level.PairsAmount);
        scaleCoef = playArea.GetRescaleCoef(normalizedScale);
        deck.CreateCards(level.PairsAmount, scaleCoef);
        ComputeCenterOffset();
        ArrangeCards();
    }

    private void ComputeCenterOffset()
    {
        var minPos = GetRawGridPos(0, 0);
        var maxPos = GetRawGridPos(cardsMatrix.GetLength(0) - 1, cardsMatrix.GetLength(1) - 1);
        var levelCenter = (minPos + maxPos) / 2f;
        centerOffset = playArea.transform.position - levelCenter;
    }

    private void PrepareTimer(float seconds)
    {
        UpdateTime(seconds);
        timer.Start(seconds, true);
    }

    private void InitMatrix(int pairsAmount)
    {
        var bounds = GetMatrixBounds(pairsAmount);
        cardsMatrix = new Card[bounds.x, bounds.y];
    }

    private void OnMatch()
    {
        pairsLeft--;
        if(pairsLeft <= 0)
        {
            timer.Pause();
            OnAllCardsMatched?.Invoke();
        }
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
                card.transform.position = cardsStartPoint;
            }
        }
    }

    private IEnumerator GetCardsAnim()
    {
        TouchController.AddPauseReason(gameObject);//Stop interactions
        float getCardsStartSpeed = getCardsAnim.Speed;
        for (int j = cardsMatrix.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < cardsMatrix.GetLength(0); i++)
            {
                getCardsAnim.Speed = getCardsStartSpeed * Vector3.Distance(cardsStartPoint, GetGridPos(i, j));//To make anim look more natural. Long path reqiuers more time 
                cardsMatrix[i, j].MoveTo(GetGridPos(i, j), getCardsAnim);
                yield return new WaitForSeconds(0.05f);
            }
        }
        getCardsAnim.Speed = getCardsStartSpeed;
        yield return new WaitForSeconds(1f);//extra time to remember cards

        for (int j = cardsMatrix.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < cardsMatrix.GetLength(0); i++)
            {
                cardsMatrix[i, j].SetState(Card.State.Back);
                yield return new WaitForSeconds(0.05f);
            }
        }
        TouchController.RemovePauseReason(gameObject);
        timer.Resume();
    }

    private Vector3 GetGridPos(int i, int j)
    {
        return GetRawGridPos(i, j) + centerOffset;
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
