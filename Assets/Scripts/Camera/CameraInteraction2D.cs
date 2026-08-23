using UnityEngine;

public class CameraInteraction2D : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private ShopCameraController cameraController;
    [SerializeField] private string targetCameraName = "Crafting";

    private void OnMouseDown()
    {
        if (cameraController == null)
        {
            Debug.LogWarning(
                $"CameraInteraction2D on '{gameObject.name}': " +
                "ShopCameraController is not assigned."
            );

            return;
        }

        cameraController.SwitchCamera(targetCameraName);
    }
}