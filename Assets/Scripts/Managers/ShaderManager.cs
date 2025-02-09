using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{

    public static ShaderManager instance;
    public Shader damageShader;
    public Shader normalShader;
    public Material whiteMountain, grayMountain, rockMountain, sandWater;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}