using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//RequireComponent에 PlayerController 를 지정할 경우 오브젝트에 스크립트를 추가할때 PlayerController를 필수적으로 추가하도록 강요
[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {
    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera;
    GunController gunController;

    protected override void Start () {
        base.Start();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        gunController = GetComponent<GunController>();
	}
	
	void Update () {
        //Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));

        Vector3 moveVelocicy = moveInput.normalized * moveSpeed;

        controller.Move(moveVelocicy);

        //Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray,out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);

            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        //Weapon input
        if (Input.GetMouseButton(0)) {
            gunController.Shoot();
        }
	}
}
