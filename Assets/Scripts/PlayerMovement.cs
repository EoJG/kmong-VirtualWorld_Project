using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public List<GameObject> foods;

    public GameObject grid;

    Transform playerCamera;

    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

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

    public bool isThirdPerson = false;
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
        ChangeView();
        MoveBezier();

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grid.activeSelf)
                grid.SetActive(false);
            else
                grid.SetActive(true);
        }

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
            Ray ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);
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

    void ChangeView()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isThirdPerson = !isThirdPerson;

            firstPersonCamera.enabled = !isThirdPerson;
            thirdPersonCamera.enabled = isThirdPerson;
        }
    }

    public GameObject pointPrefab;

    List<GameObject> instPoints = new List<GameObject>();
    Coroutine bezierCoroutine;

    void MoveBezier()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (instPoints.Count > 0)
            {
                foreach (GameObject obj in instPoints)
                {
                    Destroy(obj);
                }
                instPoints.Clear();
            }

            NearestGrid nearestGrid = GetComponent<NearestGrid>();

            instPoints.Add(Instantiate(pointPrefab, grid.transform.GetChild(nearestGrid.nearGridIndex).position, Quaternion.identity));

            List<int> gridIndexList = new List<int>(nearestGrid.canMoveGird);
            for (int i = 0; i < gridIndexList.Count; i++)
            {
                if (gridIndexList[i] == nearestGrid.nearGridIndex)
                {
                    gridIndexList.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int randIndex = Random.Range(0, gridIndexList.Count);

                instPoints.Add(Instantiate(pointPrefab, grid.transform.GetChild(gridIndexList[randIndex]).position, Quaternion.identity));

                gridIndexList.RemoveAt(randIndex);
            }

            DrawBezierCurve(instPoints[0].transform.position, instPoints[1].transform.position, instPoints[2].transform.position, instPoints[3].transform.position);

            GetComponent<CharacterController>().enabled = false;

            if (bezierCoroutine != null)
                StopCoroutine(bezierCoroutine);
            bezierCoroutine = StartCoroutine(MoveAlongBezier(instPoints[0].transform.position, instPoints[1].transform.position, instPoints[2].transform.position, instPoints[3].transform.position));
        }
    }

    Vector3 BezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    public LineRenderer lr;

    void DrawBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        if (!lr.enabled)
            lr.enabled = true;

        int resolution = 20;
        lr.positionCount = resolution + 1;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            lr.SetPosition(i, BezierPoint(t, p0, p1, p2, p3));
        }
    }

    public float moveDuration = 3f;

    IEnumerator MoveAlongBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            Vector3 pointOnCurve = BezierPoint(t, p0, p1, p2, p3);

            transform.position = pointOnCurve;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치 보정
        transform.position = p3;

        GetComponent<CharacterController>().enabled = true;
    }
}
