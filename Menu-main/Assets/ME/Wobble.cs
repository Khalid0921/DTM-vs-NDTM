using UnityEngine;
using TMPro;

public class TitleWobble : MonoBehaviour
{
    public float wobbleAmount = 10f;  // How much it wobbles (degrees)
    public float wobbleSpeed = 1.5f;  // Speed of the wobble

    void Start()
    {
        WobbleEffect();
    }

    void WobbleEffect()
    {
        LeanTween.rotateZ(gameObject, wobbleAmount, wobbleSpeed)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setLoopPingPong(); // Moves back and forth forever
    }
}
