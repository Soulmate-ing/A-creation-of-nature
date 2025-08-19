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

            // �ȴ���Ƶ��������¼�
            yield return new WaitUntil(() => UIManager.Instance != null);
            EventHandler.VideoPlaybackFinishedEvent += OnVideoFinished;
        }

        private void OnVideoFinished()
        {
            // ��Ƶ���������������
            StartCoroutine(LoadMainSceneAfterVideo());
        }
        private IEnumerator LoadMainSceneAfterVideo()
        {
            // ����������������
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
        /// �����л�
        /// </summary>
        /// <param name="sceneName">Ŀ�곡��</param>
        /// <param name="targetPosition">Ŀ��λ��</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActice(sceneName);
            //�ƶ���������
            EventHandler.CallMoveToPosition(targetPosition);
            EventHandler.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }
        /// <summary>
        /// ���س���������Ϊ����
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <returns></returns>
        public IEnumerator LoadSceneSetActice(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

        }
        /// <summary>
        /// ���뵭������
        /// </summary>
        /// <param name="targetAlpha"> 1�Ǻ� 0��͸�� </param>
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