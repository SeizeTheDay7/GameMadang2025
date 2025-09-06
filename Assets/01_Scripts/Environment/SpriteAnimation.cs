using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] float framePerSecond = 10f;
    [SerializeField, Min(0)] int startFrame = 0;
    [SerializeField, Min(0)] int endFrame = 0;
    [SerializeField] bool isPingpong;
    int direction = 1;
    SpriteRenderer spriteRenderer;
    int currentFrame;
    float timer;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
    }

    void Start()
    {
        currentFrame = startFrame;
        spriteRenderer.sprite = sprites[currentFrame];
        if(endFrame == 0 || endFrame > sprites.Length)
        {
            endFrame = sprites.Length;
        }
    }

    void Update()
    {
        if (sprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer < 1f / framePerSecond) return;

        timer = 0f;

        if (isPingpong)
        {
            currentFrame += direction;
            if (currentFrame >= endFrame || currentFrame < startFrame)
            {
                currentFrame = Mathf.Clamp(currentFrame, startFrame, endFrame - 1);
                direction *= -1;
            }
        }
        else
        {
            currentFrame = (currentFrame + 1) % endFrame;
        }

        spriteRenderer.sprite = sprites[currentFrame];
    }
}
