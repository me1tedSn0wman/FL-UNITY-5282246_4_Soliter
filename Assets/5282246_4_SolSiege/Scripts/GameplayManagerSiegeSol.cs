using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase { 
    Idle
        ,Pre
        ,Layout_Init
        ,Layout_InitMove
        ,Layout_MoveCardToSlots
        ,Layout_EndMoveCardToSlot
        ,Waiting
        ,Post
        ,GameOver_Moving
        ,GameOver_End
        ,SomethingMoving
}

[Serializable]
public struct HistPair
{
    public CardSiegeSol card;
    public SlotSiegeSol slotFrom;

    public HistPair(CardSiegeSol card, SlotSiegeSol slotFrom) { 
        this.card = card;
        this.slotFrom = slotFrom;
    }
}

public class GameplayManagerSiegeSol : Singleton<GameplayManagerSiegeSol>
{
    [SerializeField] private TurnPhase _turnPhase;
    public TurnPhase turnPhase
    {
        get { return _turnPhase; }
        set { _turnPhase = value; }
    }

    [Header("Static")]
    public GameObject movingCardAnchorGO;
    public static Transform MOVING_CARD_ANCHOR;


    [Header("Game params")]
    public TextAsset deckXML;
    public TextAsset layoutXML;

    [Header("Layout")]
    [SerializeField] private List<Slot> initialSlots;

    [SerializeField] private GameObject canvas_MapGo;

    [SerializeField] protected SlotSiegeSol slotL0;
    [SerializeField] protected SlotSiegeSol slotL1;
    [SerializeField] protected SlotSiegeSol slotL2;
    [SerializeField] protected SlotSiegeSol slotL3;
    [SerializeField] protected SlotSiegeSol slotM0;
    [SerializeField] protected SlotSiegeSol slotM1;
    [SerializeField] protected SlotSiegeSol slotM2;
    [SerializeField] protected SlotSiegeSol slotM3;
    [SerializeField] protected SlotSiegeSol slotR0;
    [SerializeField] protected SlotSiegeSol slotR1;
    [SerializeField] protected SlotSiegeSol slotR2;
    [SerializeField] protected SlotSiegeSol slotR3;

    public float waitIntervalMoveCardsToSlots = 0.5f;

    [Header("Start Layout")]

    [SerializeField] private GameObject startLayoutAnchor;
    [SerializeField] public static Transform START_LAYOUT_ANCHOR;
    [SerializeField] private Vector3 startLayoutCardStepOffset;


    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardSiegeSol> drawPile;
    public Dictionary<string, CardSiegeSol> drawPileDict;

    public List<SlotSiegeSol> availableSlots;

    public HashSet<CardSiegeSol> waitMovingList;

    public int crtnCardIndex = 0;

    public CardSiegeSol cardClickedFirst;
    public CardSiegeSol cardClickedSecond;
    public SlotSiegeSol slotClicked;

    public float xLeft, xRight, xArea;

    public float xStepMin = 3f;
    public float xStepMax = 7f;


    public SlotSiegeSol[] sideSlots;
    public SlotSiegeSol[] middleSlots;
    public SlotSiegeSol[] allSlots;

    [Header("History")]
    public Stack<HistPair> history;

    private void Start()
    {
        turnPhase = TurnPhase.Pre;
        MOVING_CARD_ANCHOR = movingCardAnchorGO.transform;
        START_LAYOUT_ANCHOR = startLayoutAnchor.transform;
        waitMovingList = new HashSet<CardSiegeSol>();
        drawPileDict = new Dictionary<string, CardSiegeSol>();
        history = new Stack<HistPair>();

        sideSlots = new SlotSiegeSol[] {
                slotL0,
                slotL1,
                slotL2,
                slotL3,
                slotR0,
                slotR1,
                slotR2,
                slotR3,
            };

        middleSlots = new SlotSiegeSol[] {
                slotM0,
                slotM1,
                slotM2,
                slotM3,
            };

        allSlots = new SlotSiegeSol[] {
                slotL0,
                slotL1,
                slotL2,
                slotL3,
                slotM0,
                slotM1,
                slotM2,
                slotM3,
                slotR0,
                slotR1,
                slotR2,
                slotR3,
            };


        GameManager.OnScreenSizeChanged += Resize;
        Resize();

        deck = GetComponent<Deck>();
        deck.InitDeck();

        

        //       Deck.Shuffle(ref deck.cards);

        drawPile = UpgradeCardList(deck.cards);

        for (int i = 0; i < drawPile.Count; i++) {
            drawPileDict.Add(drawPile[i].name, drawPile[i]);
        }

        LayoutCards_MoveInit();
    }

