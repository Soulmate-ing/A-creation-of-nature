using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MyGame.Transition
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        public string startSceneName = string.Empty;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade;
        private new void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }
        private void OnEnable()
        {
            EventHandler.TransitiongEvent += OnTransitionEvent;
        }
        private void OnDisable()
        {
            EventHandler.TransitiongEvent -= OnTransitionEvent;
        }
        private IEnumerator Start()
        {
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();

            // 等待视频播放完毕事件
            yield return new WaitUntil(() => UIManager.Instance != null);
            EventHandler.VideoPlaybackFinishedEvent += OnVideoFinished;
        }

        private void OnVideoFinished()
        {
            // 视频结束后加载主场景
            StartCoroutine(LoadMainSceneAfterVideo());
        }
        private IEnumerator LoadMainSceneAfterVideo()
        {
            // 加载主场景并激活
            yield return SceneManager.LoadSceneAsync(startSceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(startSceneName));

            EventHandler.CallAfterSceneLoadedEvent();
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if (!isFade)
                StartCoroutine(Transition(sceneToGo, positionToGo));
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActice(sceneName);
            //移动人物坐标
            EventHandler.CallMoveToPosition(targetPosition);
            EventHandler.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }
        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        public IEnumerator LoadSceneSetActice(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

        }
        /// <summary>
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha"> 1是黑 0是透明 </param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            fadeCanvasGroup.blocksRaycasts = true;
            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;
            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }
            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }

    }
}