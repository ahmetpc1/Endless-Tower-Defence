using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower 
{
  
    public void UpgradeTower();
   public IEnumerator fireToEnemy(Transform enemy);

}
