using Goat.Helper;
using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.U2D;

namespace Goat.AI.Feelings
{
    public class CustomerFeelings : MonoBehaviour
    {
        [SerializeField] private GameObject dotObject;
        [SerializeField] private MaterialPropertySetter resourceToFind;
        [SerializeField] private MaterialPropertySetter questionMark;
        [SerializeField] private Sprite satisfiedSprite;
        [SerializeField] private NPC npc;
        private Vector3 dotScaleAfter = new Vector3(0.45f, 0.4f, 0.45f);
        private Vector3 dotScaleBefore = new Vector3(0.25f, 0.25f, 0.25f);
        private bool playerNear;
        private Sequence sequence;
        private bool alreadyTransitioned;

        public void Setup()
        {
            npc.ItemsToGet.InventoryChangedEvent += ItemsToGet_InventoryChangedEvent;
            npc.ItemsToGet.InventoryResetEvent += ItemsToGet_InventoryChangedEvent;
            alreadyTransitioned = false;

            ChangeQuestionMark();
        }

        public void OnReturn()
        {
            npc.ItemsToGet.InventoryChangedEvent -= ItemsToGet_InventoryChangedEvent;
            npc.ItemsToGet.InventoryResetEvent -= ItemsToGet_InventoryChangedEvent;
        }

        private void ItemsToGet_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            ChangeQuestionMark();
        }

        private void ItemsToGet_InventoryChangedEvent()
        {
            ChangeQuestionMark();
        }

        private void ChangeQuestionMark()
        {
            if (npc.ItemsToGet.ItemsInInventory > 0)
            {
                var looper = npc.ItemsToGet.Items.GetEnumerator();
                looper.MoveNext();
                SearchingForResource(looper.Current.Key);
            }
            else
            {
                Satisfied();
            }
        }

        public void TransitionUI(bool playerNear)
        {
            if (playerNear && !alreadyTransitioned)
            {
                DotToQuestionMark();
            }
            else
            {
                QuestionMarkToDot();
            }
        }

        private void DotToQuestionMark()
        {
            alreadyTransitioned = true;
            if (sequence.NotNull())
                sequence.Complete();

            sequence = DOTween.Sequence();

            sequence.Append(dotObject.transform.DOScale(dotScaleAfter, 1).OnComplete(() => ActivateQuestioMark()));
        }

        private void ActivateQuestioMark()
        {
            dotObject.SetActive(false);
            resourceToFind.gameObject.SetActive(true);
            questionMark.gameObject.SetActive(true);
        }

        private void QuestionMarkToDot()
        {
            dotObject.SetActive(true);
            resourceToFind.gameObject.SetActive(false);
            questionMark.gameObject.SetActive(false);
            if (sequence.NotNull())
                sequence.Complete();

            sequence = DOTween.Sequence();
            sequence.Append(dotObject.transform.DOScale(dotScaleBefore, 1));
            alreadyTransitioned = false;
        }

        private void Satisfied()
        {
            resourceToFind.gameObject.SetActive(false);

            for (int i = 0; i < questionMark.MaterialValueToChanges.Count; i++)
            {
                MaterialValueToChange texture = questionMark.MaterialValueToChanges[i];
                if (texture.MaterialValue != MaterialValue.Texture) continue;
                texture.NewTexture = satisfiedSprite.texture;
                questionMark.ModifyValues();
            }
        }

        private void SearchingForResource(Resource res)
        {
            for (int i = 0; i < resourceToFind.MaterialValueToChanges.Count; i++)
            {
                MaterialValueToChange texture = resourceToFind.MaterialValueToChanges[i];
                if (texture.MaterialValue != MaterialValue.Texture) continue;
                texture.NewTexture = res.Image.texture;
                resourceToFind.ModifyValues();
            }
        }
    }
}