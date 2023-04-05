using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float gravity = 20.0f;
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0, -gravity, 0));
        }
    }
}
