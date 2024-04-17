using Oculus.Interaction.Input;
using Oculus.Interaction.MoveFast;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    

    [SerializeField] private HandPoseActiveStateList _leftStateList;
    [SerializeField] private HandPoseActiveStateList _rightStateList;

    [SerializeField] private RawHandVelocity _leftVelocity;
    [SerializeField] private RawHandVelocity _rightHandVelocity;

    public static HandPoseActiveStateList GetStateList(Handedness hand) =>
        hand == Handedness.Left ? _instance._leftStateList : _instance._rightStateList;

    public static RawHandVelocity GetRawHandVelocity(Handedness hand) =>
        hand == Handedness.Left ? _instance._leftVelocity : _instance._rightHandVelocity;

    private void Awake()
    {
        _instance = this;
    }
}
