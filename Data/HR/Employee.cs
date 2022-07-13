namespace MESystem.Data.HR;

public class Employee
{
    string companyCode;
    string? code;
    Payload payload;

    public string CompanyCode { get => companyCode; set => companyCode=value; }
    public string? Code { get => code; set => code=value; }
    public Payload Payload { get => payload; set => payload=value; }
}
