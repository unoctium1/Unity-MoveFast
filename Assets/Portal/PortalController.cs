using System;
using Meta.XR.MRUtilityKit;
using Oculus.Interaction;
using Oculus.Interaction.MoveFast;
using UnityEngine;
using UnityEngine.Playables;

public class PortalController : MonoBehaviour
{
    [SerializeField] private PlayableDirector _portalPrefab;

    [SerializeField] private RayInteractor _leftHand;
    [SerializeField] private RayInteractor _rightHand;

    [SerializeField] private MRUK.PositioningMethod _positioningMethod = MRUK.PositioningMethod.DEFAULT;

    [SerializeField] private StringPropertyBehaviour _appState;

    private bool _isLoaded = false;
    
    private readonly LabelFilter _filter = LabelFilter.FromEnum(MRUKAnchor.SceneLabels.WALL_FACE | MRUKAnchor.SceneLabels.SCREEN);

    private PlayableDirector _portal = null;
    
    private void OnEnable()
    {
        _leftHand.WhenStateChanged += LeftHandOnWhenStateChanged;
        _rightHand.WhenStateChanged  += RightHandOnWhenStateChanged;
    }
    
    private void OnDisable()
    {
        _leftHand.WhenStateChanged -= LeftHandOnWhenStateChanged;
        _rightHand.WhenStateChanged  -= RightHandOnWhenStateChanged;
        
    }

    private void Start()
    {
        MRUK.Instance?.RegisterSceneLoadedCallback(OnSceneLoaded);
    }

    private void LeftHandOnWhenStateChanged(InteractorStateChangeArgs args)
    {
        HandOnStateChanged(_leftHand, args);
    }
    
    private void RightHandOnWhenStateChanged(InteractorStateChangeArgs args)
    {
        HandOnStateChanged(_rightHand, args);
    }

    private void HandOnStateChanged(RayInteractor handRay, InteractorStateChangeArgs args)
    {
        if (!_isLoaded || _portal) return;

        if (args.NewState != InteractorState.Select && args.NewState != InteractorState.Hover) return;
        
        MRUKAnchor sceneAnchor = null;
        Pose? bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(handRay.Ray, Mathf.Infinity, _filter, out sceneAnchor, _positioningMethod);
        if (bestPose.HasValue && sceneAnchor && _portalPrefab)
        {
            SpawnPortal(bestPose.Value);
        }
    }

    private void OnSceneLoaded()
    {
        _isLoaded = true;
    }

    private void SpawnPortal(Pose pose)
    {
        _portal = Instantiate(_portalPrefab, pose.position, pose.rotation);
        var lookAtPos = _portal.transform.position;

        var camPosition = Camera.main.transform.position;
        camPosition.y = 0;
        lookAtPos.y = 0;
        gameObject.transform.position = camPosition;
        gameObject.transform.LookAt(lookAtPos);
        
        _leftHand.gameObject.SetActive(false);
        _rightHand.gameObject.SetActive(false);
        if (_appState.Value.Equals("findwall"))
        {
            _appState.Value = "guidelines";
        }
    }
}
