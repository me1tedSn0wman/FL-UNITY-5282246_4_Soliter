using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Transform container;
    
    public Vector3 childrenSlotPosOffset;
    public Vector3 childrenSlotRotOffset;
    public Card slotCard;

    public Card childrenCard;
    
    [Header("Card Characteristics")]
    [SerializeField] protected float moveTimeDuration = 0.1f;
    [SerializeField] protected float timeStartMove = -1f;

    protected List<Vector3> pts = new List<Vector3>();

    public virtual Vector3 contLocPos
    {
        set
        {
            timeStartMove = Time.time;
            pts = new List<Vector3>() { container.transform.localPosition, value };
        }
    }

    public virtual void Awake()
    {
        slotCard = GetComponent<Card>();
    }

    protected virtual void Update() {
        Move();
    }

    protected virtual void Move()
    {
        if (timeStartMove == -1) return;
        float deltaTime = (Time.time - timeStartMove) / moveTimeDuration;
        deltaTime = Mathf.Clamp01(deltaTime);
        Vector3 tPos = Utils.Util.Bezier(deltaTime, pts);
        container.transform.localPosition = tPos;

        if (deltaTime == 1)
        {
            timeStartMove = -1;
            EndMoving();
        }
    }

    protected virtual void EndMoving()
    {

    }

    public virtual bool IsAvailable() {
        if (childrenCard != null) return false; 
//        Debug.Log("it's from base");
        return true;
    }

    public virtual void SetChildren(Card newCard) {

        if (childrenCard != null)
        {
            Debug.Log(gameObject.name + "there is already exists children" + newCard.name);
            return;
        }
        else
        {
            childrenCard = newCard;
        }
    }

    public virtual void RemoveChildren(Card card) {
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

    public void Resizing() { 
    }

    public Slot GetFirstSlot() {
        if (slotCard != null) {
            return slotCard.crntSlot.GetFirstSlot();
        }
        return this;
    }

    public int CardRowCount() {
        int count = 0;
        if (childrenCard != null)
        {
            count = childrenCard.cardSlot.CardRowCount()+1;
        }
        return count;
    }

    public Slot GetLastSlot()
    {
        if (childrenCard != null)
        {
            return childrenCard.cardSlot.GetLastSlot();
        }
        return this;
    }

    public Card GetLastCard() {
        if (childrenCard != null)
            return childrenCard.cardSlot.GetLastCard();
        return slotCard;
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
//        Debug.Log("This game OBject:" + gameObject.name + " enter to this " + other.name);
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
//        Debug.Log("This game OBject:" + gameObject.name + " left to this " + other.name);
    }

    public virtual void OnTriggerStay2D(Collider2D other) {
//        Debug.Log("LELELELELELELELEL");
    }

    public void UpdateCardSlotParamtr(Vector3 newPosOffset, Vector3 newRotOffset) {
        

        contLocPos = newPosOffset;
        container.transform.localRotation = Quaternion.Euler(newRotOffset);

        childrenSlotPosOffset = newPosOffset;
        childrenSlotRotOffset = newRotOffset;

        if (childrenCard != null) {
            childrenCard.cardSlot.UpdateCardSlotParamtr(newPosOffset, newRotOffset);
        }
    }
}