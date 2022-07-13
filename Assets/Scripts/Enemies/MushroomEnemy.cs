using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomEnemy : EnemyBase
{
    #region Variables

    [SerializeField] protected Animator animator;

    #endregion

    #region Overrides

    public override void MoveToTarget()
    {
        Vector3 movementDirection = transform.position - target;
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);

        if (animator.GetBool("InAir") && (transform.rotation.eulerAngles - targetRotation.eulerAngles).magnitude < rotationBeforeMovementThreshold)
        {
            transform.position -= movementDirection.normalized * movementSpeed * (59 + GameBase.instance.spawnedEnemyNumber) / 600;//Vector3.Lerp(transform.position, target, movementSpeed / 500);
        }
    }

    public override IEnumerator HandleFreeze(float time)
    {
        bFreezed = true;
        animator.speed = 0;
        List<Material> mat = new List<Material>();
        rendererComp.GetMaterials(mat);
        foreach (var m in mat)
        {
            m.color = Color.cyan;
        }
        yield return new WaitForSeconds(time);

        for (int i = 0; i < mat.Count; i++)
        {
            mat[i].color = startColors[i];
        }
        bFreezed = false;
        animator.speed = 1;
    }

    #endregion

    #region AnimationCallbacks

    public void OnJumpStarted()
    {
        animator.SetBool("InAir", true);
    }

    public void OnJumpEnded()
    {
        animator.SetBool("InAir", false);
    }

    #endregion
}
