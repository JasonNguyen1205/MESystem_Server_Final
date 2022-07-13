namespace MESystem.Data.HR;

public class BankAccount
{
    string? bankCode;
    string? accountNumber;

    public string? BankCode { get => bankCode; set => bankCode=value; }
    public string? AccountNumber { get => accountNumber; set => accountNumber=value; }
}
