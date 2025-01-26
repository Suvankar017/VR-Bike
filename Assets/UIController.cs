using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text m_TopSpeedText;
    [SerializeField] private Text m_CurrentMaxSpeedText;
    [SerializeField] private Text m_CurrentSpeedText;
    [SerializeField] private Text m_GearText;

    [SerializeField] private ReVR.Vehicles.Bike.BikeController m_Controller;

    private void Start()
    {
        //UpdateGearUIText(0);
    }

    private void Update()
    {
        UpdateTopSpeedUIText();
        UpdateCurrentMaxSpeedUIText();
        UpdateCurrentSpeedUIText();
    }

    public void UpdateGearUIText(int currentGear)
    {
        if (m_GearText == null)
            return;

        string text = (currentGear == 0) ? "N" : currentGear.ToString();
        m_GearText.text = text;
    }

    private void UpdateTopSpeedUIText()
    {
        if (m_Controller == null || m_TopSpeedText == null)
            return;

        float topSpeed = m_Controller.TopSpeed;
        m_TopSpeedText.text = topSpeed.ToString("F2");
    }

    private void UpdateCurrentMaxSpeedUIText()
    {
        if (m_Controller == null || m_CurrentMaxSpeedText == null)
            return;

        float currentMaxSpeed = m_Controller.CurrentMaxSpeed;
        m_CurrentMaxSpeedText.text = currentMaxSpeed.ToString("F2");
    }

    private void UpdateCurrentSpeedUIText()
    {
        if (m_Controller == null || m_CurrentSpeedText == null)
            return;

        float currentSpeed = m_Controller.Speed;
        m_CurrentSpeedText.text = currentSpeed.ToString("F2");
    }
}
