using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower 
{

    
    bool CanShoot { get; set; }

    public void UpgradeTower();
   public IEnumerator fireToEnemy(Transform enemy);

    public void ChangeCanShootBool(bool flag) 
    {
        CanShoot = flag;
    }

}
