using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Game.Stages;

namespace Game.Interaction
{
    public class ReportCardHUDController : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _DamageDealt;
        [SerializeField] private TextMeshProUGUI _DamageTaken;

        [SerializeField] private TextMeshProUGUI _Players_Eliminated;
        [SerializeField] private TextMeshProUGUI _Player_RemainingTowers;
        [SerializeField] private TextMeshProUGUI _Budget_Spent;
        [SerializeField] private TextMeshProUGUI _Budget_Remaining;
        [SerializeField] private TextMeshProUGUI _Ammo_Purchased;
        [SerializeField] private TextMeshProUGUI _Ammo_Shot;
        [SerializeField] private TextMeshProUGUI _Ammo_Hit;

        [SerializeField] private TextMeshProUGUI _Victory;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetupLabels(ReportStage.ReportData playerData, bool winner)
        {
            if (winner) _Victory.enabled = true;

            _DamageDealt.text               = $"You have taken {playerData.Damage_Dealt} damage";
            _DamageTaken.text               = $"You have dealt {playerData.Damage_Taken} damage";
            _Players_Eliminated.text        = $"You have eliminated {playerData.Data[ReportStage.ReportData.Players_Eliminated]} players";
            _Player_RemainingTowers.text    = $"You have {playerData.Data[ReportStage.ReportData.Player_RemainingTowers]} towers remaining";
            _Budget_Spent.text              = $"You have spent {playerData.Data[ReportStage.ReportData.Budget_Spent]} coins";
            _Budget_Remaining.text          = $"You finished with a budget of {playerData.Data[ReportStage.ReportData.Budget_Remaining]}";
            _Ammo_Purchased.text            = $"You have purchased {playerData.Data[ReportStage.ReportData.Ammo_Purchased]} weapons";
            _Ammo_Shot.text                 = $"You have shot {playerData.Data[ReportStage.ReportData.Ammo_Shot]} ammunition";
            _Ammo_Hit.text                  = $"You have hit a player {playerData.Data[ReportStage.ReportData.Ammo_Hit]} times";
        }

        public void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}