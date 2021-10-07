#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
using NUnit.Framework;
using System.Linq;
using UnityEngine;
public class StatusDataEditTester
{

    private ICaster _caster;
    private UnitCard _uCard;
    private UnitData _uData;
    private UnitActor _unitActor;
    private StatusData _statusData;

    [SetUp]
    public void SetUp()
    {
        _uData = new UnitData();
        _statusData = new StatusData();
        _unitActor = new UnitActor();
        _uCard = UnitCard.Create(_uData);
        _unitActor.SetData(_uCard);
        _caster = new UnitActor();


    }

    [TearDown]
    public void TearDown()
    {
        _uData = null;
        _statusData = null;
        _unitActor = null;
        _caster = null;
        _uCard = null;
    }


    [Test]
    public void StatusData_LifeSpan_Caster() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _unitActor.SetStatusData(_caster, _statusData);
        Assert.IsTrue(_unitActor.IsHasStatusData(StatusData.TYPE_STATUS_LIFE_SPAN.Caster));
    }

    [Test]
    public void StatusData_LifeSpan_Caster_Dead() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.RemoveStatusData(_caster);
        Assert.IsFalse(_unitActor.IsHasStatusData(_statusData));
    }

    [Test]
    public void StatusData_LifeSpan_Turn_Start()
    {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
        _statusData.SetTurnCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.TurnCount);
        Assert.IsTrue(statusElement.TurnCount == 2);
    }

    [Test]
    public void StatusData_LifeSpan_Turn_Next() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
        _statusData.SetTurnCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.Turn();
        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.TurnCount);
        Assert.IsTrue(statusElement.TurnCount == 1);
    }

    [Test]
    public void StatusData_LifeSpan_Turn_End() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
        _statusData.SetTurnCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.Turn();
        _unitActor.Turn();
        Assert.IsFalse(_unitActor.IsHasStatusData(_statusData));
    }

    [Test]
    public void StatusData_LifeSpan_Always() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Always);
        _unitActor.SetStatusData(_caster, _statusData);
        Assert.IsTrue(_unitActor.IsHasStatusData(StatusData.TYPE_STATUS_LIFE_SPAN.Always));
    }

    [Test]
    public void StatusData_Overlap_Count2_Set1() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _statusData.SetIsOverlap(true);
        _statusData.SetOverlapCount(2);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.OverlapCount);
        Assert.IsTrue(statusElement.OverlapCount == 1);

    }

    [Test]
    public void StatusData_Overlap_Count2_Set2() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _statusData.SetIsOverlap(true);
        _statusData.SetOverlapCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.OverlapCount);
        Assert.IsTrue(statusElement.OverlapCount == 2);


    }
    [Test]
    public void StatusData_Overlap_Count2_Set3() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _statusData.SetIsOverlap(true);
        _statusData.SetOverlapCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.OverlapCount);
        Assert.IsFalse(statusElement.OverlapCount == 3);
    }

    [Test]
    public void StatusData_NotOverlap_Set1() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _statusData.SetIsOverlap(false);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.OverlapCount);
        Assert.IsTrue(statusElement.OverlapCount == 1);
    }
    [Test]
    public void StatusData_NotOverlap_Set2() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Caster);
        _statusData.SetIsOverlap(false);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.OverlapCount);
        Assert.IsFalse(statusElement.OverlapCount == 2);
    }

    [Test]
    public void StatusData_LifeSpan_Turn_Count2_NotOverlap_Set1() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
        _statusData.SetTurnCount(2);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.TurnCount + " " + statusElement.OverlapCount);
        Assert.IsTrue(statusElement.TurnCount == 2 && statusElement.OverlapCount == 1);
    }

    [Test]
    public void StatusData_LifeSpan_Turn_Count2_NotOverlap_Set2() {
        _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
        _statusData.SetTurnCount(2);
        _unitActor.SetStatusData(_caster, _statusData);
        _unitActor.SetStatusData(_caster, _statusData);

        var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
        Debug.Log(statusElement.TurnCount + " " + statusElement.OverlapCount);
        Assert.IsTrue(statusElement.TurnCount == 2 && statusElement.OverlapCount == 1);
    }



    //public void StatusData_LifeSpan_Turn_Count2_NotOverlap_Set2()
    //{
    //    _statusData.SetTypeStatusLifeSpan(StatusData.TYPE_STATUS_LIFE_SPAN.Turn);
    //    _statusData.SetTurnCount(2);
    //    _unitActor.SetStatusData(_caster, _statusData);
    //    _unitActor.SetStatusData(_caster, _statusData);

    //    var statusElement = _unitActor.StatusActor.GetStatusElement(_statusData);
    //    Debug.Log(statusElement.TurnCount + " " + statusElement.OverlapCount);
    //    Assert.IsTrue(statusElement.TurnCount == 2 && statusElement.OverlapCount == 1);
    //}

    [Test]
    public void StatusData_StatusValueAttack_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueAttack>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.damageValue);
        Assert.That(_unitActor.damageValue == 20, Is.True);

    }

    [Test]
    public void StatusData_StatusValueMovement_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueMovement>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.movementValue);
        Assert.That(_unitActor.movementValue == 11, Is.True);

    }

    [Test]
    public void StatusData_StatusValueChargeMovement_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueChargeMovement>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.chargeMovementValue);
        Assert.That(_unitActor.chargeMovementValue == 12, Is.True);
    }

    [Test]
    public void StatusData_StatusValueDefensive_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueDefensive>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.defensiveValue);
        Assert.That(_unitActor.defensiveValue == 10, Is.True);
    }

    [Test]
    public void StatusData_StatusValueProficiency_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueProficiency>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.proficiencyValue);
        Assert.That(_unitActor.proficiencyValue == 40, Is.True);
    }

    [Test]
    public void StatusData_StatusValueCounter_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueCounter>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.counterValue);
        Assert.That(_unitActor.counterValue == 12, Is.True);
    }


    [Test]
    public void StatusData_StatusValueRevCounter_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueRevCounter>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.reverseCounterValue);
        Assert.That(_unitActor.reverseCounterValue == 10.5, Is.True);
    }

    [Test]
    public void StatusData_StatusValueMaxHealth_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueMaxHealth>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.maxHealthValue);
        Assert.That(_unitActor.maxHealthValue == 110, Is.True);
    }

    [Test]
    public void StatusData_StatusValueAttackCount_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValueAttackCount>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.attackCount);
        Assert.That(_unitActor.attackCount == 11, Is.True);
    }

    [Test]
    public void StatusData_StatusValuePriority_Value()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusValuePriority>(StatusValue.TYPE_VALUE.Value, 10));
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.priorityValue);
        Assert.That(_unitActor.priorityValue == 10, Is.True);
    }


    //StatusData_StatusValueAttackRange
    //StatusData_StatusValueAttackStartRange


    //[Test]
    //public void StatusData_StatusValueSkillCastRate_Value()
    //{
    //    _statusData.AddStatusData(StatusDataFactory.Create<StatusValuePriority>(StatusValue.TYPE_VALUE.Value, 10));
    //    _unitActor.SetStatusData(_caster, _statusData);
    //    Debug.Log(_unitActor.priorityValue);
    //    Assert.That(_unitActor.priorityValue == 10, Is.True);
    //}

    //[Test]
    //public void StatusData_StatusValueIncreateNowHealth_Value()
    //{
    //    _statusData.AddStatusData(StatusDataFactory.Create<StatusValuePriority>(StatusValue.TYPE_VALUE.Value, 10));
    //    _unitActor.SetStatusData(_caster, _statusData);
    //    Debug.Log(_unitActor.priorityValue);
    //    Assert.That(_unitActor.priorityValue == 10, Is.True);
    //}

    //StatusData_StatusValueSkillCastRate
    //StatusData_StatusValueIncreateNowHealth


    //StatusData_StatusEffectPenetrate
    //StatusData_StatusEffectParrying
    //StatusData_StatusEffectPanic

    [Test]
    public void StatusData_StatusEffectPenetrate()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusEffectPenetrate>());
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.IsHasStatusEffect<StatusEffectPenetrate>());
        Assert.That(_unitActor.IsHasStatusEffect<StatusEffectPenetrate>(), Is.True);
    }
    [Test]
    public void StatusData_StatusEffectParrying()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusEffectParrying>());
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.IsHasStatusEffect<StatusEffectParrying>());
        Assert.That(_unitActor.IsHasStatusEffect<StatusEffectParrying>(), Is.True);
    }
    [Test]
    public void StatusData_StatusEffectPanic()
    {
        _statusData.AddStatusData(StatusDataFactory.Create<StatusEffectPanic>());
        _unitActor.SetStatusData(_caster, _statusData);
        Debug.Log(_unitActor.IsHasStatusEffect<StatusEffectPanic>());
        Assert.That(_unitActor.IsHasStatusEffect<StatusEffectPanic>(), Is.True);
    }

}
#endif