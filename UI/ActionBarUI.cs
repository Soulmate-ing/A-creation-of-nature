using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionBarUI : MonoBehaviour
{
    [Tooltip("Player�������ڵĳ�������")]
    public string playerSceneName = "PlayerScene"; // ����ʵ�ʳ��������޸�

    private SpriteRenderer _targetRenderer;
    private SpriteRenderer _playerRenderer;
    private GameObject _playerObject;

    void Awake()
    {
        _targetRenderer = GetComponent<SpriteRenderer>();
        if (_targetRenderer == null)
        {
            Debug.LogError("Ŀ��������ҪSpriteRenderer���", gameObject);
            enabled = false;
        }

        // ע�᳡�������¼�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        FindPlayerReference();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��Ŀ�곡������ʱ���²���Player
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
        // ͨ����ǩ����Player��ȷ��Player��"Player"��ǩ��
        _playerObject = GameObject.FindWithTag("Player");

        if (_playerObject != null)
        {
            _playerRenderer = _playerObject.GetComponent<SpriteRenderer>();
            if (_playerRenderer == null)
            {
                Debug.LogWarning("�ҵ���Player����û��SpriteRenderer���", _playerObject);
            }
        }
        else
        {
            Debug.LogWarning($"�ڳ��� {playerSceneName} ��δ�ҵ�Player����");
        }
    }

    void OnDestroy()
    {
        // ȡ��ע���¼�
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}