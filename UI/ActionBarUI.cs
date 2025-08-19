using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionBarUI : MonoBehaviour
{
    [Tooltip("Player对象所在的场景名称")]
    public string playerSceneName = "PlayerScene"; // 根据实际场景名称修改

    private SpriteRenderer _targetRenderer;
    private SpriteRenderer _playerRenderer;
    private GameObject _playerObject;

    void Awake()
    {
        _targetRenderer = GetComponent<SpriteRenderer>();
        if (_targetRenderer == null)
        {
            Debug.LogError("目标物体需要SpriteRenderer组件", gameObject);
            enabled = false;
        }

        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        FindPlayerReference();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 当目标场景加载时重新查找Player
        if (scene.name == playerSceneName)
        {
            FindPlayerReference();
        }
    }

    void Update()
    {
        if (_playerRenderer != null)
        {
            _targetRenderer.enabled = _playerRenderer.enabled;
        }
    }

    private void FindPlayerReference()
    {
        // 通过标签查找Player（确保Player有"Player"标签）
        _playerObject = GameObject.FindWithTag("Player");

        if (_playerObject != null)
        {
            _playerRenderer = _playerObject.GetComponent<SpriteRenderer>();
            if (_playerRenderer == null)
            {
                Debug.LogWarning("找到的Player对象没有SpriteRenderer组件", _playerObject);
            }
        }
        else
        {
            Debug.LogWarning($"在场景 {playerSceneName} 中未找到Player对象");
        }
    }

    void OnDestroy()
    {
        // 取消注册事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}