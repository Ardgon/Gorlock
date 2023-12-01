using System.Collections;
using UnityEngine;

public class SlowPuddle : MonoBehaviour
{
    [SerializeField] float slowAmount = 1f;
    [SerializeField] float lifeSeconds = 1.5f;

    private void Start()
    {
        StartCoroutine(KillAfterSeconds());
    }

    private void OnTriggerStay(Collider other)
    {
        var aiController = other.GetComponent<AIController>();
        if (aiController != null)
        {
            if (aiController.speedModifierCoroutine != null)
            {
                StopCoroutine(aiController.speedModifierCoroutine);
                aiController.speedModifierCoroutine = null;
            }
            aiController.speedModifierCoroutine = StartCoroutine(ApplySlowModifier(aiController));
        }
    }

    private IEnumerator ApplySlowModifier(AIController aiController)
    {
        aiController.speedModifier = -slowAmount;
        yield return new WaitForSeconds(1.5f);
        aiController.speedModifier = 0f;
    }

    private IEnumerator KillAfterSeconds()
    {
        yield return new WaitForSeconds(lifeSeconds);
        Destroy(gameObject);
    }
}
