using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardSiegeSolState { 
    JustCreated,
    IsClicked,
    IsDragged,
    Moving,
    MovingToSlot,
    Locked,
    Locked_Forever,
    Free
}

public enum CardSiegePos { 
    Moving,
    Left,
    Middle,
    Right,
}

public class CardSiegeSol : Card
{
    CardSiegePos cardSiegePos;

    [SerializeField] private CardSiegeSolState _cardSiegeSolState;
    public CardSiegeSolState cardSiegeSolState
    {
        get { return _cardSiegeSolState; }
        set {
            switch (value) {
                case CardSiegeSolState.JustCreated:
                case CardSiegeSolState.IsDragged:
                case CardSiegeSolState.Moving:
                case CardSiegeSolState.MovingToSlot:
                case CardSiegeSolState.Locked:
                case CardSiegeSolState.Free:
                    glowingGO.SetActive(false);
                    break;
                case CardSiegeSolState.IsClicked:
                    glowingGO.SetActive(true);
                    break;
                default:
                    glowingGO.SetActive(false);
                    break;

            }
            _cardSiegeSolState = value;
//            Debug.Log(this.name + " ______ "+_cardSiegeSolState);
        }
    }

    public GameObject glowingGO;

    public override Vector3 pos
    {
        set
        {
            base.pos = value;
            GameplayManagerSiegeSol.Instance.AddCardToWaitMovingList(this);
            if(cardSiegeSolState != CardSiegeSolState.MovingToSlot)
                cardSiegeSolState = CardSiegeSolState.Moving;
        }
    }

    public override void Awake()
    {
        base.Awake();
        glowingGO.SetActive(false);
        cardSiegePos = CardSiegePos.Left;
        cardSiegeSolState = CardSiegeSolState.JustCreated;
    }

    public override void OnMouseDown()
    {
        if(!GameplayManagerSiegeSol.IsTurnAvailable()) return;
        if (false 
            || cardSiegeSolState == CardSiegeSolState.Moving
            || cardSiegeSolState == CardSiegeSolState.MovingToSlot
            || cardSiegeSolState == CardSiegeSolState.Locked
            ) return;
        //        Debug.Log(gameObject.name + " card clicked");
        cardSiegeSolState = CardSiegeSolState.IsClicked;
        GameplayManagerSiegeSol.Instance.CardClicked(this);
        base.OnMouseDown();
    }

    public override void OnMouseDrag()
    {
        if (!GameplayManagerSiegeSol.IsTurnAvailable()) return;
        if (false
            || cardSiegeSolState == CardSiegeSolState.Moving
            || cardSiegeSolState == CardSiegeSolState.MovingToSlot
            || cardSiegeSolState == CardSiegeSolState.Locked
            || cardSiegeSolState == CardSiegeSolState.Locked_Forever
            ) return;
        if (!PlayerControlManager.IS_DRAG) return;
        base.OnMouseDrag();
        GameplayManagerSiegeSol.Instance.ClearClickedCard();
        cardSiegePos = CardSiegePos.Moving;
        cardSiegeSolState = CardSiegeSolState.IsDragged;
    }

    public override void OnMouseUp()
    {
        if (!GameplayManagerSiegeSol.IsTurnAvailable()) return;
        if (false
            || cardSiegeSolState == CardSiegeSolState.Moving
            || cardSiegeSolState == CardSiegeSolState.MovingToSlot
            || cardSiegeSolState == CardSiegeSolState.Locked
            || cardSiegeSolState == CardSiegeSolState.Locked_Forever
            || cardSiegeSolState == CardSiegeSolState.IsClicked

            ) return;
        pointerOffset = Vector3.zero;
        if (cardSiegeSolState == CardSiegeSolState.IsDragged)
        {
            if (
                lastAvailableSlot != null
                && availableSlots.Count > 0
                && lastAvailableSlot != availableSlots[availableSlots.Count - 1]
                )
            {
                ((SlotSiegeSol)lastAvailableSlot).UpdateCardStatusDown();
                ((SlotSiegeSol)lastAvailableSlot).ResizeRow();
                MoveToSlot(availableSlots[availableSlots.Count - 1], true);
            }

            if (
                lastAvailableSlot != null
                && availableSlots.Count > 0
                && lastAvailableSlot == availableSlots[availableSlots.Count - 1]
                )
            {
                MoveToSlot(availableSlots[availableSlots.Count - 1], false);
            }

            if (availableSlots.Count == 0)
            {
                MoveToSlot(lastAvailableSlot, false);
            }
            availableSlots.Clear();
        }
        
    }

