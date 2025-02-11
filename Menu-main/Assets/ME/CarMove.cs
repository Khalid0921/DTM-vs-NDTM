using UnityEngine;

public class CarUIMove : MonoBehaviour
{
    public float moveDistance = 800f;  // Distance the car moves
    public float moveSpeed = 2f;       // Speed of movement
    public AudioSource carAudio;       // Car engine sound

    private RectTransform carRect;
    private Vector2 startPosition;
    private Vector2 offScreenPosition;
    private bool isMoving = false;

    void Start()
    {
        carRect = GetComponent<RectTransform>();  // Get UI RectTransform
        startPosition = carRect.anchoredPosition; // Save initial position
        offScreenPosition = startPosition + new Vector2(moveDistance, 0);
    }

    public void OnCarClick()
    {
        if (!isMoving)
        {
            isMoving = true;
            PlayCarSound(); // Play car sound
            MoveCarOffScreen();
        }
    }

    void MoveCarOffScreen()
    {
        // Move car off-screen
        LeanTween.move(carRect, offScreenPosition, moveSpeed)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     carRect.anchoredPosition = startPosition - new Vector2(moveDistance, 0);

                     // Move it back to start
                     LeanTween.move(carRect, startPosition, moveSpeed)
                              .setEase(LeanTweenType.easeInOutQuad)
                              .setOnComplete(() => 
                              {
                                  isMoving = false;
                                  StopCarSound(); // Stop sound after movement
                              });
                 });
    }

    void PlayCarSound()
    {
        if (carAudio != null)
        {
            carAudio.Play();
        }
    }

    void StopCarSound()
    {
        if (carAudio != null)
        {
            carAudio.Stop();
        }
    }
}
