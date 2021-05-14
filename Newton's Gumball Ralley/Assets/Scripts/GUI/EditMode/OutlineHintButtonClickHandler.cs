using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Hints;

namespace GUI.EditMode
{
    public class OutlineHintButtonClickHandler : MonoBehaviour
    {
        private Button button;
        private Image buttonImage;
        private List<OutlineHintOrderTracker> outlineHintsOrderTrackers;
        private int outlineHintDisplayIndex;

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            ResetOrderTrackerVariables();
            SetClickabilityBasedOnOrderTrackers();
        }

        private void ResetOrderTrackerVariables()
        {
            outlineHintsOrderTrackers = GameObject
                .Find(GameStateManager.HINTS_KEY)
                .GetComponentsInChildren<OutlineHintOrderTracker>(true)
                .ToList();
            outlineHintsOrderTrackers.Sort(delegate
                (OutlineHintOrderTracker orderTracker1, OutlineHintOrderTracker orderTracker2)
            {
                return orderTracker1.order.CompareTo(orderTracker2.order);
            }
            );
            outlineHintsOrderTrackers.ForEach(orderTracker => orderTracker.gameObject.SetActive(false));
            outlineHintDisplayIndex = 0;
        }

        private void SetClickabilityBasedOnOrderTrackers()
        {
            bool shouldBeClickable = outlineHintDisplayIndex < outlineHintsOrderTrackers.Count;
            button.interactable = shouldBeClickable;
            buttonImage.color = shouldBeClickable ? Color.white : Color.clear;
        }

        public void ShowNextHint()
        {
            if (outlineHintDisplayIndex >= outlineHintsOrderTrackers.Count)
            {
                return;
            }
            outlineHintsOrderTrackers[outlineHintDisplayIndex++].gameObject.SetActive(true);
            SetClickabilityBasedOnOrderTrackers();
        }
    }
}
