using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Cards", menuName = "CardsData/CardObject", order = 1)]
public class CardDataScriptable : ScriptableObject
{
	public List<CardsData> m_Cardsdata;

}
public enum rank
{
	Ace = 14, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6,
	Seven = 7, Either = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13
}
public enum SuitEnum
{
	Hearts = 1,
	Clubs = 2,
	Diamonds = 3,
	Spades = 4,
}
[Serializable]
public class CardsData
{
	public int id;
	public SuitEnum Type;
	public rank Value;
	public Sprite cardFace;
	//public Sprite cardCover;
}

