using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public Slider slider;

    Animator animator;
    NavMeshAgent agent;

    float maxHungry = 10f;
    float hungry;
    float curFeedingTime = 2.7f;
    float feedingTime = 2.7f;

    public bool isEating = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(Hunger());
        StartCoroutine(RandomMove());

        hungry = maxHungry;

    }

    private void Update()
    {
        slider.transform.forward = Camera.main.transform.forward;

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

            float randomX = Random.Range(-23f, 23f);
            float randomY = Random.Range(-13f, 33f);
            Vector3 randomPos = new Vector3(randomX, 0, randomY);

            agent.SetDestination(randomPos);
        }
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
}
