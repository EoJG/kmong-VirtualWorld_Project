using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum NeedFood
{
    None,
    Bone,
    Apple,
    Carrot
}

public class Animal : MonoBehaviour
{
    GameObject gridObj;

    public NeedFood needFood = NeedFood.None;

    public Slider slider;

    Animator animator;
    NavMeshAgent agent;

    PlayerMovement player;

    float maxHungry = 10f;
    float hungry;
    float curFeedingTime = 2.7f;
    float feedingTime = 2.7f;

    public bool isEating = false;

    Coroutine randomMove;

    public List<Vector2Int> path = new();

    void Start()
    {
        gridObj = GameObject.FindFirstObjectByType<ObjectOnGrid>().gameObject;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindFirstObjectByType<PlayerMovement>();

        //StartCoroutine(Hunger());
        randomMove = StartCoroutine(RandomMove());

        hungry = maxHungry;
    }

    private void Update()
    {
        if (!player.isThirdPerson)
        {
            if (!slider.gameObject.activeSelf)
                slider.gameObject.SetActive(true);

            slider.transform.forward = Camera.main.transform.forward;
        }
        else
        {
            if (slider.gameObject.activeSelf)
                slider.gameObject.SetActive(false);
        }

        slider.value = hungry / maxHungry;

        if (isEating)
        {
            feedingTime -= Time.deltaTime;
            if (feedingTime <= 0)
            {
                StopFeeding();
                feedingTime = curFeedingTime;
            }
        }

        Debug.Log($"{gameObject.name} randomMove ป๓ลย: {(randomMove == null ? "null" : "active")}");
        if (randomMove == null && path.Count == 0)
        {
            Vector3 lookTarget = player.transform.position;
            lookTarget.y = transform.position.y;
            transform.LookAt(lookTarget);
        }

        bool isMoveing = agent.velocity.sqrMagnitude > 0.01f && !agent.isStopped;
        float value = isMoveing ? 0.5f : 0f;

        animator.SetFloat("Speed_f", value);
    }

    IEnumerator Hunger()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            hungry -= 1f;
            if (hungry <= 0f)
                Destroy(gameObject);
        }
    }

    IEnumerator RandomMove()
    {
        while (true)
        {
            int randomTime = Random.Range(3, 11);

            yield return new WaitForSeconds(randomTime);

            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-10f, 30f);
            Vector3 randomPos = new Vector3(randomX, 0, randomY);

            agent.SetDestination(randomPos);
        }
    }

    public void StartRandomMove()
    {
        if (moveAlongPath != null)
            StopCoroutine(moveAlongPath);

        path.Clear();

        if (randomMove != null)
        {
            StopCoroutine(randomMove);
            randomMove = null;
        }
        randomMove = StartCoroutine(RandomMove());
    }

    public void StartFeeding()
    {
        isEating = true;
        agent.isStopped = true;
        animator.SetBool("Eat_b", true);
        hungry += 5f;
    }

    public void StopFeeding()
    {
        isEating = false;
        agent.isStopped = false;
        animator.SetBool("Eat_b", false);
    }

    Coroutine moveAlongPath;

    public void SetPath(List<Vector2Int> newPath)
    {
        if (newPath == null || newPath.Count == 0)
            return;

        if (randomMove != null)
        {
            StopCoroutine(randomMove);
            randomMove = null;
        }

        path = new List<Vector2Int>(newPath);
        path.RemoveAt(path.Count - 1);

        if (moveAlongPath != null)
            StopCoroutine(moveAlongPath);
        moveAlongPath = StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        foreach (Vector2Int grid in path)
        {
            Vector3 targetPos = gridObj.transform.GetChild(grid.x * 11 + grid.y).position;

            agent.SetDestination(targetPos);

            while (Vector3.Distance(transform.position, targetPos) > 0.2f)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        path.Clear();
        moveAlongPath = null;
    }
}
