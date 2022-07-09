using System.ComponentModel.DataAnnotations;

namespace MESystem.Helper;

public class PhoenixSerials
{

    public string? Serial => Calculation(RunningPart, out _serial)+_serial.ToString().PadLeft(4, '0');

    int runningPart;

    [Required]
    [Range(1, 29997, ErrorMessage = "Reach limit")]
    public int RunningPart { get => runningPart; set => runningPart=value; }

    private int _serial = 0;

    private PhoenixCharacter Calculation(int i, out int _serial)
    {
        PhoenixCharacter character = (PhoenixCharacter)(i/10000);
        _serial=(RunningPart-(10000*(i/10000)))==0 ? 1 : (RunningPart-(10000*(i/10000)));

        return character;
    }
    private PhoenixCharacter Ger_Calculation(int i, out int _serial)
    {
        PhoenixCharacter character = (PhoenixCharacter)(i/10000);
        _serial=(RunningPart-(10000*(i/10000)))==0 ? 1 : (RunningPart-(10000*(i/10000)));

        return character;
    }
}

enum PhoenixCharacter
{
    P,
    Q,
    O
}

enum Ger_PhoenixCharacter
{
    P,
    Q,
    O
}
