
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace VowganVR
{
    public class DeckManager : UdonSharpBehaviour
    {
        
        [UdonSynced, FieldChangeCallback(nameof(CardCountCallback))] public int CardCount;
        [UdonSynced] public int CardCurrent;
        
        [Header("References")]
        public Transform Deck;
        [HideInInspector] public VRCObjectPool Pool;
        
        private VRCPlayerApi playerLocal;
        private CardPickup[] cards;
        private GameObject currentCard;
        
        
        private void Start()
        {
            playerLocal = Networking.LocalPlayer;
            Pool = (VRCObjectPool) GetComponent(typeof(VRCObjectPool));
            
            cards = new CardPickup[Pool.Pool.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = Pool.Pool[i].GetComponent<CardPickup>();
            }
            
            if (Networking.IsOwner(gameObject)) SendCustomEventDelayedSeconds(nameof(ResetDeck), 1);
        }
        
        public int CardCountCallback
        {
            get => CardCount;
            set
            {
                CardCount = value;
                
                if (CardCount <= 0)
                {
                    Deck.localScale = Vector3.zero;
                }
                else
                {
                    Deck.localScale = new Vector3(1, CardCount + 0.002f, 1);
                }
            }
        }


        public void NextCard()
        {
            if (CardCurrent >= Pool.Pool.Length - 1)
            {
                Deck.localScale = Vector3.zero;
            }
            else
            {
                CardCountCallback -= 1;
                CardCurrent += 1;
                RequestSerialization();
                
                Networking.SetOwner(playerLocal, Pool.gameObject);
                currentCard = Pool.TryToSpawn();
                Networking.SetOwner(playerLocal, currentCard);
                
                currentCard.transform.localPosition = new Vector3(0,  CardCount * 0.002f, 0);
                VRCObjectSync sync = (VRCObjectSync) currentCard.GetComponent(typeof(VRCObjectSync));
                if (sync) sync.FlagDiscontinuity();
            }
        }
        
        public void ShuffleDeck()
        {
            Pool.Shuffle();
        }
        
        public void ResetDeck()
        {
            Networking.SetOwner(playerLocal, gameObject);
            
            foreach (CardPickup card in cards)
            {
                Networking.SetOwner(playerLocal, card.gameObject);
                card.Grabbed = false;
                card.RequestSerialization();
                Pool.Return(card.gameObject);
                if (!card.gameObject.activeSelf) continue;
                card._Drop();
            }
            
            CardCurrent = -1;
            CardCountCallback = Pool.Pool.Length;
            
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ShuffleDeck));
            NextCard();
        }

        public void ReturnCard()
        {
            CardCountCallback += 1;
            CardCurrent -= 1;
            currentCard.transform.localPosition = new Vector3(0,  CardCount * 0.002f, 0);
        }
        
    }
}