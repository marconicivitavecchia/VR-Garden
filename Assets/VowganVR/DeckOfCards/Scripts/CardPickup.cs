
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace VowganVR
{
    public class CardPickup : UdonSharpBehaviour
    {
        
        public DeckManager DeckManager;
        [HideInInspector] [UdonSynced] public bool Grabbed;
        
        private VRC_Pickup pickup;
        private bool toBeReturned;
        
        
        private void Start()
        {
            pickup = (VRC_Pickup) GetComponent(typeof(VRC_Pickup));
        }
        
        public override void OnPickup()
        {
            if (Grabbed) return;
            Grabbed = true;
            DeckManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DeckManager.NextCard));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == DeckManager.Deck)
            {
                toBeReturned = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.transform == DeckManager.Deck)
            {
                toBeReturned = false;
            }
        }

        public void _Drop()
        {
            pickup.Drop();
        }
        
        public override void OnDrop()
        {
            if (toBeReturned)
            {
                Networking.SetOwner(Networking.LocalPlayer, DeckManager.gameObject);
                Grabbed = false;
                DeckManager.CardCount += 1;
                DeckManager.CardCurrent -= 1;
                DeckManager.Pool.Return(gameObject);
            }
        }
    }
}