    public void Update()
    {
        GameStateCheck();
    }

    public void GameStateCheck() {
        if (turnPhase == TurnPhase.Layout_InitMove && waitMovingList.Count == 0) {
            turnPhase = TurnPhase.Layout_MoveCardToSlots;
            Shuffle(ref drawPile);
            StartCoroutine(LayoutCards_MoveToSlots());
        }
        if (turnPhase == TurnPhase.Layout_EndMoveCardToSlot && waitMovingList.Count == 0) {
            CardsStatusUpdate();
            turnPhase = TurnPhase.Waiting;
        }
        if (turnPhase == TurnPhase.SomethingMoving && waitMovingList.Count == 0)
        {
            turnPhase = TurnPhase.Waiting;
            if (CheckForEndGameState()) {
                GameOver_Pre();
            }
        }
        if (turnPhase == TurnPhase.Waiting && waitMovingList.Count > 0)
        {
            turnPhase = TurnPhase.SomethingMoving;
        }
    }

    private void LayoutCards_MoveInit() {
        turnPhase = TurnPhase.Layout_Init;
        for (int i = 0; i < drawPile.Count; i++) {
            CardSiegeSol tCard = drawPile[i];
            tCard.posImmediate = startLayoutAnchor.transform.position + startLayoutCardStepOffset * i;
        }

        CardSiegeSol tTouseCard = new CardSiegeSol();
        if (drawPileDict.TryGetValue("Club01", out tTouseCard)) {
            tTouseCard.MoveToSlot(slotM0, false);
            drawPile.Remove(tTouseCard);
            tTouseCard.SetLockedForever();
        }
        if (drawPileDict.TryGetValue("Diamond01", out tTouseCard))
        {
            tTouseCard.MoveToSlot(slotM1, false);
            drawPile.Remove(tTouseCard);
            tTouseCard.SetLockedForever();
        }
        if (drawPileDict.TryGetValue("Heart01", out tTouseCard))
        {
            tTouseCard.MoveToSlot(slotM2, false);
            drawPile.Remove(tTouseCard);
            tTouseCard.SetLockedForever();
        }
        if (drawPileDict.TryGetValue("Spade01", out tTouseCard))
        {
            tTouseCard.MoveToSlot(slotM3, false);
            drawPile.Remove(tTouseCard);
            tTouseCard.SetLockedForever();
        }

        foreach (CardSiegeSol card in drawPile) {
            card.pos = startLayoutAnchor.transform.position;
            card.isFrontSide = false;
        }

        CardAudioControl.Instance.PlayShuffle();
        turnPhase = TurnPhase.Layout_InitMove;
    }

    public IEnumerator LayoutCards_MoveToSlots() {
        foreach (CardSiegeSol card in drawPile)
        {
            card.MoveToSlot(sideSlots[crtnCardIndex % sideSlots.Length].GetLastSlot() , false);
            card.isFrontSide = true;
            crtnCardIndex++;
            yield return new WaitForSeconds(waitIntervalMoveCardsToSlots);
        }
        turnPhase = TurnPhase.Layout_EndMoveCardToSlot;
    }

    public void CardsStatusUpdate() {
        foreach (SlotSiegeSol slot in sideSlots) {
            //            Debug.Log(slot.GetLastCard().name);
            ((CardSiegeSol)slot.GetLastCard()).UpdateCardStatusDown();
        }
    }

