using UnityEngine;

public class LadderStep : MonoBehaviour
{
    public int Id { get; private set; }

    public void Init(int id)
    {
        Id = id;
    }

    public virtual void OnTaked()
    {
        
    }

    public virtual void OnRelease()
    {

    }
}
