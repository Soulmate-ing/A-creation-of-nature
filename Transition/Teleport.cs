using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame.Transition
{
    public class Teleport : MonoBehaviour
    {
        public string sceneToGo;
        public Vector3 positionToGo;
        private GameObject player;
        private SpriteRenderer playerSprite; // ���ڴ洢��ҵ� SpriteRenderer ���
        private BoxCollider2D boxCollider;
        private SpriteRenderer shadowSprite;  // ���ڴ洢 Shadow �� SpriteRenderer ���
        private SpriteRenderer holdItemSprite; // ���ڴ洢 HoldItem �� SpriteRenderer ���

        private void Awake()
        {
            // ��ȡ��Ҷ���
            player = GameObject.Find("Player").transform.Find("player").gameObject;
            // ��ȡ��Ҷ����ϵ� SpriteRenderer ���
            playerSprite = player.GetComponent<SpriteRenderer>();
            boxCollider = player.GetComponent<BoxCollider2D>();
            shadowSprite = player.transform.Find("Shadow").GetComponent<SpriteRenderer>();
            holdItemSprite = player.transform.Find("HoldItem").GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // ��ÿ�θ���ʱ�����Ҷ����Ƿ���ڣ���������������»�ȡ
            if (player == null)
            {
                player = GameObject.Find("Player").transform.Find("player").gameObject;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }

        public void OnButtonDownToPuzzle()
        {
            if (player != null)
            {
                if (playerSprite != null)
                {
                    playerSprite.enabled = false;
                }
                if(boxCollider != null)
                {
                    boxCollider.enabled = false;
                }
                if (shadowSprite != null)
                {
                    shadowSprite.enabled = false;
                }
                if (holdItemSprite != null)
                {
                    holdItemSprite.enabled = false;
                }
            }
            EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
        }

        public void OnButtonDownToField()
        {
            if (player != null)
            {
                if (playerSprite != null)
                {
                    playerSprite.enabled = true;
                }
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
                if (shadowSprite != null)
                {
                    shadowSprite.enabled = true;
                }
                if (holdItemSprite != null)
                {
                    holdItemSprite.enabled = true;
                }
            }
            EventHandler.CallTransitionEvent(sceneToGo, Talkable.GlobalReturnPosition);
            
        }
    }
}