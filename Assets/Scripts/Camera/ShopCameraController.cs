using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class ShopCameraController : MonoBehaviour
{
    public enum CameraState
    {
        Free,
        Moving,
        Locked
    }

    [Serializable]
    public class CameraProfile
    {
        [Header("Camera")]
        public string cameraName;
        public CinemachineCamera virtualCamera;

        [Tooltip("Target followed by this Cinemachine camera.")]
        public Transform target;

        [Header("Movement")]
        public float moveSpeed = 8f;
        public float smoothTime = 0.15f;

        [Header("X Limits")]
        public float minX = 0f;
        public float maxX = 20f;

        [HideInInspector]
        public float targetX;

        [HideInInspector]
        public float velocityX;
    }

    [Header("Cameras")]
    [SerializeField] private CameraProfile[] cameras;

    [Header("Starting Camera")]
    [SerializeField] private int startingCameraIndex = 0;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineBrain cinemachineBrain;

    [Header("Camera Transition")]
    [SerializeField] private Image transitionImage;
    [SerializeField] private float closeDuration = 0.2f;
    [SerializeField] private float openDuration = 0.2f;

    [Header("Transition")]
    [SerializeField] private float blackHoldTime = 0.05f;

    private CameraProfile activeCamera;
    private CameraState currentState = CameraState.Free;

    private Coroutine transitionCoroutine;

    public CameraState CurrentState => currentState;
    public bool CanMove => currentState == CameraState.Free;
    public CameraProfile ActiveCamera => activeCamera;

    private void Awake()
    {
        if (cameras == null || cameras.Length == 0)
        {
            Debug.LogError(
                "ShopCameraController: No camera profiles assigned."
            );

            return;
        }

        if (cinemachineBrain == null)
        {
            cinemachineBrain = FindFirstObjectByType<CinemachineBrain>();
        }

        SetupCinemachine();

        InitializeCameras();

        startingCameraIndex = Mathf.Clamp(
            startingCameraIndex,
            0,
            cameras.Length - 1
        );

        SetActiveCamera(startingCameraIndex, true);

        if (transitionImage != null)
        {
            Color color = transitionImage.color;
            color.a = 0f;
            transitionImage.color = color;
            transitionImage.raycastTarget = true;
        }
    }

    private void Update()
    {
        if (activeCamera == null)
            return;

        if (currentState == CameraState.Free)
        {
            HandleMovement();
        }

        MoveActiveCamera();
    }

    // ==================================================
    // CINEMACHINE
    // ==================================================

    private void SetupCinemachine()
    {
        if (cinemachineBrain == null)
        {
            Debug.LogWarning(
                "ShopCameraController: Cinemachine Brain not found."
            );

            return;
        }

        // Camera changes should happen instantly.
        cinemachineBrain.DefaultBlend =
            new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Styles.Cut,
                0f
            );
    }

    // ==================================================
    // INITIALIZATION
    // ==================================================

    private void InitializeCameras()
    {
        foreach (CameraProfile camera in cameras)
        {
            if (camera == null)
                continue;

            if (camera.virtualCamera == null)
            {
                Debug.LogWarning(
                    "ShopCameraController: Camera profile has no Cinemachine Camera assigned."
                );

                continue;
            }

            if (camera.target == null)
            {
                Debug.LogWarning(
                    $"ShopCameraController: Camera '{camera.cameraName}' has no target assigned."
                );

                continue;
            }

            camera.targetX = Mathf.Clamp(
                camera.target.position.x,
                camera.minX,
                camera.maxX
            );

            camera.target.position = new Vector3(
                camera.targetX,
                camera.target.position.y,
                camera.target.position.z
            );

            camera.virtualCamera.Priority = 0;
        }
    }

    // ==================================================
    // MOVEMENT
    // ==================================================

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) < 0.01f)
            return;

        activeCamera.targetX +=
            input *
            activeCamera.moveSpeed *
            Time.deltaTime;

        activeCamera.targetX = Mathf.Clamp(
            activeCamera.targetX,
            activeCamera.minX,
            activeCamera.maxX
        );
    }

    private void MoveActiveCamera()
    {
        if (activeCamera.target == null)
            return;

        float newX = Mathf.SmoothDamp(
            activeCamera.target.position.x,
            activeCamera.targetX,
            ref activeCamera.velocityX,
            activeCamera.smoothTime
        );

        activeCamera.target.position = new Vector3(
            newX,
            activeCamera.target.position.y,
            activeCamera.target.position.z
        );
    }

    // ==================================================
    // CAMERA SWITCHING
    // ==================================================

    public void SetActiveCamera(int cameraIndex, bool instant = false)
    {
        if (cameraIndex < 0 || cameraIndex >= cameras.Length)
        {
            Debug.LogWarning(
                $"ShopCameraController: Invalid camera index {cameraIndex}."
            );

            return;
        }

        CameraProfile newCamera = cameras[cameraIndex];

        if (newCamera == null ||
            newCamera.virtualCamera == null ||
            newCamera.target == null)
        {
            return;
        }

        foreach (CameraProfile camera in cameras)
        {
            if (camera == null || camera.virtualCamera == null)
                continue;

            camera.virtualCamera.Priority = 0;
        }

        newCamera.virtualCamera.Priority = 10;

        activeCamera = newCamera;

        activeCamera.targetX = Mathf.Clamp(
            activeCamera.target.position.x,
            activeCamera.minX,
            activeCamera.maxX
        );

        activeCamera.velocityX = 0f;

        if (instant)
        {
            activeCamera.target.position = new Vector3(
                activeCamera.targetX,
                activeCamera.target.position.y,
                activeCamera.target.position.z
            );
        }

        currentState = CameraState.Free;
    }

    public void SetActiveCamera(
        string cameraName,
        bool instant = false
    )
    {
        if (string.IsNullOrEmpty(cameraName))
            return;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == null)
                continue;

            if (cameras[i].cameraName == cameraName)
            {
                SetActiveCamera(i, instant);
                return;
            }
        }

        Debug.LogWarning(
            $"ShopCameraController: Camera '{cameraName}' not found."
        );
    }

    // ==================================================
    // CAMERA TRANSITION
    // ==================================================

    public void SwitchCamera(string cameraName)
    {
        if (currentState != CameraState.Free)
            return;

        if (string.IsNullOrEmpty(cameraName))
            return;

        int cameraIndex = -1;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null &&
                cameras[i].cameraName == cameraName)
            {
                cameraIndex = i;
                break;
            }
        }

        if (cameraIndex == -1)
        {
            Debug.LogWarning(
                $"ShopCameraController: Camera '{cameraName}' not found."
            );

            return;
        }

        if (cameras[cameraIndex] == activeCamera)
            return;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(
            CameraTransition(cameraIndex)
        );
    }

    private IEnumerator CameraTransition(int cameraIndex)
    {
        currentState = CameraState.Locked;

        // Stop current camera movement.
        StopMovement();

        // Close screen.
        yield return FadeScreen(0f, 1f, closeDuration);

        // Make sure screen is completely black.
        yield return new WaitForSecondsRealtime(blackHoldTime);

        // Switch camera instantly while screen is black.
        SetActiveCamera(cameraIndex, true);

        // Open screen.
        yield return FadeScreen(1f, 0f, openDuration);

        currentState = CameraState.Free;

        transitionCoroutine = null;
    }

    private IEnumerator FadeScreen(
        float startAlpha,
        float targetAlpha,
        float duration
    )
    {
        if (transitionImage == null)
            yield break;

        float elapsed = 0f;

        Color color = transitionImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth transition.
            t = t * t * (3f - 2f * t);

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            transitionImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        transitionImage.color = color;
    }

    // ==================================================
    // CAMERA MOVEMENT
    // ==================================================

    public void MoveToX(float x)
    {
        if (activeCamera == null)
            return;

        activeCamera.targetX = Mathf.Clamp(
            x,
            activeCamera.minX,
            activeCamera.maxX
        );

        activeCamera.velocityX = 0f;

        currentState = CameraState.Moving;
    }

    public void MoveToPoint(Transform point)
    {
        if (point == null)
            return;

        MoveToX(point.position.x);
    }

    // ==================================================
    // STATE
    // ==================================================

    public void LockCamera()
    {
        currentState = CameraState.Locked;

        StopMovement();
    }

    public void UnlockCamera()
    {
        currentState = CameraState.Free;
    }

    public void StopMovement()
    {
        if (activeCamera == null ||
            activeCamera.target == null)
        {
            return;
        }

        activeCamera.targetX =
            activeCamera.target.position.x;

        activeCamera.velocityX = 0f;
    }

    // ==================================================
    // TARGET CHECK
    // ==================================================

    public bool HasReachedTarget()
    {
        if (activeCamera == null ||
            activeCamera.target == null)
        {
            return true;
        }

        return Mathf.Abs(
            activeCamera.target.position.x -
            activeCamera.targetX
        ) < 0.01f;
    }

    // ==================================================
    // PROPERTIES
    // ==================================================

    public float CurrentX
    {
        get
        {
            if (activeCamera == null ||
                activeCamera.target == null)
            {
                return 0f;
            }

            return activeCamera.target.position.x;
        }
    }

    public float MinX
    {
        get
        {
            if (activeCamera == null)
                return 0f;

            return activeCamera.minX;
        }
    }

    public float MaxX
    {
        get
        {
            if (activeCamera == null)
                return 0f;

            return activeCamera.maxX;
        }
    }
}