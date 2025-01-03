var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\21\\input.txt");

var nboRobots = 25;

var numpad = new NumericKeypad();
var robots = new List<DirectionalKeypad>();
for (var i = 0; i < nboRobots; i++)
    robots.Add(new DirectionalKeypad());

var answer = 0L;
foreach (var line in lines)
    answer += typeCode(line);

Console.WriteLine(answer);

long typeCode(string code)
{
    var requiredSequenceLength = 0L;
    foreach (var character in code)
    {
        var numpadSequence = numpad.MoveTo(character);
        foreach (var move in numpadSequence)
            pressButtons(move, 0, ref requiredSequenceLength);
    }

    return requiredSequenceLength * int.Parse(code[..^1]);
}

void pressButtons(char button, int robotIndex, ref long length)
{
    var sequence = robots[robotIndex].MoveTo(button);
    if (robotIndex == nboRobots - 1)
        length += sequence.Length;
    else
        foreach (var move in sequence)
            pressButtons(move, robotIndex + 1, ref length);
}

class DirectionalKeypad : Keypad
{
    public DirectionalKeypad()
    {
        x = 2;
        y = 0;

        buttons = new char[2, 3]
        {
            { 'X', '^', 'A' },
            { '<', 'v', '>' }
        };
    }

    public string MoveTo(char button)
    {
        var requiredSequence = string.Empty;

        for (var dY = 0; dY < 2; dY++)
        {
            for (var dX = 0; dX < 3; dX++)
            {
                if (buttons[dY, dX].Equals(button))
                {
                    if (dX == 0 && x > 0)
                    {
                        requiredSequence += MotionsY(y, dY);
                        requiredSequence += MotionsX(x, dX);
                    }
                    else if (x == 0 && dY == 0)
                    {
                        requiredSequence += MotionsX(x, dX);
                        requiredSequence += MotionsY(y, dY);
                    }
                    else
                    {
                        if (dX < x)
                        {
                            requiredSequence += MotionsX(x, dX);
                            requiredSequence += MotionsY(y, dY);
                        }
                        else
                        {
                            requiredSequence += MotionsY(y, dY);
                            requiredSequence += MotionsX(x, dX);
                        }
                    }
                    requiredSequence += "A";

                    x = dX;
                    y = dY;
                }
            }
        }

        return requiredSequence;
    }
}

class NumericKeypad : Keypad
{
    public NumericKeypad()
    {
        x = 2;
        y = 3;

        buttons = new char[4, 3]
        {
            { '7', '8', '9' },
            { '4', '5', '6' },
            { '1', '2', '3' },
            { 'X', '0', 'A' }
        };
    }

    public string MoveTo(char button)
    {
        var requiredSequence = string.Empty;

        for (var dY = 0; dY < 4; dY++)
        {
            for (var dX = 0; dX < 3; dX++)
            {
                if (buttons[dY, dX].Equals(button))
                {
                    if (y == 3 && dY < 3 && dX == 0)
                    {
                        requiredSequence += MotionsY(y, dY);
                        requiredSequence += MotionsX(x, dX);
                    }
                    else if (dY == 3 && x == 0)
                    {
                        requiredSequence += MotionsX(x, dX);
                        requiredSequence += MotionsY(y, dY);
                    }
                    else
                    {
                        if (dX < x)
                        {
                            requiredSequence += MotionsX(x, dX);
                            requiredSequence += MotionsY(y, dY);
                        }
                        else
                        {
                            requiredSequence += MotionsY(y, dY);
                            requiredSequence += MotionsX(x, dX);
                        }
                    }
                    requiredSequence += "A";

                    x = dX;
                    y = dY;
                }
            }
        }

        return requiredSequence;
    }
}

class Keypad
{
    public char[,] buttons;

    public int x;
    public int y;

    protected string MotionsX(int x, int dX)
    {
        if (dX > x)
            return string.Empty.PadLeft(Distance(x, dX), '>');
        else
            return string.Empty.PadLeft(Distance(x, dX), '<');
    }

    protected string MotionsY(int y, int dY)
    {
        if (dY > y)
            return string.Empty.PadLeft(Distance(y, dY), 'v');
        else
            return string.Empty.PadLeft(Distance(y, dY), '^');
    }

    protected int Distance(int a, int b) => a > b ? a - b : b - a;
}