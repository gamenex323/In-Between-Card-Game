using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;

public class CardDeckManager : ExtendedBehavior
{
	public static Action OnTwoCardFliped;
	//public static Action OnCenterCardFliped;
	public static Action OnCountDownCompleted;
	[SerializeField] float shuffleSpeed, leftRightSpace = 50f, cardStartY, cardDeckMarginY = .05f;
	[SerializeField] float[] dealPosX = new float[3] { 300, 0, -300 };
	[SerializeField] float dealPosY = 300f, dealSpeed;

	[SerializeField] CardDataScriptable m_CardDataScriptable;
	//[SerializeField] List<Card> m_AllCards;
	

	[SerializeField] GameObject cardPrefab, cardContainer;


	//Card ID list
	[SerializeField] List<Card> cardsList;
	[SerializeField] List<Card> dealCardList;
	[SerializeField] List<int> deckIds;
	
	List<int> topTheeCardIndex;

	public static CardDeckManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{

		//InitCard();
	}



	//********************Card List suffle**********************************************************


	

	


	//***********************************************************************************************
	public void InitCard(string newDeck)
	{
		cardsList = new List<Card>();
		deckIds = Utility.StringToIntList(newDeck);
		dealSpeed = GameSetting.Instance.cardDealTime;
		shuffleSpeed = GameSetting.Instance.cardShuffleTime;
		
		for (int i = 0; i < deckIds.Count; i++)
		{

			GameObject tempCard = Instantiate(cardPrefab, cardContainer.transform);
			tempCard.GetComponent<Card>().InitCard();
			tempCard.name ="Card_"+ i;
			cardsList.Add(tempCard.GetComponent<Card>());

		}
		
		SetCardDefaultDeckPos();
	}

	void SetCardDefaultDeckPos()
    {
		
		for (int i = 0; i < cardsList.Count; i++)
        {
			cardsList[i].transform.localPosition = Vector3.zero;			
			cardsList[i].transform.DOLocalMoveY(i * cardDeckMarginY, 0);
			cardsList[i].transform.DOLocalRotate(new Vector3(0, 0, 180), 0);
			cardsList[i].gameObject.SetActive(true);
			cardsList[i].ResetCard();
		}
	
		PlayShuffleCardDeckAnimation();

	}


	public void PlayShuffleCardDeckAnimation()
	{
		  

        for (int i = 0; i < cardsList.Count; i++)
        {

			cardsList[i].transform.DOLocalMoveY(cardStartY, 0);
			if (i % 2 == 0)
			{
				cardsList[i].transform.DOLocalMoveX(leftRightSpace, 0);

			}
			else
			{
				cardsList[i].transform.DOLocalMoveX(-leftRightSpace, 0);


			}
			float dly = ((shuffleSpeed/ cardsList.Count) * i);
			cardsList[i].transform.DOLocalMoveX(0, 0.25f).SetDelay(dly).SetEase(Ease.InSine);			
			if (i == cardsList.Count - 1)
				cardsList[i].transform.DOLocalMoveY(i * cardDeckMarginY, 0.25f).SetDelay(dly).OnComplete(OnDeckAnimationCompleted);
            else
            {
				cardsList[i].transform.DOLocalMoveY(i * cardDeckMarginY, 0.25f).SetDelay(dly);			
				

			}
			Wait(dly, () =>
			{
				AudioManager.Instance.PlaySound("CardDistribution");
			});

		}


	}
	void OnDeckAnimationCompleted()
	{		
		Debug.Log("OnDeckAnimationCompleted");

		if (GameManager.Instance.currentGameState == State.INPLAY)
		{
			GameManager.Instance.ReShuffleCompleted();
		}
		else
		{
			GameManager.Instance.IamReady();
		}

		//Wait(1, () => { MoveCard(); });
	}

	public void MoveThreeCard()
	{
		Debug.Log("MoveThreeCard");
		Debug.Log(GameManager.Instance.currentGameState);
		if (GameManager.Instance.currentGameState != State.INPLAY) return;
		//RemoveCard();
		topTheeCardIndex = GetTopThreeCardIds();
		Wait(GameSetting.Instance.cardTransistionGap, ()=>{ MoveCard(); });			

		
	}

	void MoveCard()
    {
		if (GameManager.Instance.currentGameState != State.INPLAY) return;
		dealCardList = GetTop3CardFromStack();
		for (int i = 0; i < dealCardList.Count; i++)
		{			
			dealCardList[i].transform.DOLocalMove(new Vector2(dealPosX[i], dealPosY), dealSpeed / dealCardList.Count).SetEase(Ease.InOutCubic).SetDelay(i * 0.1f);
			
			if(i == dealCardList.Count-1)
			dealCardList[i].transform.DORotate(Vector3.zero, dealSpeed / dealCardList.Count).SetEase(Ease.InOutCubic).SetDelay(i * 0.1f).OnComplete(FlipCardAfterMoveCompleted);
			else
				dealCardList[i].transform.DORotate(Vector3.zero, dealSpeed / dealCardList.Count).SetEase(Ease.InOutCubic).SetDelay(i * 0.1f);

			Wait(i * 0.1f, () =>
			{
				AudioManager.Instance.PlaySound("CardDistribution");
			});
		}
	}

    

