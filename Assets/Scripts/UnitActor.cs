using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public enum TYPE_TEAM { Left, Right}

public class UnitActor : MonoBehaviour
{

    TYPE_TEAM _typeTeam;

    [SerializeField]
    UIBar _uiBar;

    [SerializeField]
    SkeletonAnimation _sAnimation;
    Spine.Skeleton _skeleton;

    [Range(1, 1000)]
    [SerializeField]
    int _healthValue = 100;

    int _nowHealthValue;

    [SerializeField]
    int _damageValue = 35;

    [SerializeField]
    int _movementValue = 1;

    [SerializeField]
    int _costValue = 1;

    [SerializeField]
    int _rangeValue = 1;
        
    public void SetTypeTeam(TYPE_TEAM typeTeam)
    {
        _typeTeam = typeTeam;
        //_renderer.color = GetTeamColor(typeTeam);
        switch (_typeTeam)
        {
            case TYPE_TEAM.Left:
                transform.localScale = Vector3.one;
                break;
            case TYPE_TEAM.Right:
                transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
        }

    }

    public void AddBar(UIBar uiBar)
    {
        _uiBar = uiBar;
        _uiBar.transform.SetParent(transform);
        _uiBar.transform.localPosition = Vector3.up * 1.25f;
        _uiBar.gameObject.SetActive(true);
    }


    private Color GetTeamColor(TYPE_TEAM typeTeam)
    {
        switch (_typeTeam)
        {
            case TYPE_TEAM.Left:
                return Color.blue;
            case TYPE_TEAM.Right:
                return Color.red;
        }
        return Color.black;
    }

    private void SetColor(Color color)
    {
        _skeleton.FindSlot("LBand").SetColor(color);
        _skeleton.FindSlot("RBand").SetColor(color);

    }

    private void Start()
    {
        _sAnimation.GetComponent<MeshRenderer>().sortingOrder = -(int)transform.position.y;
        _skeleton = _sAnimation.skeleton;
        SetColor(GetTeamColor(_typeTeam));

        _nowHealthValue = _healthValue;

        _uiBar.SetBar(HealthRate());

    }

    public void IncreaseHealth(int value)
    {
        if (_nowHealthValue - value < 0)
            _nowHealthValue = 0;
        else
            _nowHealthValue -= value;

        //        _renderer.color = Color.Lerp(Color.white, GetTeamColor(_typeTeam), HealthRate());
        _uiBar.SetBar(HealthRate());

    }

    public float HealthRate() => (float)_nowHealthValue / (float)_healthValue;

    public bool IsDead() => _nowHealthValue == 0;

    public int damageValue => _damageValue;

    public int movementValue => _movementValue;

    public int rangeValue => _rangeValue;

    public TYPE_TEAM typeTeam => _typeTeam;



}
