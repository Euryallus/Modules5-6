using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponHolder))]
public class Player : MonoBehaviour
{
    public CharacterController characterController;
    public float moveSpeed;

    private WeaponHolder weaponHolder;

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GetComponent<WeaponHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float axisHorizontal = Input.GetAxis("Horizontal");
        float axisVertical = Input.GetAxis("Vertical");

        Vector3 moveVector = transform.right * axisHorizontal + transform.forward * axisVertical;

        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }
}