    void FlipCardAfterMoveCompleted()
    {
		if (GameManager.Instance.currentGameState != State.INPLAY) return;
		Debug.Log("FlipCardAfterMoveCompleted");
		Wait(GameSetting.Instance.cardTransistionGap, () => { FlipTwoSideCard(); });
    }

	
	public void FlipTwoSideCard()
    {
		if (GameManager.Instance.currentGameState != State.INPLAY) return;
		for (int i = 0; i < topTheeCardIndex.Count; i++)
		{
			if (i != 1)
			{
				int cardIndex = topTheeCardIndex[i];
				CardsData cd = m_CardDataScriptable.m_Cardsdata.Find(x => x.id == cardIndex);				
				
				dealCardList[i].FlipFrontCard(cd.cardFace);				

			}
		}
		Wait(GameSetting.Instance.cardFlipTime,()=> { OnTwoCardFliped?.Invoke(); });

	}

	public void FlipCenterCard(Action _callback = null)
	{
		if (GameManager.Instance.currentGameState != State.INPLAY) return;
		int cardIndex = topTheeCardIndex[1];
		CardsData cd = m_CardDataScriptable.m_Cardsdata.Find(x => x.id == cardIndex);
		dealCardList[1].FlipFrontCard(cd.cardFace);		
		Wait(GameSetting.Instance.cardFlipTime, () => { _callback?.Invoke(); });

	}


	public void RemoveCard(Action OnCardRemoved)
	{
		//send card to outside of the screen with animation

		for (int i = 0; i < dealCardList.Count; i++)
		{
			//dealCardList[i].gameObject.SetActive(false);
			Card card = dealCardList[i];
			if (i == dealCardList.Count - 1)
				dealCardList[i].transform.DOLocalMove(new Vector2(1000, 1000), dealSpeed / dealCardList.Count).SetEase(Ease.InOutCubic).SetDelay(i * 0.1f).OnComplete(()=> { Destroy(card.gameObject); OnCardRemoved?.Invoke(); });
			else
				dealCardList[i].transform.DOLocalMove(new Vector2(1000, 1000), dealSpeed / dealCardList.Count).SetEase(Ease.InOutCubic).SetDelay(i * 0.1f).OnComplete(() => { Destroy(card.gameObject); });

		}
		RemoveToTheeCardIds();

	}


	//UTILITY METHOD **************************************************************************************************************

	//GETTER METHOD **************************************************************************************************************
	public List<int> GetTopThreeCardIds()
	{
		//string str = (string)PhotonNetwork.CurrentRoom.CustomProperties[CARD_DECK_IDS];
		List<int> ids = deckIds;
		List<int> tmp = new List<int>();

		for (int i = 0; i < 3; i++)
		{
			tmp.Add(ids[(ids.Count - 1) - i]);
		}

		return tmp;
	}

	List<Card> GetTop3CardFromStack() {
		

		List<Card> tmp = new List<Card>();
		
		for (int i = 0; i < 3; i++)
        {
			if (cardsList.Count > 0)
			{
				int index = cardsList.Count - 1;
				Card value = cardsList[index];
				cardsList.RemoveAt(index);
				tmp.Add(value);
			}					

		}

		return tmp;
	}
	public List<CardsData> GetTopThreeCardData()
    {
		List<CardsData> cardData = new List<CardsData>();
		for (int i = 0; i < topTheeCardIndex.Count; i++)
		{			
			int cardIndex = topTheeCardIndex[i];
			CardsData cd = m_CardDataScriptable.m_Cardsdata.Find(x => x.id == cardIndex);
			cardData.Add(cd);
			
		}
		return cardData;

	}

	public List<CardsData> GetCardData()
    {
		return m_CardDataScriptable.m_Cardsdata;

	}
	public int GetRemainCardCount()
    {
		return cardsList.Count;
	}
	

	//*********************************************************************************************************************************
	public void RemoveToTheeCardIds()
   {
		Debug.Log("RemoveToTheeCardIds");
		for (int i = 0; i < 3; i++)
		{
			if (deckIds.Count > 0)
			{			
				deckIds.RemoveAt(deckIds.Count - 1);				
			}

		}
		dealCardList.Clear();
	}
	
	public void ResetCards()
    {
        foreach (Transform child in cardContainer.transform)
		{
			GameObject.Destroy(child.gameObject);
			

		}

		deckIds.Clear();
		cardsList.Clear();

	}
}
