using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public List<GameObject> foods;

    Transform playerCamera;

    public RawImage minimap;
    public RenderTexture rotatingMinimapRT;
    public RenderTexture fixedMinimapRT;

    CharacterController controller;
    Animator animator;

    public FoodInventory inventory;

    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float cameraPitchLimit = 80f;
    float pitch = 0f;
    float rayDistance = 5f;

    bool fixedMinimap = false;

    void Start()
    {
        playerCamera = transform.GetChild(0);
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Look();
        Feed();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!fixedMinimap)
            {
                minimap.texture = fixedMinimapRT;
                fixedMinimap = true;
            }
            else
            {
                minimap.texture = rotatingMinimapRT;
                fixedMinimap = false;
            }
        }
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * moveX + transform.forward * moveY).normalized;
        controller.Move(dir * speed * Time.deltaTime);

        animator.SetFloat("Speed_f", dir.magnitude);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -cameraPitchLimit, cameraPitchLimit);

        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);  
    }

    void Feed()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Animal animal = hit.collider.GetComponent<Animal>();
                if (animal != null && !hit.collider.GetComponent<Animal>().isEating)
                {
                    if (hit.collider.CompareTag("Dog") && inventory.index == 0)
                    {
                        hit.collider.GetComponent<Animal>().StartFeeding();
                        InstFood(hit.collider.transform.position);
                    }
                    else if (hit.collider.CompareTag("Fox") && inventory.index == 1)
                    {
                        hit.collider.GetComponent<Animal>().StartFeeding();
                        InstFood(hit.collider.transform.position);
                    }
                    else if (hit.collider.CompareTag("Horse") && inventory.index == 2)
                    {
                        hit.collider.GetComponent<Animal>().StartFeeding();
                        InstFood(hit.collider.transform.position);
                    }
                }
            }
        }
    }

    void InstFood(Vector3 hitPos)
    {
        Vector3 spawnPos = transform.position + (Vector3.up * 2.1f);

        GameObject instFood = Instantiate(foods[inventory.index], spawnPos, Quaternion.identity);
        instFood.GetComponent<FlyingFood>().SetDirection(((hitPos + (Vector3.up * 1)) - spawnPos).normalized);
    }
}
