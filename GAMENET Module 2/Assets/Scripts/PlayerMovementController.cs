using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;
    private RigidbodyFirstPersonController rbFPSController;
    private Animator animator;

    void Start()
    {
        rbFPSController = this.GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rbFPSController.joystickInputAxis.x = joystick.Horizontal;
        rbFPSController.joystickInputAxis.y = joystick.Vertical;
        rbFPSController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

        animator.SetFloat("horizontal", joystick.Horizontal);
        animator.SetFloat("vertical", joystick.Vertical);

        if(Mathf.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9) 
        {
            animator.SetBool("isRunning", true);
            rbFPSController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            animator.SetBool("isRunning", false);
            rbFPSController.movementSettings.ForwardSpeed = 5;
        }

    }
}
