using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrerardHolder : MonoBehaviour
{
    public int _number;
    public string _waitItem; 
    public Frerard _frerard;
    AllFather _allFather;
    Canvas _canvas;
    Inventory _inventory;
    IsItem _placedItem;
    int _currentRotation;
    int _rotations;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _canvas = GameObject.FindObjectOfType<Canvas>();
        _inventory = _canvas.GetComponent<Inventory>();
    }

    public void Take(IsItem newItem)
    {
        if (!_frerard._activated)
        {
            if (newItem != null)
            {
                newItem.obj.transform.position = transform.position;
                newItem.obj.transform.rotation = transform.rotation;
                newItem.obj.transform.localScale = transform.localScale;
                newItem._renderer.enabled = true;

                int rotations = UnityEngine.Random.Range(0, 4);
                _currentRotation = 0;

                for (int i = 0; i < rotations; i++)
                    Rotate(newItem);
                _rotations = 0;

                if (_placedItem != null)
                    _inventory.Take(_placedItem);
                _placedItem = newItem;
                _placedItem.transform.SetParent(_frerard.transform);

                _allFather._audioManager.Play("kill", 0.7f);
            }
            else if (_placedItem != null)
            {
                if (_rotations >= 3)
                {
                    _inventory.Take(_placedItem);
                    _placedItem = null;
                }
                else
                    Rotate(_placedItem);

                _allFather._audioManager.Play("kill", 0.7f);
            }

            if (_placedItem != null)
            {
                //Debug.Log($"{_placedItem.name} {_waitItem} {_currentRotation}");
                bool ok = _placedItem.name == _waitItem && _currentRotation == 0;
                _frerard.Set(_number, ok);
            }
        }
	}

    void Rotate(IsItem item)
    {
        item.transform.Rotate(0, 0, 90);
        _rotations++;
        _currentRotation++;

        if (_currentRotation == 4)
            _currentRotation = 0;
    }
}
