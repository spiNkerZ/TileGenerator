using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public  Slider sliderSeamSize, sliderOffset, sliderAngle;
    public Toggle toggleSqaure, toggleGIZMO;

    [SerializeField] GeneratorManager generatorManager;
    [SerializeField] Text countTiles, countCalculateTailes, squareOneTile,squareMainValueText;

    public void GenerateRandomTile()
    {

    }

    public void MakeSeamSize(Slider _slider)
    {
        generatorManager.UpdateTile(_slider.value,0,0);
    }
    public void MakeOffset(Slider _slider)
    {
        generatorManager.UpdateTile(0, _slider.value, 0);
    }
    public void MakeAngle(Slider _slider)
    {
        generatorManager.UpdateTile(0,0,_slider.value);
    }
    public void MakeSliders()
    {
        generatorManager.GenerateTile(sliderSeamSize.value, sliderOffset.value, sliderAngle.value);
    }

    public void SetCountsUIText(float _newValue)
    {
        countTiles.text = _newValue.ToString();
    }
    public void SetCountsCalculateUIText(float _newValue)
    {
        countCalculateTailes.text = _newValue.ToString();
    }
    public void SetSquareOneTileText(float _newValue)
    {
        squareOneTile.text = _newValue.ToString();
    }
    public void SetMainSquare(float _newValue)
    {
        squareMainValueText.text = _newValue.ToString();
    }

    public void SetToggleSqare(Toggle _toggle)
    {
        generatorManager.SetActiveTMPTails(_toggle.isOn);
    }
}