    public override void UpdateCrntSlotOnMouseIntercation()
    {
        ((SlotSiegeSol)crntSlot).RemoveChildren(this);
//        Debug.Log("try To Resize crnt slot");
        ((SlotSiegeSol)(crntSlot.GetFirstSlot())).ResizeRow();
        gameObject.transform.SetParent(GameplayManagerSiegeSol.MOVING_CARD_ANCHOR);
        crntSlot = null;
    }

    public override void SetParentSlot(Slot newSlot) { 
        base.SetParentSlot(newSlot);
        SlotSiegeSol newSlotSiege = (SlotSiegeSol)newSlot; 

//        isMiddle = ((SlotSiegeSol)newSlot).isMiddle;
        switch (newSlotSiege.slotSiegePos) {
            case SlotSiegePos.Left:
                cardSiegePos = CardSiegePos.Left;
                break;
            case SlotSiegePos.Right:
                cardSiegePos = CardSiegePos.Right;
                break;
            case SlotSiegePos.Middle:
                cardSiegePos = CardSiegePos.Middle;
                break;

            }
        ((SlotSiegeSol)cardSlot).slotSiegePos = newSlotSiege.slotSiegePos;
    }


    public void UpdateCardStatusDown() {
        UpdateCardStatus();
        if (crntSlot != null && crntSlot.slotCard != null)
        {
            ((CardSiegeSol)crntSlot.slotCard).UpdateCardStatusDown();
        }
    }

    public void UpdateCardStatus() {
        if (cardSiegeSolState == CardSiegeSolState.Locked_Forever
            || cardSiegeSolState == CardSiegeSolState.MovingToSlot
            || cardSiegeSolState == CardSiegeSolState.Moving
            ) {
            return;
        }
        if (cardSlot.childrenCard == null)
        {
            cardSiegeSolState = CardSiegeSolState.Free;
//            Debug.Log(gameObject.name + "card set Free, because there is no top");
            return;
        }
        if (((CardSiegeSol)cardSlot.childrenCard).cardSiegeSolState == CardSiegeSolState.Locked)
        {
            cardSiegeSolState = CardSiegeSolState.Locked;
//            Debug.Log(gameObject.name + "card set Locked, because top locked");
            return;
        }
        if (
            cardSlot.childrenCard != null
            && (cardSlot.childrenCard.rank + 1) == rank
            && ((CardSiegeSol)cardSlot.childrenCard).cardSiegeSolState == CardSiegeSolState.Free
            )
        {
//            Debug.Log(gameObject.name + "card set Free, from top rank");
            //            Debug.Log(gameObject.name + " try check this " + cardSlot.childrenCard.name);
            cardSiegeSolState = CardSiegeSolState.Free;
            return;
        }
//        Debug.Log(gameObject.name + "card set Locked, because nothing change");
        cardSiegeSolState = CardSiegeSolState.Locked;
    }

    protected override void EndMoving()
    {
        GameplayManagerSiegeSol.Instance.RemoveCardFromWaitmovingList(this);
//        Debug.Log(cardSiegeSolState);
        if (cardSiegeSolState == CardSiegeSolState.MovingToSlot)
        {
            SlotSiegeSol firsdtSlot = (SlotSiegeSol)crntSlot.GetFirstSlot();
            firsdtSlot.ResizeRow();
            //            Debug.Log("tryToresize");
            gameObject.transform.localPosition = Vector3.zero;
        }

        if (cardSiegeSolState!= CardSiegeSolState.Locked_Forever)
            cardSiegeSolState = CardSiegeSolState.Free;
        UpdateCardStatusDown();

        base.EndMoving();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == gameObject) return;
        SlotSiegeSol otherSlot = other.GetComponent<SlotSiegeSol>();
        if (IsOtherSlotAvailable(otherSlot))
            availableSlots.Add((SlotSiegeSol)otherSlot);

