using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStorage
{

    List<UnitData> _units = new List<UnitData>();

    UnitData _castleUnit = null;

    public UnitStorage()
    {
        var units = Resources.LoadAll<UnitData>("Units");
        if(units != null)
        {
            _units.AddRange(units);

            _castleUnit = SearchCastleData();
        }
    }

    UnitData SearchCastleData()
    {
        if (_castleUnit != null) return _castleUnit;

        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].typeUnit == TYPE_UNIT.Castle) return _units[i];
        }
        return null;
    }

    public UnitData GetCastleUnit()
    {
        return SearchCastleData();
    }

    public UnitData[] GetRandomUnits(int count)
    {
        List<UnitData> filterUnits = new List<UnitData>();
        while(count > 0)
        {
            var unit = _units[Random.Range(0, _units.Count)];
            if (unit.typeUnit != TYPE_UNIT.Castle)
            {
                if (!filterUnits.Contains(unit))
                {
                    filterUnits.Add(unit);
                    count--;
                }
            }
        }
        return filterUnits.ToArray();
    }

    public UnitCard[] GetRandomUnitCards(int count)
    {
        var units = GetRandomUnits(count);
        List<UnitCard> filterUnits = new List<UnitCard>();
        for(int i = 0; i < units.Length; i++)
        {
            var uCard = new UnitCard(units[i]);
            uCard.SetFormation(GetRandomFormation(4));
            filterUnits.Add(uCard);
        }
        return filterUnits.ToArray();
    }

    public Vector2Int[] GetRandomFormation(int count)
    {
        var formation = CreateFormationArray();
        for (int i = 0; i < count; i++)
        {
            formation = GetRandomFormation(formation);
        }

        return ConvertArrayToListFormation(formation);
    }

    public Vector2Int[] ConvertArrayToListFormation(bool[][] formation)
    {
        List<Vector2Int> formationList = new List<Vector2Int>();
        for (int y = 0; y < formation.Length; y++)
        {
            for(int x = 0; x < formation[y].Length; x++)
            {
                if (formation[y][x])
                    formationList.Add(new Vector2Int(x - 1, y - 1));
            }
        }
        return formationList.ToArray();
    }

    public bool[][] ConvertListToArrayFormation(Vector2Int[] formation)
    {
        bool[][] arr = CreateFormationArray();
        for (int i = 0; i < formation.Length; i++)
        {
            arr[formation[i].y + 1][formation[i].x + 1] = true;
        }
        return arr;
    }

    private bool[][] CreateFormationArray()
    {
        bool[][] formation = new bool[3][];
        for (int i = 0; i < formation.Length; i++)
        {
            formation[i] = new bool[3];
        }
        return formation;
    }

    private bool[][] GetRandomFormation(bool[][] formation, int stackCnt = 100)
    {
        if (stackCnt >= 0)
        {
            stackCnt--;
            var dirX = Random.Range(0, formation.Length);
            var dirY = Random.Range(0, formation.Length);

            if (!formation[dirY][dirX])
                formation[dirY][dirX] = true;
            else
                return GetRandomFormation(formation, stackCnt);
        }
        return formation;

    }
}