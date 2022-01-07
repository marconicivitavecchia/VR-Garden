using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;

namespace VowganVR
{
    public class DeckGenerator : EditorWindow
    {
    
        public GameObject DeckParent;
        public DeckManager Manager;
        
    
        [MenuItem("Tools/Vowgan/Deck Generator")]
        public static void ShowWindow()
        {
            EditorWindow win = GetWindow<DeckGenerator>("Deck Generator");
            win.minSize = new Vector2(50, 50);
            win.Show();
        }
        
        private void OnGUI()
        {
            DeckParent = (GameObject)EditorGUILayout.ObjectField(DeckParent, typeof(GameObject), true);
            Manager = (DeckManager)EditorGUILayout.ObjectField(Manager, typeof(DeckManager), true);
            
            if (GUILayout.Button("Generate Deck"))
            {
                foreach (Transform cardt in DeckParent.transform)
                {
                    GameObject card = cardt.gameObject;
                    Rigidbody rb = card.GetOrAddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                    card.GetOrAddComponent<VRCPickup>();
                    
                    VRCObjectSync objectSync = card.GetOrAddComponent<VRCObjectSync>();
                    objectSync.AllowCollisionOwnershipTransfer = false;

                    CardPickup cPickup = card.GetOrAddComponent<CardPickup>();
                    cPickup.DeckManager = Manager;
                    UdonSharpEditorUtility.ConvertToUdonBehaviours(new UdonSharpBehaviour[] {cPickup});

                    BoxCollider bCollider = card.GetOrAddComponent<BoxCollider>();
                    bCollider.center = new Vector3(0, 0.001f, 0);
                    bCollider.size = new Vector3(0.14f, 0.002f, 0.2f);
                }
            }
        }
    }
}