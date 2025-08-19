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
        private SpriteRenderer playerSprite; // 用于存储玩家的 SpriteRenderer 组件
        private BoxCollider2D boxCollider;
        private SpriteRenderer shadowSprite;  // 用于存储 Shadow 的 SpriteRenderer 组件
        private SpriteRenderer holdItemSprite; // 用于存储 HoldItem 的 SpriteRenderer 组件

        private void Awake()
        {
            // 获取玩家对象
            player = GameObject.Find("Player").transform.Find("player").gameObject;
            // 获取玩家对象上的 SpriteRenderer 组件
            playerSprite = player.GetComponent<SpriteRenderer>();
            boxCollider = player.GetComponent<BoxCollider2D>();
            shadowSprite = player.transform.Find("Shadow").GetComponent<SpriteRenderer>();
            holdItemSprite = player.transform.Find("HoldItem").GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // 在每次更新时检查玩家对象是否存在，如果不存在则重新获取
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