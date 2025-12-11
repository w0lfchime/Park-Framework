using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AppCameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Static,
        FreeCam,
        LerpToTarget
    }

    [Header("Mode Settings")]
    public CameraMode currentMode = CameraMode.Static;


    //-------------------------------------
    [Header("FreeCam Settings")]
    public float moveSpeed = 15f;
    public float fastMoveMultiplier = 5f;
    public float lookSensitivity = 5f;

    private float yaw;
    private float pitch;

    // Store original transform for restoring after FreeCam
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    //------------------------------------
    [Header("LerpToTarget Settings")]
    // needed settings
    public Transform lerpTarget;
    public float positionLerpSpeed = 5f;
    public float rotationLerpSpeed = 5f;
    // needed private variables, if any
    // (none required for now)


    void Start()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;

        SetMode(CameraMode.Static);

        CommandHandler.RegisterCommand("freecam", args =>
        {
            if (currentMode != CameraMode.FreeCam)
            {
                SetMode(CameraMode.FreeCam);
            }
            else
            {
                //HACK: need to return to previous mode
                SetMode(CameraMode.Static);
            }
        });
    }

    void Update()
    {
        switch (currentMode)
        {
            case CameraMode.Static:
                // Do nothing
                break;

            case CameraMode.FreeCam:
                UpdateFreeCam();
                break;

            case CameraMode.LerpToTarget:
                UpdateLerpToTarget();
                break;
        }
    }

    private void UpdateFreeCam()
    {
        // --- Mouse Look (hold right-click) ---
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // --- Movement ---
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.E)) move += transform.up;
        if (Input.GetKey(KeyCode.Q)) move -= transform.up;

        transform.position += move * speed * Time.deltaTime;
    }

    private void UpdateLerpToTarget()
    {
        if (lerpTarget == null)
            return;

        // Lerp world position and rotation towards target
        transform.position = Vector3.Lerp(
            transform.position,
            lerpTarget.position,
            positionLerpSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lerpTarget.rotation,
            rotationLerpSpeed * Time.deltaTime
        );
    }

    public void SetMode(CameraMode newMode)
    {
        if (newMode == currentMode) return;

        // --- Handle exiting current mode ---
        switch (currentMode)
        {
            case CameraMode.FreeCam:
                // restore original transform when leaving FreeCam
                transform.position = savedPosition;
                transform.rotation = savedRotation;
                break;

                // For Static and LerpToTarget, nothing special on exit for now
        }

        // --- Handle entering new mode ---
        switch (newMode)
        {
            case CameraMode.FreeCam:
                // save current transform for later restore
                savedPosition = transform.position;
                savedRotation = transform.rotation;

                // initialize yaw/pitch to match current rotation
                Vector3 euler = transform.rotation.eulerAngles;
                yaw = euler.y;
                pitch = euler.x;
                break;

            case CameraMode.LerpToTarget:
                // No special setup needed here by default.
                // lerpTarget is expected to be set externally or via FindAndCopySceneCamera().
                break;
        }

        currentMode = newMode;
    }

    /// <summary>
    /// Finds cameras in all scenes except the one containing this AppCameraController,
    /// and attempts to copy their camera settings and transform into this camera.
    /// 
    /// Behaviour:
    /// - If camera mode is FreeCam: abort entirely.
    /// - If >1 external camera is found: abort.
    /// - If exactly one is found:
    ///     - Always copy its Camera settings to this component.
    ///     - If current mode == Static: also copy world transform immediately.
    ///     - If current mode == LerpToTarget: set lerpTarget to the found camera's transform,
    ///       and do NOT directly change this transform (the Lerp mode will move it).
    /// - In all successful cases, the found camera component is disabled.
    /// </summary>
    public bool FindAndCopySceneCamera()
    {
        // If we're in FreeCam, we do nothing.
        if (currentMode == CameraMode.FreeCam)
        {
            Debug.Log("[AppCameraController] FindAndCopySceneCamera aborted: in FreeCam mode.");
            return false;
        }

        Camera thisCamera = GetComponent<Camera>();
        if (thisCamera == null)
        {
            Debug.LogError("[AppCameraController] No Camera component found on this GameObject.");
            return false;
        }

        Scene myScene = gameObject.scene;
        Camera foundCamera = null;

        // Search all scenes other than our own
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            if (scene == myScene) continue;

            var roots = scene.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++)
            {
                // Include inactive objects too, just in case
                Camera[] cams = roots[r].GetComponentsInChildren<Camera>(true);
                for (int c = 0; c < cams.Length; c++)
                {
                    Camera cam = cams[c];

                    // Skip our own camera if somehow found
                    if (cam == thisCamera)
                        continue;

                    if (foundCamera == null)
                    {
                        foundCamera = cam;
                    }
                    else
                    {
                        // More than one camera found -> abort
                        Debug.LogWarning("[AppCameraController] FindAndCopySceneCamera aborted: more than one external Camera found.");
                        return false;
                    }
                }
            }
        }

        if (foundCamera == null)
        {
            Debug.Log("[AppCameraController] FindAndCopySceneCamera: no external Camera found.");
            return false;
        }

        // Copy camera settings
        CopyCameraSettings(foundCamera, thisCamera);

        // Mode-specific behaviour
        if (currentMode == CameraMode.Static)
        {
            // Directly copy world transform
            transform.position = foundCamera.transform.position;
            transform.rotation = foundCamera.transform.rotation;
        }
        else if (currentMode == CameraMode.LerpToTarget)
        {
            // Use the found camera's transform as the lerp target
            lerpTarget = foundCamera.transform;
        }

        // Disable the found camera (we only need its transform if used as target)
        foundCamera.enabled = false;

        Debug.Log("[AppCameraController] FindAndCopySceneCamera: copied settings from camera '" +
                  foundCamera.name + "' in scene '" + foundCamera.gameObject.scene.name + "'.");

        return true;
    }

    /// <summary>
    /// Copies the main runtime camera settings from source to destination.
    /// Extend this if you want to support more fields.
    /// </summary>
    private void CopyCameraSettings(Camera source, Camera destination)
    {
        if (source == null || destination == null) return;

        destination.clearFlags = source.clearFlags;
        destination.backgroundColor = source.backgroundColor;
        destination.cullingMask = source.cullingMask;
        destination.orthographic = source.orthographic;
        destination.orthographicSize = source.orthographicSize;
        destination.fieldOfView = source.fieldOfView;
        destination.nearClipPlane = source.nearClipPlane;
        destination.farClipPlane = source.farClipPlane;
        destination.depth = source.depth;
        destination.renderingPath = source.renderingPath;
        destination.usePhysicalProperties = source.usePhysicalProperties;
        destination.projectionMatrix = source.projectionMatrix;
        destination.allowHDR = source.allowHDR;
        destination.allowMSAA = source.allowMSAA;
        destination.allowDynamicResolution = source.allowDynamicResolution;
        destination.targetDisplay = source.targetDisplay;
        // Add more fields here if your project relies on them.
    }
}
