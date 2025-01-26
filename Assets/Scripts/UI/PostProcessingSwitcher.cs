using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessingSwitcher : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public PostProcessProfile oneBitProfile; // Low-res, monochromatic
    public PostProcessProfile bigBitProfile; // Vibrant, colorful

    private bool isOneStyle = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Toggle style on spacebar press
        {
            isOneStyle = !isOneStyle;
            SwitchProfile();
        }
    }

    void SwitchProfile()
    {
        postProcessVolume.profile = isOneStyle ? oneBitProfile : bigBitProfile;
    }
}