    List<CardSiegeSol> UpgradeCardList(List<Card> listCard) {
        List<CardSiegeSol> listCardsSiege = new List<CardSiegeSol>();
        foreach (Card tCard in listCard)
        {
            listCardsSiege.Add(tCard as CardSiegeSol);
        }
        return listCardsSiege;
    }

    public void AddCardToWaitMovingList(CardSiegeSol card) {
        if (!waitMovingList.Contains(card))
            waitMovingList.Add(card);
    }

    public void RemoveCardFromWaitmovingList(CardSiegeSol card) {
        waitMovingList.Remove(card);
    }

    static public void Shuffle(ref List<CardSiegeSol> oCards)
    {
        List<CardSiegeSol> tCards = new List<CardSiegeSol>();

        int ndx;
        while (oCards.Count > 0)
        {
            ndx = UnityEngine.Random.Range(0, oCards.Count);
            tCards.Add(oCards[ndx]);
            oCards.RemoveAt(ndx);
        }
        oCards = tCards;
    }

    public void CardClicked(CardSiegeSol newCard) {
        if (cardClickedFirst != null)
        {
            cardClickedSecond = newCard;
            if (cardClickedFirst != cardClickedSecond)
            {
                TryMoveCard(cardClickedFirst, (SlotSiegeSol)cardClickedSecond.cardSlot);
            }
            cardClickedFirst.UpdateCardStatusDown();
            cardClickedSecond.UpdateCardStatusDown();
            ClearClickedCard();
            slotClicked = null;
        }
        else {
            cardClickedFirst = newCard;
            slotClicked = null;
        }

    }

    public void SlotClicked(SlotSiegeSol newSlot)
    {
        slotClicked = newSlot;
        if (cardClickedFirst != null) {
            TryMoveCard(cardClickedFirst, newSlot);
            Debug.Log(cardClickedFirst.name);
            cardClickedFirst.UpdateCardStatusDown();
        }
        ClearClickedCard();
        slotClicked = null;
    }

    public void ClearClickedCard() {
        cardClickedFirst = null;
        cardClickedSecond = null;
    }

    public void TryMoveCard(CardSiegeSol card, SlotSiegeSol slotMoveTo) {
        CardSiegeSol cardMoveTo = (CardSiegeSol)slotMoveTo.slotCard;


        if (cardMoveTo == null) {
            if (
                slotMoveTo.slotSiegePos == SlotSiegePos.Middle 
                && card.cardSlot.childrenCard != null
                ) return;
            SlotSiegeSol prevSlot = (SlotSiegeSol)card.crntSlot;
            card.UpdateCrntSlotOnMouseIntercation();
            card.MoveToSlot(slotMoveTo, true);

            prevSlot.UpdateCardStatusDown();
            prevSlot.ResizeRow();
        }
        if (true
            && cardMoveTo != null
            && slotMoveTo.IsAvailable()
            && cardMoveTo.IsTopCardFollowOrder(card, true)

            /*
            && (
                !cardMoveTo.isMiddle && card.rank == (cardMoveTo.rank - 1)
                || cardMoveTo.isMiddle && card.rank == (cardMoveTo.rank + 1) && card.suit.Equals(cardMoveTo.suit)
                )
            
            */
            )
        {
            SlotSiegeSol prevSlot = (SlotSiegeSol)card.crntSlot;
            card.UpdateCrntSlotOnMouseIntercation();
            card.MoveToSlot(slotMoveTo, true);
            prevSlot.UpdateCardStatusDown();
            prevSlot.ResizeRow();
        }
    }

    public bool CheckForEndGameState() {
        foreach (SlotSiegeSol slot in allSlots) {

            //            Debug.Log("Check slot" + slot.name);
            CardSiegeSol firstCard = (CardSiegeSol)slot.childrenCard;

            if (firstCard != null) {
                bool check = firstCard.CheckCardRowStatus();
                if (!check) {
                    //                    Debug.Log(slot.name + " card start " + firstCard.name + "___" + check);
                    return false;
                }
            }
        }
        //        Debug.Log("Check");
        return true;
    }

