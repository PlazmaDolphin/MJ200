using System;
using UnityEngine;

public class ColliderEventSender : MonoBehaviour
{
    public Action<Collider2D> OnTriggerEnter;
    public Action<Collider2D> OnTriggerExit;

    public Action<Collision2D> OnCollisionEnter;
    public Action<Collision2D> OnCollisionExit;

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit?.Invoke(collision);
    }
    #endregion

    #region COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit?.Invoke(collision);
    }
    #endregion
}
