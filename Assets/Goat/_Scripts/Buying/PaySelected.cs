using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PaySelected : SelectedCells
{
    [SerializeField] private Button paySelectedButton;
    [SerializeField] private RectTransform paySelectedTransform;
    [SerializeField] private Money money;
    [SerializeField] private TextMeshProUGUI priceTM;
    [SerializeField] private AudioCue errorSfx, confirmSfx;
    private int fullPrice;
    private Sequence payButtonAnimation;

    private void Awake()
    {
        fullPrice = 0;
        priceTM.text = fullPrice.ToString();
        paySelectedButton.onClick.AddListener(Pay);
    }

    public override void Add(UICell cell)
    {
        base.Add(cell);
        if (cell is ExpenseCell expenseCell)
            fullPrice += expenseCell.Price;
        priceTM.text = fullPrice.ToString();
    }

    public override void Subtract(UICell cell)
    {
        base.Subtract(cell);
        if (cell is ExpenseCell expenseCell)
            fullPrice -= expenseCell.Price;
        priceTM.text = fullPrice.ToString();
    }

    protected override void Clear()
    {
        base.Clear();
        fullPrice = 0;
        priceTM.text = fullPrice.ToString();
    }

    private void Pay()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] is ExpenseCell expenseCell && AnimatePayButton(money.CanPay(fullPrice)))
            {
                expenseCell.Pay();
                priceTM.text = fullPrice.ToString();
            }
        }
    }

    protected bool AnimatePayButton(bool validated)
    {
        if (payButtonAnimation != null)
        {
            payButtonAnimation.Complete();
        }
        payButtonAnimation = DOTween.Sequence();
        if (validated)
        {
            confirmSfx.PlayAudioCue();
            payButtonAnimation.Append(paySelectedTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
        }
        else
        {
            errorSfx.PlayAudioCue();
            payButtonAnimation.Append(paySelectedTransform.DOShakeAnchorPos(0.3f, new Vector3(5, 0, 0), 90));
        }
        return validated;
    }
}