        /*
        if (other.CompareTag("Slot"))
        {
            SlotSiegeSol otherSlot = other.GetComponent<SlotSiegeSol>();
            if (otherSlot != null && otherSlot.IsAvailable() && !availableSlots.Contains(otherSlot))
            {
                //                Debug.Log(gameObject.name + "___" + other.name + "___" + otherSlot.IsAvailable());
                availableSlots.Add((SlotSiegeSol)otherSlot);
            }
        }
        if (other.CompareTag("Card")) {
            SlotSiegeSol otherSlot = other.GetComponent<SlotSiegeSol>();
            CardSiegeSol otherCard = other.GetComponent<CardSiegeSol>();

            if (true
                && otherSlot != null 
                && otherSlot.IsAvailable() 
                && (
                    !otherCard.isMiddle && rank == (otherCard.rank - 1) 
                    || otherCard.isMiddle && rank == (otherCard.rank + 1) && suit.Equals(otherCard.suit)
                    )
                && !availableSlots.Contains(otherSlot)
                )
            {
                //                Debug.Log(gameObject.name + "___" + other.name + "___" + otherSlot.IsAvailable());
                availableSlots.Add((SlotSiegeSol)otherSlot);
            }
        }
        */
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    public void SetFree() {
        cardSiegeSolState = CardSiegeSolState.Free;
        UpdateCardStatusDown();
    }

    public bool IsOtherSlotAvailable(SlotSiegeSol otherSlot) {

        CardSiegeSol otherCard = otherSlot.GetComponent<CardSiegeSol>();

        if (otherCard == null)
        {
            if (
                otherSlot != null 
                && otherSlot.IsAvailable() 
                && !availableSlots.Contains(otherSlot)
                && ( 
                    !(otherSlot.slotSiegePos == SlotSiegePos.Middle)
                    || cardSlot.childrenCard == null
                )
                )
            {
                //                Debug.Log(gameObject.name + "___" + other.name + "___" + otherSlot.IsAvailable());
                return true;
            }

        }
        else
        {
            if (true
                    && otherSlot != null
                    && otherSlot.IsAvailable()
                    && otherCard.IsTopCardFollowOrder(this, true)

                    /*
                    && (
                        !otherCard.isMiddle && rank == (otherCard.rank - 1)
                        || otherCard.isMiddle && rank == (otherCard.rank + 1) && suit.Equals(otherCard.suit)
                        )
                    */
                    && !availableSlots.Contains(otherSlot)
                    )
            {
                //                Debug.Log(gameObject.name + "___" + other.name + "___" + otherSlot.IsAvailable());
                return true;
            }
        }

        return false;
    }

    public bool IsTopCardFollowOrder(CardSiegeSol topCard, bool checkCardHaveChildren) {
        if (
            !(cardSiegePos == CardSiegePos.Middle) && (topCard.rank + 1) == rank
            || (cardSiegePos == CardSiegePos.Middle) && (topCard.rank - 1) == rank && suit.Equals(topCard.suit) && (!checkCardHaveChildren || topCard.cardSlot.childrenCard == null)
            ) return true;
        return false;
    }

    public bool CheckCardOrder()
    {
        if (cardSlot.childrenCard == null) return true;
        if (
            IsTopCardFollowOrder((CardSiegeSol)cardSlot.childrenCard, false)
            ) return true;
        return false;
    }

    public bool CheckCardRowStatus() {
        if (cardSlot.childrenCard == null) return true;
        if (
            IsTopCardFollowOrder((CardSiegeSol)cardSlot.childrenCard, false)
            && ((CardSiegeSol)cardSlot.childrenCard).CheckCardRowStatus()
            ) return true;
//        Debug.Log("CheckCardRowStatus() " + cardSlot.name  + "___" + cardSlot.childrenCard + "___" + isMiddle);
        return false;
    }

    public override void MoveToSlot(Slot newSlot, bool saveToHist)
    {
        cardSiegeSolState = CardSiegeSolState.MovingToSlot;
        //        Debug.Log("MOve to lsot ");
        if (saveToHist)
        {
            GameplayManagerSiegeSol.Instance.SaveTurn(this, (SlotSiegeSol)lastAvailableSlot);
        }
        base.MoveToSlot(newSlot, saveToHist);
    }

    public void SetLockedForever() {
        cardSiegeSolState = CardSiegeSolState.Locked_Forever;
    }
        

}
