using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {
    private Animator animator;

    private float forwardBackward = 0f;
    private float leftRight = 0f;
    private float upDown = 0f;

    void Start() => animator = GetComponent<Animator>();

    void Update() {
        float moveForwardBackward = Input.GetAxis("Vertical");
        float moveLeftRight = Input.GetAxis("Horizontal");
        float moveUpDown = 0f;

        if (Input.GetKey(KeyCode.E)) moveUpDown = 1f;
        else if (Input.GetKey(KeyCode.Q)) moveUpDown = -1f;

        forwardBackward = Mathf.Lerp(forwardBackward, moveForwardBackward, Time.deltaTime * 5f);
        leftRight = Mathf.Lerp(leftRight, moveLeftRight, Time.deltaTime * 5f);
        upDown = Mathf.Lerp(upDown, moveUpDown, Time.deltaTime * 5f);

        animator.SetFloat("ForwardBackward", forwardBackward);
        animator.SetFloat("LeftRight", leftRight);
        animator.SetFloat("UpDown", upDown);
    }
}
