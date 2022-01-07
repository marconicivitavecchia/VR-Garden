
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace VowganVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DeckResetButton : UdonSharpBehaviour
    {

        public DeckManager DeckOfCards;
        
        private Animator anim;
        private int hashTrigger;


        private void Start()
        {
            anim = GetComponent<Animator>();
            hashTrigger = Animator.StringToHash("Trigger");
        }

        public override void Interact()
        {
            DeckOfCards.ResetDeck();
            anim.SetTrigger(hashTrigger);
        }
    }
}