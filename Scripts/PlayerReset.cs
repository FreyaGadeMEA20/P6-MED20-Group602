using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    [SerializeField] Transform resetTransform;  // Position it should reset to
    [SerializeField] GameObject player;         // The player it needs to reset
    [SerializeField] Camera cam;                // The camera

    // Context menu allows us to access the function from the inspector
    [ContextMenu("Reset Position")]
    public void ResetPosition(){
        // The angle it resets
        var rotAngleY = cam.transform.rotation.eulerAngles.y - resetTransform.rotation.eulerAngles.y;

        // applies it to the players transform
        player.transform.Rotate(0, -rotAngleY, 0);

        // Reset the distance between that and the cam, as else it will not properly reset the position
        var dist = resetTransform.position - cam.transform.position;

        // resets the position
        player.transform.position += new Vector3(dist.x, 0f, dist.z);
    }
}
