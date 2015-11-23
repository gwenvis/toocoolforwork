using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {

    public int skatingState;
    public int preparingState;
    public int ollieState;
    public int fallingState;
    public int grindState;
    public int landingState;
    public int shortLandState;
    public int dyingState;
    public int grindFront;
    public int grindBack;

    public int skateSkatingState;
    public int skateOllieState;
    public int skateGrind;
    public int skateKickflip;
    public int skateHeelflip;
    public int skateGrind2;

    void Awake()
    {
        dyingState = Animator.StringToHash("Base Layer.dying");
        preparingState = Animator.StringToHash("Base Layer.preparingtrick");
        ollieState = Animator.StringToHash("Base Layer.ollie");
        fallingState = Animator.StringToHash("Base Layer.falling");
        landingState = Animator.StringToHash("Base Layer.landing");
        shortLandState = Animator.StringToHash("Base Layer.shortlanding");
        skatingState = Animator.StringToHash("Base Layer.skating");
        grindState = Animator.StringToHash("Base Layer.grinding");
        grindFront = Animator.StringToHash("Base Layer.FrontGrind");
        grindBack = Animator.StringToHash("Base Layer.BackGrind");

        skateSkatingState = Animator.StringToHash("Base Layer.skateboard_skate");
        skateOllieState = Animator.StringToHash("Base Layer.skateboard_ollie");
        skateGrind = Animator.StringToHash("Base Layer.skateboard_grind");
        skateHeelflip = Animator.StringToHash("Base Layer.skateboard_heelflip");
        skateKickflip = Animator.StringToHash("Base Layer.skateboard_kickflip");
        skateGrind2 = Animator.StringToHash("Base Layer.skateboard_grind2");
    }
}
