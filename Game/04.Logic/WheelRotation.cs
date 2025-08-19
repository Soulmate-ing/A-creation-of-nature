using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WheelRotation : MonoBehaviour
{
    [Header("��ת����")]
    public float rotateSpeed = 100f;
    public float smoothFactor = 50f;

    private Rigidbody2D rb;
    private float targetRotation;
    public SpriteRenderer ropeSprite;

    [Header("��Ч")]
    public SoundName instrumental;
    private float timeSinceLastSound = 0f;
    public float soundInterval = 0.5f; // ��Ч���ż��ʱ�䣨�룩

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    void Update()
    {
        HandleRotationInput();
        ApplyRotation();
    }

    void HandleRotationInput()
    {
        // ֻ������ת����
        if (Input.GetKey(KeyCode.E) && ropeSprite.size.x > 0)
        {
            targetRotation -= rotateSpeed * Time.deltaTime;
            timeSinceLastSound += Time.deltaTime;
            if (timeSinceLastSound >= soundInterval)
            {
                if (instrumental != SoundName.none)
                {
                    var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(instrumental);
                    EventHandler.CallInitSoundEffect(soundDetails);
                }
                timeSinceLastSound = 0f; 
            }
        }
        else if (Input.GetKey(KeyCode.Q) && ropeSprite.size.x < 12f)
        {
            targetRotation += rotateSpeed * Time.deltaTime;
            timeSinceLastSound += Time.deltaTime;
            if (timeSinceLastSound >= soundInterval)
            {
                if (instrumental != SoundName.none)
                {
                    var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(instrumental);
                    EventHandler.CallInitSoundEffect(soundDetails);
                }
                timeSinceLastSound = 0f; 
            }
        }
        else
        {
            // ���ֹͣ���̣����ü�ʱ��
            timeSinceLastSound = 0f;
        }
    }

    void ApplyRotation()
    {
        float currentRotation = Mathf.Lerp(rb.rotation, targetRotation, Time.deltaTime * smoothFactor);
        rb.MoveRotation(currentRotation);
    }
}