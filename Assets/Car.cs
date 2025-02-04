using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dreamteck.Splines; // Make sure to include this for Spline Animate

public class Car : MonoBehaviour
{
    public SplineAnimate splineAnimator; // Reference to the Spline Animate component
    private StateNode currentState;
    public GameObject choiceUI;
    public Button optionA;
    public Button optionB;
    private bool isStopped = false;

    void Start()
    {
        choiceUI.SetActive(false);
        splineAnimator.Play(); // Start moving along the spline
    }

    public void EnterState(StateNode state)
    {
        currentState = state;
        isStopped = true;
        splineAnimator.Pause(); // Stop the car at the stop sign
        choiceUI.SetActive(true);
    }

    public void ChooseTransition(string input)
    {
        if (currentState.transitions.ContainsKey(input))
        {
            StateNode nextState = currentState.transitions[input];
            currentState = nextState;
        }

        isStopped = false;
        choiceUI.SetActive(false);
        splineAnimator.Play(); // Resume the car movement
    }
}
