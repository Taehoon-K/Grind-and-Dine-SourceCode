using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TableState 구조체 정의
public struct TableState
{
    public bool[] ingre;
    public bool isCook; //가열인지, 뚝배기인지 둘다 어차피 겹침

    public TableState(bool[] ingre, bool isCook)
    {
        this.ingre = ingre;
        this.isCook = isCook;
    }
}

