using UnityEngine;

public class StyleTransition : MonoBehaviour
{
    public float transitionSpeed = 1f;
    private float transitionProgress = 0f;
    private bool isTransitioning = false;
    private bool targetStyleBool = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            targetStyleBool = !targetStyleBool;
            isTransitioning = true;
        }

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime * transitionSpeed;
            transitionProgress = Mathf.Clamp01(transitionProgress);

            // Apply lerped shader values or post-processing settings here <-!!
            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
            }
        }
    }
}