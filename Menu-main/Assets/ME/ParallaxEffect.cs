using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform[] layers; // Assign UI Image layers here
    public float[] parallaxScales; // Adjust movement intensity per layer
    public float smoothing = 5f; // Smoothness of movement

    private Vector3 _previousMousePosition;

    void Start()
    {
        _previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        Vector3 delta = Input.mousePosition - _previousMousePosition;

        for (int i = 0; i < layers.Length; i++)
        {
            Vector3 targetPos = layers[i].position + new Vector3(delta.x * parallaxScales[i], delta.y * parallaxScales[i], 0);
            layers[i].position = Vector3.Lerp(layers[i].position, targetPos, smoothing * Time.deltaTime);
        }

        _previousMousePosition = Input.mousePosition;
    }
}
