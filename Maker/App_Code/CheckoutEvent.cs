using System;
using MakerShop.Payments;
using MakerShop.Orders;

/// <summary>
/// Delegate for handling events for checkout
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void CheckedOutEventHandler(object sender, CheckedOutEventArgs e);

public delegate void CheckingOutEventHandler(object sender, CheckingOutEventArgs e);

public class CheckedOutEventArgs
{
    private CheckoutResponse _CheckoutResponse;

    public CheckedOutEventArgs(CheckoutResponse response) { _CheckoutResponse = response; }

    public int OrderId
    {
        get { return _CheckoutResponse.OrderId; }
    }

    public CheckoutResponse CheckoutResponse
    {
        get { return _CheckoutResponse; }
    }
}

public class CheckingOutEventArgs
{
    private bool _Cancel;
    private Payment _Payment;

    public CheckingOutEventArgs() { }

    public CheckingOutEventArgs(Payment p)
    {
        _Payment = p;
    }

    public bool Cancel
    {
        get { return _Cancel; }
        set { _Cancel = value; }
    }

    public Payment Payment
    {
        get { return _Payment; }
    }
}