using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Card Definition")]
    public string suit;
    public int rank;

    public Sprite spriteFront;
    public Sprite spriteBack;

    [Header("Card Characteristics")]
    [SerializeField] protected float moveTimeDuration = 0.1f;
    [SerializeField] protected float timeStartMove = -1f;

    [Header("Set Dynamically Card Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] public Slot cardSlot;

    [Header("Set Dybamically Algaritm variables")]
    [SerializeField] protected List<Slot> availableSlots;
    [SerializeField] public Slot lastAvailableSlot;
    [SerializeField] public Slot crntSlot;

    protected List<Vector3> pts;
    public virtual Vector3 pos
    {
        set
        {
            timeStartMove = Time.time;
            pts = new List<Vector3>() { transform.position, value };
        }
    }

    public Vector3 posImmediate
    {
        set
        {
            transform.position = value;
        }
    }

    protected Vector3 pointerOffset;

    protected bool _isFrontSide;
    public bool isFrontSide {
        get { return _isFrontSide; }
        set 
        {
            if (value)
            {
                _isFrontSide = true;
                spriteRenderer.sprite = spriteFront;
            }
            else 
            {
                _isFrontSide = false;
                spriteRenderer.sprite = spriteBack;
            }
        }
    }

    /*
    Functions     
     */

    public virtual void Awake() {
        _isFrontSide = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cardSlot = GetComponent<Slot>();
        pts= new List<Vector3>();
    }

    public virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (timeStartMove == -1) return;
        float deltaTime = (Time.time - timeStartMove) / moveTimeDuration;
        deltaTime = Mathf.Clamp01(deltaTime);
        Vector3 tPos = Utils.Util.Bezier(deltaTime, pts);
        transform.position = tPos;

        if (deltaTime == 1)
        {
            timeStartMove = -1;
            EndMoving();
        }
    }

    public void SetCard(string suit, int rank, Sprite spriteFront, Sprite spriteBack) {
        this.suit = suit;
        this.rank = rank;
        this.spriteFront = spriteFront;
        this.spriteBack = spriteBack;

        gameObject.name = suit + string.Format("{0:D2}",rank);
        spriteRenderer.sprite = spriteFront;
    }

    protected virtual void EndMoving() {
        
        CardAudioControl.Instance.PlayOneCard();
    }

    public virtual void OnMouseDown() {
        /*
        Vector3 pointerPos = Camera.main.ScreenToWorldPoint(PlayerControlManager.Instance.mousePos);
        posImmediate = new Vector3(pointerPos.x, pointerPos.y, 0);

        if (crntSlot != null) {
            UpdateCrntSlotOnMouseIntercation();
        }
        */
        Vector3 pointerPos = Camera.main.ScreenToWorldPoint(PlayerControlManager.Instance.mousePos);
        pointerOffset = transform.position - pointerPos;
    }

    public virtual void OnMouseDrag() {
        if (!PlayerControlManager.IS_DRAG) return;
        Vector3 pointerPos = Camera.main.ScreenToWorldPoint(PlayerControlManager.Instance.mousePos);

        posImmediate = new Vector3(pointerPos.x + pointerOffset.x, pointerPos.y+ pointerOffset.y, GameplayManagerSiegeSol.MOVING_CARD_ANCHOR.position.z);

        if (crntSlot != null)
        {
            UpdateCrntSlotOnMouseIntercation();
        }
    }

    public virtual void UpdateCrntSlotOnMouseIntercation() {
        crntSlot.RemoveChildren(this);
        gameObject.transform.SetParent(GameplayManagerSiegeSol.MOVING_CARD_ANCHOR);
        crntSlot = null;
    }

    public virtual void OnMouseUp() {
        pointerOffset = Vector3.zero;
        if (availableSlots.Count > 0)
        {
            lastAvailableSlot = availableSlots[availableSlots.Count - 1];
        }
        if (lastAvailableSlot != null)
        {
            MoveToSlot(lastAvailableSlot, true);
        }
        availableSlots.Clear();
    }

    public virtual void MoveToSlot(Slot newSlot, bool saveToHist) {
        lastAvailableSlot = newSlot;
        crntSlot = newSlot;
        SetParentSlot(newSlot);
        availableSlots.Clear();
    }

    public virtual void SetParentSlot(Slot newSlot) {

        newSlot.SetChildren(this);

        pos = newSlot.container.position;
        gameObject.transform.SetParent(newSlot.container);
        crntSlot = newSlot;
        UpdateThisCardSlotToParent(newSlot);
    }

    public void UpdateThisCardSlotToParent(Slot newSlot) {

        cardSlot.UpdateCardSlotParamtr(
            newSlot.childrenSlotPosOffset,
            newSlot.childrenSlotRotOffset
            );
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == gameObject) return;
        if (other.CompareTag("Card") || other.CompareTag("Slot")) {
            Slot otherSlot = other.GetComponent<Slot>();
            if (otherSlot != null && otherSlot.IsAvailable() && !availableSlots.Contains(otherSlot))
            {
//                Debug.Log(gameObject.name + "___" + other.name + "___" + otherSlot.IsAvailable());
                availableSlots.Add(otherSlot);
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == gameObject) return;
        if (other.CompareTag("Card") || other.CompareTag("Slot"))
        {
            Slot otherSlot = other.GetComponent<Slot>();
            if (otherSlot != null && availableSlots.Contains(otherSlot))
            {

                availableSlots.Remove(otherSlot);
            }
        }
    }

    protected virtual void Destroy()
    {
        if(crntSlot!=null) crntSlot.RemoveChildren(this);
        Destroy(this.gameObject);
    }
}

