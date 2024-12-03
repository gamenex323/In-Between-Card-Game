using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

	public CardDataScriptable m_CardDataScriptable;
	public List<Card> m_AllCards;
	public GameObject cardPrefab;
	GameObject tempCard;

	void Start()
	{
		foreach (CardsData CD in m_CardDataScriptable.m_Cardsdata)
		{
			tempCard = Instantiate(cardPrefab, this.transform);
			//tempCard.GetComponent<Card>().InitCard();
			m_AllCards.Add(tempCard.GetComponent<Card>());
		}

	}
}
[System.Serializable]
public class CardData
{
	[SerializeField]
	public string name, cardName, type;
}
