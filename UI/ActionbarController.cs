using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class ActionbarController : MonoBehaviour
{
    public GameObject actionBar;
    public bool isVisable = true; 

    public List<string> allowedScenes = new List<string> { "01.Field" };
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
    }
    private void OnAfterSceneLoadEvent()
    {
        // 检查当前场景是否在允许列表中
        if (allowedScenes.Contains(SceneManager.GetActiveScene().name))
        {
            isVisable = true;
        }
        else
        {
            isVisable = false;
        }
    }
    private void Update()
    {
        if (!isVisable)
        {
            actionBar.SetActive(false);
        }
        else
        {
            actionBar.SetActive(true);
        }
    }
}