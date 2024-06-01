using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Deck definition")]
    public GameObject deckAnchorGO;
    public string[] cardsSuits;

    public int cardsIndexStart;
    public int cardsInOneSuit;

    [Header("Card Definition")]

    public Sprite cardBack;
    public Sprite[] cardFronts;

    public Card prefabCard;
    private Dictionary<string, Sprite> dictOfCardFronts;


    [Header("Set Dynamically")]

    public List<string> cardNames;
    public List<Card> cards;

    public void InitDeck()
    {
        if (deckAnchorGO == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchorGO = anchorGO;
        }

        MakeDictOfCardsFronts();
        MakeCards();
    }

    public void MakeDictOfCardsFronts() {
        dictOfCardFronts = new Dictionary<string, Sprite>();
        foreach (Sprite tSprite in cardFronts) {
            dictOfCardFronts.Add(tSprite.name, tSprite);
        }
    }

    public void MakeCards()
    {
        cardNames = new List<string>();
        
        foreach (string s in cardsSuits)
        {
            for (int i = cardsIndexStart; i <= cardsIndexStart + cardsInOneSuit -1; i++)
            {
                string txt = s + string.Format("{0:D2}", i);
                cardNames.Add(txt);
            }
        }

        cards = new List<Card>();

        for (int i = 0; i < cardNames.Count; i++)
        {
            cards.Add(MakeCard(i));
        }
    }

    private Card MakeCard(int cNum = 0)
    {
        Card tCard = Instantiate(prefabCard);
        tCard.transform.parent = deckAnchorGO.transform;

        tCard.SetCard(
            cardsSuits[cNum / cardsInOneSuit]
            , (cNum% cardsInOneSuit) + 1
            , dictOfCardFronts[cardNames[cNum]]
            , cardBack
            );
        return tCard;
    }

}
