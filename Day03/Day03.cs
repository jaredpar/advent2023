using Advent.Util;

Part1("Input.txt");
void Part1(string filePath)
{
    var schematic = new Schematic(filePath);
    var sum = 0;
    foreach (var number in schematic.GetPartNumbers())
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

    public IEnumerable<(Memory<char> Memory, int Row, int Column)> GetNumbers()
    {
        for (int r = 0; r < Grid.Rows; r++)
        {
            var memory = Grid.GetRowMemory(r);
            var column = 0;
            while (column < memory.Length)
            {
                if (!char.IsDigit(memory.Span[column]))
                {
                    column++;
                    continue;
                }

                var start = column;
                while (column < memory.Length && char.IsDigit(memory.Span[column]))
                {
                    column++;
                }

                var number = memory.Slice(start, column - start);
                yield return (number, r, start);
            }
        }
    }
}

