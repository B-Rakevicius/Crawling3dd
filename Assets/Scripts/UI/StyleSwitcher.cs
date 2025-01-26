using UnityEngine;

public class StyleSwitcher : MonoBehaviour
{
    public Shader oneBitShader; // 1-bit shader
    public Shader bigBitShader; // Hand-painted shader
    public Material[] materials; // Materials to switch

    private bool isOneShader = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Toggle style on spacebar press
        {
            isOneShader = !isOneShader;
            SwitchStyle();
        }
    }

    void SwitchStyle()
    {
        Shader targetShader = isOneShader ? oneBitShader : bigBitShader;

        foreach (var material in materials)
        {
            material.shader = targetShader;
        }
    }
}