    public void GameOver_Pre() {
        Debug.Log("Game OVer");
        turnPhase = TurnPhase.GameOver_Moving;

        StartCoroutine(GameOver_MoveToSlots());


    }

    public void GameOver_End() {
        turnPhase = TurnPhase.GameOver_End;
        GameplayUIManager.GAME_OVER();
    }

    public SlotSiegeSol TryFindPlaceForCardInMid(CardSiegeSol card) {

        foreach (SlotSiegeSol slot in middleSlots) {
            CardSiegeSol slotLastCard = (CardSiegeSol)slot.GetLastCard();
            if (slotLastCard.IsTopCardFollowOrder(card,true))
            {
                return (SlotSiegeSol)slotLastCard.cardSlot;
            }
        }
        return null;
    }

    public IEnumerator GameOver_MoveToSlots() {
 
        while (true)
        {
            int spaceSlots = 0;
            for (int i = 0; i < sideSlots.Length; i++) {
                SlotSiegeSol slot = sideSlots[i];
                CardSiegeSol card = (CardSiegeSol)(slot.GetLastCard());

                
                if (card == null) 
                {
                    spaceSlots++;
//                    Debug.Log("GAME_OVER SLOT" + slot.name + " CARD NULL ");
                }
                else
                {
                    SlotSiegeSol trySlot = TryFindPlaceForCardInMid(card);

                    if (trySlot != null)
                    {
//                        Debug.Log("GAME_OVER SLOT" + slot.name + " CARD " + card.name + " TO SLOT " + trySlot.name);
                        card.UpdateCrntSlotOnMouseIntercation();
                        card.MoveToSlot(trySlot, true);
                        yield return new WaitForSeconds(waitIntervalMoveCardsToSlots);
                    }
                    else { 
//                    Debug.Log("GAME_OVER SLOT" + slot.name + " CARD " + card.name + "SLOT NULL");

                    }
                }
            }
//            yield return new WaitForSeconds(waitIntervalMoveCardsToSlots);
            if (spaceSlots == sideSlots.Length) break; ;
        }
        GameOver_End();
    }

    public static bool IsTurnAvailable() {
        if (Instance.turnPhase == TurnPhase.Waiting) return true;
        return false;
    }

    public void SaveTurn(CardSiegeSol card, SlotSiegeSol slotFrom) {
        HistPair newHist = new HistPair(card, slotFrom);

        history.Push(newHist);
    }

    public void LoadTurn() {
        if (turnPhase == TurnPhase.Waiting)
        {
            HistPair loadHist;
            if (history.TryPop(out loadHist))
            {
                CardSiegeSol card = loadHist.card;
                SlotSiegeSol prevSlot = (SlotSiegeSol)card.crntSlot;
                card.UpdateCrntSlotOnMouseIntercation();
                loadHist.card.MoveToSlot(loadHist.slotFrom, false);
                prevSlot.UpdateCardStatusDown();
                prevSlot.ResizeRow();
                card.UpdateCardStatusDown();
            }
        }
    }

    public void Resize(Vector2Int newScreenSize) {
        Resize();
    }

    public void Resize() {
        Vector2 gameplayAreaXMinMax = GameplayUIManager.Instance.GetGameplayAreaXMinMax();
        Vector3 leftBorder, rightBorder;
        leftBorder = Camera.main.ScreenToWorldPoint(new Vector3(gameplayAreaXMinMax.x, 0, 0));
        rightBorder = Camera.main.ScreenToWorldPoint(new Vector3(gameplayAreaXMinMax.y, 0, 0));
        xLeft = leftBorder.x;
        xRight = rightBorder.x;
        xArea = Mathf.Min(Mathf.Abs(xLeft), Mathf.Abs(xRight));
        //    Debug.Log("Gamaplay Transform : " + leftBorder.x + "___" + rightBorder.x);

        for (int i = 0; i < sideSlots.Length; i++) {
            sideSlots[i].ResizeRow();
        }
    }

    public override void OnDestroy()
    {
        GameManager.OnScreenSizeChanged -= Resize;
        base.OnDestroy();
    }


}
