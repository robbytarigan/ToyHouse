<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.GoogleCheckout.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <ul>
                <li><b>If you're using this gateway remember that you should set your store primary
                    currency to US Dollar or GBP.</b></li>
                <li><b>Tax is not supported for Google Checkout now (beta-version).</b> </li>
                <li><b>Discounts are not supported for Google Checkout now (beta-version).</b> </li>
                <li><b>Checkout attributes are not supported for Google Checkout now (beta-version).</b> </li>
                <li><b>Reward points are not supported for Google Checkout now (beta-version).</b> </li>
                <li><b>Gift cards are not supported for Google Checkout now (beta-version).</b> </li>
                <li><b>Shipping rate computation method should NOT be set to real time methods like
                    USP, USPS.</b> </li>
            </ul>
            <br />
            1. Go to <a href="http://sandbox.google.com/checkout/sell/">http://sandbox.google.com/checkout/sell/</a>
            to set up test accounts in the Google Checkout Sandbox service. The Sandbox is a
            development environment that is designed to help you test your Google Checkout implementation.
            The Sandbox offers the same functionality as the production Google Checkout system
            with the following exceptions:
            <br />
            <ul>
                <li>The Sandbox requires you to use test credit card numbers.</li>
                <li>The Sandbox does not actually execute debits and credits.</li>
                <li>The Sandbox user interface displays an overlay that indicates you are working in
                    the Sandbox environment.</li>
            </ul>
            <br />
            You need to create two test accounts in the Sandbox. One of these accounts will
            function as a <b>buyer account</b> and the other will function as your <b>merchant account</b>.
            Please note that Google Checkout will not let you use your merchant account to complete
            an order at your own store. (In other words, the same account can not function as
            both the customer and the merchant for the same transaction.) In addition, you need
            to provide different information to create these two accounts.
            <br />
            <ul>
                <li>Create your buyer account at <a href="http://sandbox.google.com/checkout">http://sandbox.google.com/checkout</a>.</li>
                <li>Create your merchant account at <a href="http://sandbox.google.com/checkout/sell/">
                    http://sandbox.google.com/checkout/sell/</a>.</li>
            </ul>
            <br />
            The following guidelines explain how to set up your test accounts:
            <br />
            <ul>
                <li>Skip any sections that ask for your bank account information. Since the Sandbox
                    system does not process billing or payments, this information is not necessary when
                    you are testing your implementation.</li>
                <li>Enter any name and address as long as the State field contains a valid two-letter
                    abbreviation for a U.S. state and the Zip Code field contains a five-digit or nine-digit
                    zip code. (You do not need to enter the correct zip code for the address.)</li>
                <li>Enter any 10-digit phone number for the Phone Number field.</li>
                <li>Enter any value in either the Federal tax ID or Social Security number fields.</li>
                <li>Use one of the credit card numbers in the following table:</li>
            </ul>
            <br />
            <p>
                <table border="1" cellspacing="0" cellpadding="0" width="80%">
                    <tr>
                        <td class="cch">
                            Card Type
                        </td>
                        <td class="cch">
                            Card Number
                        </td>
                        <td class="cch">
                            CVC
                        </td>
                        <td class="cch">
                            Expiration Date
                        </td>
                    </tr>
                    <tr>
                        <td class="ccname">
                            American Express
                        </td>
                        <td class="ccinfo">
                            3782 8224 6310 005
                        </td>
                        <td class="ccinfo">
                            any four digits
                        </td>
                        <td class="ccinfo">
                            any future date
                        </td>
                    </tr>
                    <tr>
                        <td class="ccname">
                            Discover
                        </td>
                        <td class="ccinfo">
                            6011 1111 1111 1117
                        </td>
                        <td class="ccinfo">
                            any three digits
                        </td>
                        <td class="ccinfo">
                            any future date
                        </td>
                    </tr>
                    <tr>
                        <td class="ccname">
                            MasterCard
                        </td>
                        <td class="ccinfo">
                            5555 5555 5555 4444
                        </td>
                        <td class="ccinfo">
                            any three digits
                        </td>
                        <td class="ccinfo">
                            any future date
                        </td>
                    </tr>
                    <tr>
                        <td class="ccname">
                            VISA
                        </td>
                        <td class="ccinfo">
                            4111 1111 1111 1111
                        </td>
                        <td class="ccinfo">
                            any three digits
                        </td>
                        <td class="ccinfo">
                            any future date
                        </td>
                    </tr>
                </table>
            </p>
            <br />
            <br />
            2. Go to <a href="http://checkout.google.com/sell/signup">http://checkout.google.com/sell/signup</a>
            to sign up for a Google Checkout merchant account. Complete the sign-up process
            and provide valid values for all fields. You will need the federal tax ID number
            for your business or a credit card and your Social Security number. Please note
            that you will use this account for your production service whereas the accounts
            you created in the previous step are for testing your Checkout integration.
            <br />
            <br />
            3. Sign in to the accounts that you created in steps 1 and 2 to locate the <b>Merchant
                ID</b> and <b>Merchant Key</b> for each account. You will need these values
            to create Google Checkout buttons and to send API requests to Google Checkout. After
            signing in to each account, click on the <b>Settings</b> tab. Then click on the
            <b>Integration</b> link on the left side of the page. Your 10- or 15-digit Merchant
            ID and your Merchant Key will both be listed under the <b>Account information</b>
            header. <b>You should never share your Merchant Key with anyone</b>. Google uses
            your Merchant Key to authenticate your API requests, and no Google representative
            will ever ask you for your Merchant Key.
            <br />
            The callback method needs to be XML, and make sure that Shopping cart post security
            is checked. The API callback URL needs to be http://YourStoreURL/GooglePostHandler.aspx
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use Sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSandbox" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Google Vendor ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGoogleVendorId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Google Merchant Key:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtGoogleMerchantKey" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Authenticate callback:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAuthenticateCallback" runat="server"></asp:CheckBox>
        </td>
    </tr>
</table>
