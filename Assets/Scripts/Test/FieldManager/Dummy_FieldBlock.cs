#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_FieldBlock : IFieldBlock
{
    private List<IUnitActor> unitActors = new List<IUnitActor>();

    private Vector2 _position;

    public Vector2Int coordinate { get; private set; }

    public IUnitActor unitActor => GetUnitActor(TYPE_UNIT_FORMATION.Ground);

    public bool isMovement => false;

    public bool isRange => false;

    public bool isFormation => false;

    public Vector2 position => _position;

    /// <summary>
    /// UnitActor를 설정합니다
    /// </summary>
    /// <param name="unitActor"></param>
    /// <param name="isPosition"></param>
    public void SetUnitActor(IUnitActor unitActor, bool isPosition = true)
    {
        unitActors.Add(unitActor);
    }
    /// <summary>
    /// 좌표를 정합ㅎ니다
    /// </summary>
    /// <param name="coor"></param>
    public void SetCoordinate(Vector2Int coor)
    {
        coordinate = coor;
    }
    /// <summary>
    /// UnitActor를 모두 비웁니다
    /// </summary>
    public void LeaveUnitActor()
    {
        unitActors.Clear();
    }

    /// <summary>
    /// UnitActor가 비어있는지 확인합니다
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => unitActors.Count == 0;

    /// <summary>
    /// 등록된 모든 UnitActor를 가져옵니다
    /// 없으면 리스트가 0으로 비어있습니다
    /// </summary>
    /// <returns></returns>
    public IUnitActor[] GetUnitActors() => unitActors.ToArray();

    /// <summary>
    /// 해당 위치의 유닛을 가져옵니다
    /// 없으면 null을 반환합니다
    /// </summary>
    /// <param name="typeUnitFormation"></param>
    /// <returns></returns>
    public IUnitActor GetUnitActor(TYPE_UNIT_FORMATION typeUnitFormation = TYPE_UNIT_FORMATION.Ground)
    {
        for (int i = 0; i < unitActors.Count; i++)
        {
            if (unitActors[i].typeUnit == typeUnitFormation)
                return unitActors[i];
        }
        return null;
    }


    public void SetRangeColor(bool isActive)
    {
    }

    public void SetMovementColor(bool isActive)
    {
    }

    public void SetFormationColor(bool isActive)
    {
    }

    public void LeaveUnitActor(IUnitActor uActor)
    {
    }
}

#endif