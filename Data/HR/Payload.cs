namespace MESystem.Data.HR;

public class Payload
{
    string? fullName;
    string? workplaceId;
    bool? status;

    Salary? salary;
    string? phoneNumber;
    string? email;

    BankAccount? bankAccount;

    SocialIdDocument? socialIdDocument;

    string? gender;
    DateTime startDate;
    DateTime dob;
    string? attendeeCode;

    public string? FullName { get => fullName; set => fullName=value; }
    public string? WorkplaceId { get => workplaceId; set => workplaceId=value; }
    public bool? Status { get => status; set => status=value; }
    public Salary? Salary { get => salary; set => salary=value; }
    public string? PhoneNumber { get => phoneNumber; set => phoneNumber=value; }
    public string? Email { get => email; set => email=value; }
    public BankAccount? BankAccount { get => bankAccount; set => bankAccount=value; }
    public SocialIdDocument? SocialIdDocument { get => socialIdDocument; set => socialIdDocument=value; }
    public string? Gender { get => gender; set => gender=value; }
    public DateTime StartDate { get => startDate; set => startDate=value; }
    public DateTime Dob { get => dob; set => dob=value; }
    public string? AttendeeCode { get => attendeeCode; set => attendeeCode=value; }
}
