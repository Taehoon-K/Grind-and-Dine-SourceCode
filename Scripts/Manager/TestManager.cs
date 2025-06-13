using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] ItemData[] smoke;
    public void PlusMoney()
    {
        PlayerStats.Earn(100000);
    }
    public void MinusMoney()
    {
        PlayerStats.Spend(100000);
    }
    public void CharismaLevelUp()
    {
        StatusManager.instance.GetStatus().level[4]++;
    }
    public void HandyLevelUp()
    {
        StatusManager.instance.GetStatus().level[2]++;
    }
    public void StrLevelUp()
    {
        StatusManager.instance.GetStatus().level[0]++;
    }
    public void GiveItemSmoke(int array) //¥„πË ª˝º∫

    {
        if (FindObjectOfType<Inventory>().Add(smoke[array], 1) == 0)
        {

        }
    }
    #region Stat
    public void Fatigue(bool plus) //Ω∫≈» ¡ı∞®
    {
        if (plus)
        {
            StatusManager.instance.GetStatus().currentFatigue += 100;
        }
        else
        {
            StatusManager.instance.GetStatus().currentFatigue -= 100;
        } 
    }
    public void Hungry(bool plus) //Ω∫≈» ¡ı∞®
    {
        if (plus)
        {
            StatusManager.instance.GetStatus().currentHungry += 100;
        }
        else
        {
            StatusManager.instance.GetStatus().currentHungry -= 100;
        }
    }
    public void Anger(bool plus) //Ω∫≈» ¡ı∞®
    {
        if (plus)
        {
            StatusManager.instance.GetStatus().currentAngry += 100;
        }
        else
        {
            StatusManager.instance.GetStatus().currentAngry -= 100;
        }
    }
    public void Sandess(bool plus) //Ω∫≈» ¡ı∞®
    {
        if (plus)
        {
            StatusManager.instance.GetStatus().currentSadness += 100;
        }
        else
        {
            StatusManager.instance.GetStatus().currentSadness -= 100;
        }
    }
    public void Boredom(bool plus) //Ω∫≈» ¡ı∞®
    {
        if (plus)
        {
            StatusManager.instance.GetStatus().currentBoredom += 100;
        }
        else
        {
            StatusManager.instance.GetStatus().currentBoredom -= 100;
        }
    }

    #endregion
}
