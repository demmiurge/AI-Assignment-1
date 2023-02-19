using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    [SerializeField] 
    private Collider2D m_MeCollider;

    [SerializeField]
    private List<Collider2D> m_ListToIgnoreColliders;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_MeCollider) m_MeCollider = GetComponent<Collider2D>();

        foreach (Collider2D Collider in m_ListToIgnoreColliders)
        {
            Physics2D.IgnoreCollision(m_MeCollider, Collider, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
