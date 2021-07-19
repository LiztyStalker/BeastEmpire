using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInformation : MonoBehaviour
{
    [SerializeField]
    UIUnitFormation uiFormation;

    [SerializeField]
    Image icon;

    [SerializeField]
    Text nameText;

    [SerializeField]
    Text groupText;

    [SerializeField]
    Text classText;

    [SerializeField]
    Text positionText;

    [SerializeField]
    Text squadText;

    [SerializeField]
    Slider expSlider;

    [SerializeField]
    Text expText;

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Text healthText;

    [SerializeField]
    Text attackTypeText;

    [SerializeField]
    Text attackValueText;

    [SerializeField]
    Text attackCountText;

    [SerializeField]
    Text attackRangeTypeText;

    [SerializeField]
    Text attackRangeText;

    [SerializeField]
    Text attackMinRangeText;

    [SerializeField]
    Text attackBaseText;

    [SerializeField]
    Text priorityText;

    [SerializeField]
    Text movementText;

    [SerializeField]
    Text movementTypeText;

    [SerializeField]
    Text employCostText;

    [SerializeField]
    Text maintenanceCostText;
        

    public void ShowData(UnitCard _uCard)
    {
        uiFormation.ShowFormation(_uCard);
        icon.sprite = _uCard.icon;
        nameText.text = _uCard.name;
        groupText.text = _uCard.typeUnitGroup.ToString();
        classText.text = _uCard.typeUnitClass.ToString();
        positionText.text = _uCard.typeUnit.ToString();
        healthSlider.value = _uCard.TotalHealthRate();
        healthText.text = string.Format("{0}/{1}", _uCard.totalNowHealthValue, _uCard.totalMaxHealthValue);
        attackTypeText.text = _uCard.typeUnitAttack.ToString();
        attackValueText.text = _uCard.damageValue.ToString();
        attackCountText.text = _uCard.attackCount.ToString();
        attackRangeTypeText.text = _uCard.typeUnitAttackRange.ToString();
        attackRangeText.text = _uCard.attackRangeValue.ToString();
        attackMinRangeText.text = _uCard.attackMinRangeValue.ToString();
        attackBaseText.text = (_uCard.bullet == null) ? "근거리" : "원거리";
        priorityText.text = _uCard.priorityValue.ToString();
        movementText.text = _uCard.movementValue.ToString();
        movementTypeText.text = _uCard.typeMovement.ToString();
        employCostText.text = _uCard.employCostValue.ToString();
        maintenanceCostText.text = _uCard.maintenenceCostValue.ToString();
        gameObject.SetActive(true);
    }
}
