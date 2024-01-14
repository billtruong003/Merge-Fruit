using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FruitCasual
{
    public class Level : MonoBehaviour
    {
        [SerializeField] public int LevelObject;
        [SerializeField] private GameObject upgradeObject;
        [SerializeField] public bool hasMerged = true;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.layer != gameObject.layer && !hasMerged)
                return;
            hasMerged = true;
            MainManager.Instance.UpgradeObject(transform.gameObject, col.gameObject, upgradeObject);
            gameObject.SetActive(false);
        }
    }

}
