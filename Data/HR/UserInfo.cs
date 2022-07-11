using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Reflection.Metadata;

namespace MESystem.Data.HR;

public class UserInfo
{
    [Key]
    [Column(nameof(UserEnrollNumber),TypeName= "BigInteger")]
    public long UserEnrollNumber { get; set; }
    [Column(nameof(UserFullCode),TypeName= "NVARCHAR(100)")]
    public string? UserFullCode { get; set; }
    [Column(nameof(UserFullName),TypeName= "NVARCHAR(100)")]
    public string? UserFullName { get; set; }
    [Column(nameof(UserLastName),TypeName= "NVARCHAR(100)")]
    public string? UserLastName { get; set; }
    [Column(nameof(UserEnrollName),TypeName= "NVARCHAR(100)")]
    public string? UserEnrollName { get; set; }
    [Column(nameof(UserCardNo),TypeName= "NVARCHAR(100)")]
    public string? UserCardNo { get; set; }
    [Column(nameof(UserHireDay),TypeName= "DATE")]
    public DateTime? UserHireDay { get; set; }
    [Column(nameof(UserIDTitle),TypeName= "int")]
	public int? UserIDTitle { get; set; }
    [Column(nameof(UserSex),TypeName= "int")]
    public int? UserSex { get; set; }
    [Column(nameof(UserBirthDay),TypeName= "NVARCHAR(100)")]
    public string? UserBirthDay { get; set; }
    [Column(nameof(UserBirthPlace),TypeName= "int")]
    public int? UserBirthPlace { get; set; }
    [Column(nameof(UserPhoto),TypeName= "image")]
    public byte[]? UserPhoto { get; set; }
    [Column(nameof(UserNoted),TypeName= "NVARCHAR(100)")]
    public string? UserNoted { get; set; }
    [Column(nameof(UserPW),TypeName= "NVARCHAR(100)")]
    public string? UserPW { get; set; }
    [Column(nameof(UserPrivilege),TypeName= "int")]
    public int? UserPrivilege { get; set; }
    [Column(nameof(UserEnabled),TypeName= "bit")]
    public bool? UserEnabled { get; set; }
    [Column(nameof(UserIDC),TypeName= "int")]
    public int? UserIDC { get; set; }
    [Column(nameof(UserIDD),TypeName= "int")]
    public int? UserIDD { get; set; }
    [Column(nameof(SchID),TypeName= "int")]
    public int? SchID { get; set; }
    [Column(nameof(UserGroup),TypeName= "int")]
    public int? UserGroup { get; set; }
    [Column(nameof(UserTZ),TypeName= "NVARCHAR(100)")]
    public string? UserTZ { get; set; }
    [Column(nameof(UserPIN1),TypeName= "NVARCHAR(100)")]
    public string? UserPIN1 { get; set; }
    [Column(nameof(PushCardID),TypeName= "NVARCHAR(100)")]
    public string? PushCardID { get; set; }
    [Column(nameof(UserNationality),TypeName= "int")]
    public int? UserNationality { get; set; }
    [Column(nameof(UserPeople),TypeName= "int")]
    public int? UserPeople { get; set; }
    [Column(nameof(UserNativeCountry),TypeName= "NVARCHAR(100)")]
    public string? UserNativeCountry { get; set; }
    [Column(nameof(UserIDCard),TypeName= "NVARCHAR(100)")]
    public string? UserIDCard { get; set; }
    [Column(nameof(IDCardPlaceOfIssue),TypeName= "NVARCHAR(100)")]
    public string? IDCardPlaceOfIssue { get; set; }
    [Column(nameof(UserCalledName),TypeName= "NVARCHAR(100)")]
    public string? UserCalledName { get; set; }
    [Column(nameof(UserAddress),TypeName= "NVARCHAR(100)")]
    public string? UserAddress { get; set; }
    [Column(nameof(UserPhoneNumber),TypeName= "NVARCHAR(100)")]
    public string? UserPhoneNumber { get; set; }
    [Column(nameof(RelationshipName),TypeName= "NVARCHAR(100)")]
    public string? RelationshipName { get; set; }
    [Column(nameof(VerifyType),TypeName= "int")]
    public int? VerifyType { get; set; }
}

