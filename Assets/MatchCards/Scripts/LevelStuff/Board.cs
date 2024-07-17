using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField] private AnimationSettings getCardsAnim;
    [SerializeField] private AnimationSettings shuffleAnim;
    [SerializeField] private float pauseAtShuffle = 2f;
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
            float allCardsWidth = deck.CardScale.x * matrixWidth;
            float allXSpacing = deck.Spacing.x * (matrixWidth - 1);
            float allCardsHeight = deck.CardScale.y * matrixHeight;
            float allYSpacing = deck.Spacing.y * (matrixHeight - 1);
            return new Vector3(allCardsWidth + allXSpacing, allCardsHeight + allYSpacing);
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

    public void ShuffleCards()
    {
        StartCoroutine(ShuffleCardsRoutune());
    }

    private IEnumerator ShuffleCardsRoutune()
    {
        if(!TouchController.TryAddPauseReason(gameObject))
        {
            yield break;//Board is used for something else. Another shuffle or even starting level animation
        }
        deck.ClearBuffer();
        timer.Pause();//don't waste any time
        List<Card> cards = new(deck.Cards);//Independent list
        List<Vector2Int> completedPositions = new();//Positions that won't participate in shuffling
        List<Card> hiddenCards = new();//to fill empty cells in new matrix
        for (int i = 0; i < cardsMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cardsMatrix.GetLength(1); j++)
            {
                var card = cardsMatrix[i, j];
                if (card.MyState == Card.State.Hidden)
                {
                    cards.Remove(card);
                    hiddenCards.Add(card);
                    completedPositions.Add(new Vector2Int(i, j));
                }
            }
        }

        Card[,] shuffledMatrix = new Card[cardsMatrix.GetLength(0), cardsMatrix.GetLength(1)];
        for (int i = 0; i < shuffledMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < shuffledMatrix.GetLength(1); j++)
            {
                if(cards.Count == 0)
                {
                    break;
                }
                if (completedPositions.Contains(new Vector2Int(i, j)))
                {
                    shuffledMatrix[i, j] = hiddenCards[0];
                    hiddenCards.RemoveAt(0);
                    continue;
                }
                var card = cards.GetRandom();
                shuffledMatrix[i, j] = card;
                cards.Remove(card);
                card.MoveTo(GetCentralizedGridPos(i, j), shuffleAnim);//Just move card to its new home
            }
        }

        yield return new WaitForSeconds(shuffleAnim.Speed);

        for (int i = 0; i < deck.Cards.Count; i++)
        {
            if (deck.Cards[i].MyState != Card.State.Back)//Don't play unneccessary anim
            {
                continue;
            }
            deck.Cards[i].SetState(Card.State.Face);//Show card
        }

        yield return new WaitForSeconds(pauseAtShuffle);//couple of seconds to remember cards

        for (int i = 0; i < deck.Cards.Count; i++)
        {
            if (deck.Cards[i].MyState != Card.State.Face)//Don't play unneccessary anim
            {
                continue;
            }
            deck.Cards[i].SetState(Card.State.Back);//Hide card
        }

        cardsMatrix = shuffledMatrix;//apply new matrix
        TouchController.RemovePauseReason(gameObject);//Resume touch system
        timer.Resume();//continue timer count
    }

    private void ComputeCenterOffset()//Need to centralize resized level
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
        TouchController.TryAddPauseReason(gameObject);//Stop interactions
        float getCardsStartSpeed = getCardsAnim.Speed;
        for (int j = cardsMatrix.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < cardsMatrix.GetLength(0); i++)
            {
                getCardsAnim.Speed = getCardsStartSpeed * Vector3.Distance(cardsStartPoint, GetCentralizedGridPos(i, j));//To make anim look more natural. Long path reqiuers more time 
                cardsMatrix[i, j].MoveTo(GetCentralizedGridPos(i, j), getCardsAnim);
                yield return new WaitForSeconds(0.05f);
            }
        }
        getCardsAnim.Speed = getCardsStartSpeed;
        yield return new WaitForSeconds(1f);//time to remember cards

        for (int j = cardsMatrix.GetLength(1) - 1; j >= 0; j--)//flip to back
        {
            for (int i = 0; i < cardsMatrix.GetLength(0); i++)
            {
                cardsMatrix[i, j].SetState(Card.State.Back);
                yield return new WaitForSeconds(0.05f);
            }
        }
        TouchController.RemovePauseReason(gameObject);//Resume touch system
        timer.Resume();//Continue timer count
    }

    private Vector3 GetCentralizedGridPos(int i, int j)
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
        //the closer the 'unsquareness' is to 0, the closer the ratio is to 1
        float unsquareness = Mathf.Abs(width / (float)height - height / (float)width);
        for (int i = 1; i <= pairsAmount - deck.GroupSize; i++)
        {
            int curWidth = deck.GroupSize + i;
            if (cardsAmount % curWidth > 0)
            {
                continue;
            }
            int curHeight = cardsAmount / curWidth;
            float curSquareness = Mathf.Abs(curWidth / (float)curHeight - curHeight / (float)curWidth);

            if (curSquareness < unsquareness)
            {
                unsquareness = curSquareness;
                width = curWidth;
                height = curHeight;
            }
        }

        return new Vector2Int(width, height);
    }
}
