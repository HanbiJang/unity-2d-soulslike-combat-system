using UnityEngine;

// BoxCollider2D(Is Trigger ON) 달린 오브젝트에 붙임
// 플레이어가 진입하면 locationNameUI.Show() 호출
public class LocationNameTrigger : MonoBehaviour
{
    [SerializeField] private LocationNameUI locationNameUI;
    [SerializeField] private string locationName = "숲";
    private bool _triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (other.GetComponent<PlayerController>() == null) return;

        _triggered = true;
        locationNameUI.Show(locationName);
    }
}
