using Spine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Spine.Unity;

public class PlayerController : MonoBehaviour
{
    PlayerManager _playerManager;
    private Bone _aimBone;
    private CancellationTokenSource _cts;
    private bool _isProcessing = false;
    private const float UpdateInterval = 0.016f; // 약 60fps
    private string _currentAnimation = "idle";
    private float _moveSpeed = 3f;
    private bool _isAiming = false;

    PlayerManager.PlayerState _pastPlayerState = PlayerManager.PlayerState.Idle;
    
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.SetAnimation(_playerManager.SelectedAnimation);
        _aimBone = _playerManager.SelectedBone;
        _cts = new CancellationTokenSource();
        UpdateAimBoneAsync(_cts.Token).Forget();
    }

    void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private async UniTaskVoid UpdateAimBoneAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_aimBone != null && !_isProcessing)
            {
                _isProcessing = true;
                
                if (Input.GetButtonDown("Fire1") && !_isAiming)
                {
                    _isAiming = true;
                    _playerManager.SetAnimation("aim", 2);
                }
                else if (Input.GetButtonUp("Fire1") && _isAiming)
                {
                    _isAiming = false;
                    _playerManager.skeletonAnimation.AnimationState.SetEmptyAnimation(2, 0.2f);
                }

                // 마우스 위치를 스켈레톤 로컬 공간으로 변환
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 localMousePosition =
                    _playerManager.skeletonAnimation.transform.InverseTransformPoint(mousePosition);

                // 스켈레톤 스케일 적용
                localMousePosition.x *= _playerManager.skeletonAnimation.Skeleton.ScaleX;
                localMousePosition.y *= _playerManager.skeletonAnimation.Skeleton.ScaleY;

                // 본에 위치 설정 (회전이 아닌 위치를 설정)
                _aimBone.SetLocalPosition(localMousePosition);

                _isProcessing = false;
            }

            WasdInput();
            await UniTask.Delay(TimeSpan.FromSeconds(UpdateInterval), cancellationToken: cancellationToken);
        }
    }


    private void WasdInput()
    {
        bool isMoving = false;

        // 좌우 이동 및 방향 전환 처리
        if (Input.GetKey(KeyCode.A))
        {
            isMoving = true;
            _playerManager.skeletonAnimation.Skeleton.ScaleX = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            _playerManager.skeletonAnimation.Skeleton.ScaleX = 1;
        }


        // 애니메이션 상태 결정
        if (Input.GetKey(KeyCode.W))
        {
            ChangePlayerState(PlayerManager.PlayerState.Walk);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ChangePlayerState(PlayerManager.PlayerState.Run);
        }
        else if (!isMoving && _pastPlayerState != PlayerManager.PlayerState.Idle)
        {
            ChangePlayerState(PlayerManager.PlayerState.Idle);
        }
    }
    
    //플레이어 상태 변경 
    private void ChangePlayerState(PlayerManager.PlayerState newState)
    {
        if (_pastPlayerState == newState)
            return;

        _pastPlayerState = newState;

        switch (newState)
        {
            case PlayerManager.PlayerState.Idle:
                _playerManager.SetAnimation("idle");
                break;
            case PlayerManager.PlayerState.Walk:
                _playerManager.SetAnimation("walk");
                break;
            case PlayerManager.PlayerState.Run:
                _playerManager.SetAnimation("run");
                break;
        }
    }
    
    
}