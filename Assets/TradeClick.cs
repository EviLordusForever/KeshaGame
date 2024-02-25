using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TradeClick : MonoBehaviour, IPointerClickHandler
{
    public Trade _trade;
    public int _tradeNumber;
    public Inventory _inventory;
    public IsTrader _trader;
    private AudioManager _audioManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_audioManager == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
            _audioManager = go.GetComponent<AudioManager>();
        }

        if (_inventory.CountOfItem(_trade._selledItemName) >= _trade._selledCount)
        {
            _inventory.Remove(_trade._selledItemName, _trade._selledCount);
            var clone = Instantiate(_trade._item);
            clone.Start(); /////////////////////////////////bug fix
            clone.count = _trade._buyedCount;
            _inventory.Take(clone);
            _trader.RemoveTrade(_tradeNumber);

            _audioManager.Play("money", 1);
        }
        else
            _audioManager.Play("notEnoughCash", 1);
    }
}
