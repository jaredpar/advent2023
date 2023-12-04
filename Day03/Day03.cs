using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Advent.Util;

Part1("Input.txt");
Part2("Input.txt");
void Part1(string filePath)
{
    var schematic = new Schematic(filePath);
    var sum = 0;
    foreach (var number in schematic.GetPartNumbers())
        sum += number;
    Console.WriteLine(sum);
}

void Part2(string filePath)
{
    var schematic = new Schematic(filePath);
    var sum = 0;
    foreach (var number in schematic.GetGearRatios())
        sum += number;
    Console.WriteLine(sum);
}

internal sealed class Schematic
{
    public string[] Lines { get; }
    public Grid<char> Grid { get; }

    public Schematic(string filePath)
    {
        Lines = File.ReadAllLines(filePath);
        Grid = Grid<char>.Parse(Lines);
    }

    public bool IsNextToSymbol(int row, int column)
    {
        foreach (var (r, c) in Grid.GetAdjacentIndexes(row, column))
        {
            var item = Grid.GetValue(r, c);
            if (!char.IsDigit(item) && item != '.')
                return true;
        }

        return false;
    }

    public IEnumerable<int> GetPartNumbers()
    {
        foreach (var tuple in GetNumbers())
        {
            for (var i = 0; i < tuple.Memory.Length; i++)
            {
                if (IsNextToSymbol(tuple.Row, tuple.Column + i))
                {
                    yield return int.Parse(tuple.Memory.Span);
                    break;
                }
            }
        }
    }

    public GridSpan<char>? GetNumber(int row, int column)
    {
        var span = Grid.GetGridSpan(row);
        if (!char.IsDigit(span[column]))
        {
            return null;
        }

        var start = column;
        while (start > 0 && char.IsDigit(span[start - 1]))
        {
            start--;
        }

        while (column + 1 < span.Length && char.IsDigit(span[column + 1]))
        {
            column++;
        }

        return span.Slice(start, (column - start) + 1);
    }

    public IEnumerable<int> GetGearRatios()
    {
        for (int r = 0; r < Grid.Rows; r++)
        {
            for (int c = 0; c < Grid.Columns; c++)
            {
                if ('*' == Grid.GetValue(r, c))
                {
                    var numbers = GetAdjacentNumbers(r, c).ToList();
                    if (numbers.Count == 2)
                    {
                        var number1 = int.Parse(numbers[0].Span);
                        var number2 = int.Parse(numbers[1].Span);
                        yield return number1 * number2;
                    }
                }
            }
        }
    }

    public IEnumerable<GridSpan<char>> GetAdjacentNumbers(int row, int column)
    {
        var startColumn = column == 0 ? 0 : column - 1;
        var maxColumn = Math.Min(column + 1, Grid.Columns - 1);
        var r = row == 0 ? 0 : row - 1;
        var c = startColumn;
        while (r <= row + 1 && row < Grid.Rows)
        {
            while (c <= maxColumn)
            {
                if (r == row && c == column)
                {
                    c++;
                    continue;
                }

                var span = GetNumber(r, c);
                if (span.HasValue)
                {
                    yield return span.Value;
                    c = span.Value.End;
                }
                else
                {
                    c++;
                }
            }

            r++;
            c = startColumn;
        }
    }

    public IEnumerable<GridSpan<char>> GetNumbers()
    {
        foreach (var gridSpan in Grid.GetGridSpans())
        {
            var column = 0;
            while (column < gridSpan.Length)
            {
                if (!char.IsDigit(gridSpan[column]))
                {
                    column++;
                    continue;
                }

                var start = column;
                while (column < gridSpan.Length && char.IsDigit(gridSpan[column]))
                {
                    column++;
                }

                yield  return gridSpan.Slice(start, column - start);
            }
        }
    }
}

