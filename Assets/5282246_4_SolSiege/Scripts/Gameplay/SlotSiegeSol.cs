using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotSiegeSolState { 
    Free,
    ChosenOne,
    Locked
}

public enum SlotSiegePos
{
    Left,
    Middle,
    Right,
}

public class SlotSiegeSol : Slot
{
    [Header("Set In Inspector")]

    public SlotSiegePos slotSiegePos;

    private SlotSiegeSolState _slotSiegeSolState;
    public SlotSiegeSolState slotSiegeSolState {
        get { return _slotSiegeSolState; }
        set {
            _slotSiegeSolState = value;
        }
    }
    public override void RemoveChildren(Card card) {

        if (childrenCard != card)
        {
            Debug.Log(gameObject.name + " try to remove card" + card.name);
            return;
        }
        else
        {
            childrenCard = null;
        }
    }

    public void UpdateCardStatusDown() {
        if (slotCard != null)
        {
            CardSiegeSol cardSiegeSol = (CardSiegeSol)slotCard;
            cardSiegeSol.UpdateCardStatusDown();
        }
    }

    public override bool IsAvailable()
    {
        return base.IsAvailable();
    }

    public void OnMouseUp()
    {
        if (slotCard == null && childrenCard == null)
        {
            GameplayManagerSiegeSol.Instance.SlotClicked(this);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    public void ResizeRow() {

        float width, step;
        switch (slotSiegePos) { 
            case SlotSiegePos.Left:
                width = transform.position.x - ((-1)*GameplayManagerSiegeSol.Instance.xArea);
                step = Mathf.Clamp(
                    width / Mathf.Max(1, CardRowCount()),
                    GameplayManagerSiegeSol.Instance.xStepMin,
                    GameplayManagerSiegeSol.Instance.xStepMax
                    );
                childrenSlotPosOffset = new Vector3(-step, 0, -1);

                break;
            case SlotSiegePos.Right:
                width = GameplayManagerSiegeSol.Instance.xArea - transform.position.x;
                step = Mathf.Clamp(
                    width / Mathf.Max(1, CardRowCount()),
                    GameplayManagerSiegeSol.Instance.xStepMin,
                    GameplayManagerSiegeSol.Instance.xStepMax
                    );
                childrenSlotPosOffset = new Vector3(step, 0, -1);

                break;
            case SlotSiegePos.Middle:
                return;
        }
        if (childrenCard != null)
        {
            childrenCard.cardSlot.UpdateCardSlotParamtr(childrenSlotPosOffset, childrenSlotRotOffset);
        }
    }
}
