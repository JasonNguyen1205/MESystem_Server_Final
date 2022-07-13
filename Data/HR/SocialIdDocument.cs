namespace MESystem.Data.HR;

public class SocialIdDocument
{
    string? number;
    DateTime? date;
    string? place;

    public string? Number { get => number; set => number=value; }
    public DateTime? Date { get => date; set => date=value; }
    public string? Place { get => place; set => place=value; }
    DateTime issuedDate;
    public DateTime IssuedDate { get => issuedDate; set => issuedDate=value; }
    string issuedPlace;
    public string IssuedPlace { get => issuedPlace; set => issuedPlace=value; }

}
