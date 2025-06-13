using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationEntryPoint : MonoBehaviour
{
    //[SerializeField]
    public SceneTransitionManager.Location locationToSwitch;
    private bool sceneSwitched = false;
    private void OnTriggerEnter(Collider other) //콜라이더 진입 시 호출
    {
        if (other.tag == "Npc")
        {
            Destroy(other.gameObject); //npc 진입시 파괴
        }
    }

    public void SwitchScene() //대화문이나 클릭 등으로 진입 시 사용
    {
        if (!sceneSwitched)
        {
            SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
            sceneSwitched = true; // 씬 전환이 완료되었음을 표시
        }
    }
}
