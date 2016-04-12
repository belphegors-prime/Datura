using UnityEngine;
using System.Collections;

public class TorchTrigger : MonoBehaviour {

    void OnTriggerEnter(Collider c)
    {
        //Debug.Log("collision detected");
        //if player passes through collider, activate torch lights
        if (c.tag.Equals("Player"))
        {
            //Debug.Log("Player has passed through collider");
            Transform door = transform.parent;
            Transform torchA = door.FindChild("TorchA");
            Transform torchB = door.FindChild("TorchB");

            torchA.FindChild("Flame").gameObject.SetActive(true);
            torchB.FindChild("Flame").gameObject.SetActive(true);
        }
        else
        {
           // Debug.Log("non-player entered");
        }
    }
}
