var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\21\\input.txt");

var nboRobots = 2;

var numpad = new NumericKeypad();
var robots = new List<DirectionalKeypad>();
for (var i = 0; i < nboRobots; i++)
    robots.Add(new DirectionalKeypad());

var answer = 0;
foreach (var line in lines)
    answer += typeCode(line);

Console.WriteLine(answer);

int typeCode(string code)
{
    var requiredSequence = string.Empty;
    foreach (var character in code)
    {
        var result = string.Empty;
        var chosenOption = 0;
        for (var i = 0; i < 2; i++)
        {
            numpad.Backup();
            foreach (var robot in robots)
                robot.Backup();

            var optionResult = "";
            var sequence = numpad.MoveTo(character, i == 0);
            foreach (var move in sequence)
                pressButtons(move, 0, ref optionResult);

            if (result.Equals(string.Empty) || optionResult.Length < result.Length)
            {
                result = optionResult;
                chosenOption = i;
            }

            numpad.Restore();
            foreach (var robot in robots)
                robot.Restore();
        }

        var numpadSequence = numpad.MoveTo(character, chosenOption == 0);
        foreach (var move in numpadSequence)
            pressButtons(move, 0, ref requiredSequence);
    }

    return requiredSequence.Length * int.Parse(code[..^1]);
}

void pressButtons(char button, int robotIndex, ref string result)
{
    var sequence = robots[robotIndex].MoveTo(button);
    if (robotIndex == 1)
        result += sequence;
    else
        foreach (var move in sequence)
            pressButtons(move, robotIndex + 1, ref result);
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
                    if (y == 0 && dY > 0)
                    {
                        requiredSequence += MotionsX(x, dX);
                        requiredSequence += MotionsY(y, dY);
                    }
                    else if (y > 0 && dY == 0)
                    {
                        requiredSequence += MotionsX(x, dX);
                        requiredSequence += MotionsY(y, dY);
                    }
                    else
                    {
                        requiredSequence += MotionsX(x, dX);
                        requiredSequence += MotionsY(y, dY);
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

    public string MoveTo(char button, bool upFirst)
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
                        if (!upFirst)
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

    private int bX;
    private int bY;

    public void Backup()
    {
        bX = x;
        bY = y;
    }

    public void Restore()
    {
        x = bX;
        y = bY;
    }

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