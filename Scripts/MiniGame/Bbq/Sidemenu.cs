using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TableState ����ü ����
public struct TableState
{
    public bool[] ingre;
    public bool isCook; //��������, �ҹ������ �Ѵ� ������ ��ħ

    public TableState(bool[] ingre, bool isCook)
    {
        this.ingre = ingre;
        this.isCook = isCook;
    }
}

