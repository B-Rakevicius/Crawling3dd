using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerStyleManager : MonoBehaviour
{
    public Camera heroCamera;
    public Camera[] ghostCameras;
    public Shader heroShader;
    public Shader ghostShader;
    public PostProcessProfile heroProfile;
    public PostProcessProfile ghostProfile;

    void Start()
    {
        heroCamera.GetComponent<PostProcessVolume>().profile = heroProfile;
        foreach (var renderer in FindObjectsOfType<Renderer>())
        {
            if (renderer.CompareTag("Hero"))
            {
                renderer.material.shader = heroShader;
            }
        }
        foreach (var ghostCamera in ghostCameras)
        {
            ghostCamera.GetComponent<PostProcessVolume>().profile = ghostProfile;
            foreach (var renderer in FindObjectsOfType<Renderer>())
            {
                if (renderer.CompareTag("Ghost"))
                {
                    renderer.material.shader = ghostShader;
                }
            }
        }
    }
}