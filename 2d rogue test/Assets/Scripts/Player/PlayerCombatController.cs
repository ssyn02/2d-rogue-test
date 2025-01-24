using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    private bool inputReceived;
    public bool isAttacking;
    private int attackCounter = 0;

    [SerializeField] private float inputBufferTime, attack1Radius, attack1Damage;
    private float lastInputTime = Mathf.NegativeInfinity;
    private float[] attackDetails = new float[2];

    [SerializeField] private Transform attack1HitboxPosition;
    [SerializeField] private LayerMask damageable;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {   
            if (combatEnabled)
            {
                inputReceived = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()
    {
        if (inputReceived)
        {
            if (!isAttacking)
            {
                inputReceived = false;
                isAttacking = true;
                anim.SetBool("attack1", true);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time > lastInputTime + inputBufferTime)
        {
            inputReceived = false;
        }
    }

    private void CheckAttackHitbox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitboxPosition.position, attack1Radius, damageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            Debug.Log("asd");
        }

    }

    private void FinishAttack1()
    {
        isAttacking = false;
        attackCounter++;

        if (attackCounter == 3)
        {
            attackCounter = 0;
        }

        anim.SetInteger("attackCounter", attackCounter);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPosition.position, attack1Radius);
    }
}
