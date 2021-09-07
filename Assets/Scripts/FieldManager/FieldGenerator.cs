using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField]
    private FieldBlock _block;

    [SerializeField]
    private Vector2Int _fieldSize;

    [SerializeField]
    private float _length;


    public void Initialize() {
        CreateBlocks();
//        FieldManager.current.Initialize(_block, _fieldSize, _length);
    }

    private void CreateBlocks()
    {
        var fieldBlocks = new FieldBlock[_fieldSize.y][];
        //var blockList = new List<FieldBlock>();

        var startX = -((float)_fieldSize.x) * _length * 0.5f + _length * 0.5f;
        var startY = -((float)_fieldSize.y) * _length * 0.5f + _length * 0.5f;

        for (int y = 0; y < _fieldSize.y; y++)
        {
            fieldBlocks[y] = new FieldBlock[_fieldSize.x];

            for (int x = 0; x < _fieldSize.x; x++)
            {
                var block = Instantiate(_block);
                block.transform.SetParent(transform);
                block.SetCoordinate(new Vector2Int(x, y));
                block.transform.localPosition = new Vector3(startX + ((float)x) * _length, startY + ((float)y) * _length, 0f);
                block.gameObject.SetActive(true);

                fieldBlocks[y][x] = block;
            }
        }

        FieldManager.Initialize(fieldBlocks, _fieldSize);
    }

    public void CleanUp()
    {
        FieldManager.CleanUp();
    }

}
