using System.Text.RegularExpressions;
#region var
int foundFiles = 0;
int foundLines = 0;
int occurrences = 0;
#endregion

#region main program
GetArguments(args, out string path, out string fileName, out string searchText, out bool insensitive, out bool invalid, out bool recursive);
if (!invalid)
{
    var pathFiles = Directory.GetFiles(path, fileName, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    foreach (var pathFile in pathFiles)
    {
        var lines = File.ReadAllLines(pathFile);
        bool fileContainsWord = false;
        bool found = false;
        for (int j = 0; j < lines.Length; j++)
        {
            if (CheckIfContainsWord(insensitive, lines[j], searchText , ref occurrences))
            {
                if(!fileContainsWord)
                {
                    foundFiles++;
                }
                fileContainsWord = true;
                string line = lines[j];
                foundLines++;
                if (!found)
                {
                    Console.WriteLine(pathFile);
                }
                found = true;
                Console.WriteLine(GetOutputSentence(searchText, line, j + 1, insensitive));
            }
        }
    }
    Console.WriteLine("SUMMARY");
    Console.WriteLine($"    Files found: {foundFiles}");
    Console.WriteLine($"    Lines found: {foundLines}");
    Console.WriteLine($"    Occurrences found: {occurrences}");
}
#endregion

#region methods
string GetOutputSentence(string searchText, string line, int currentLine, bool insensitive)
{
    int _ = 0;
    string output = $"      {currentLine}: ";
    for (int i = 0; i < line.Length; i++)
    {
        if (line[i] == searchText[0] && i + searchText.Length <= line.Length 
            || insensitive && i + searchText.Length <= line.Length && char.ToLower(line[i]) == char.ToLower(searchText[0]))
        {
            if (CheckIfContainsWord(insensitive, line.Substring(i, searchText.Length), searchText, ref _))
            {
                output += $">>>{searchText.ToUpper()}<<<";
                i += searchText.Length - 1;
            }
        }
        else
        {
            output += line[i];
        }
    }
    return output;
}
bool CheckIfContainsWord(bool insensitive, string line, string searchText, ref int occurrences)
{
    bool containsWord = false;
    for (int i = 0; i < line.Length; i++)
    {
        if (insensitive && char.ToLower(line[i]) == char.ToLower(searchText[0]) && i + searchText.Length <= line.Length)
        {
            for (int j = 0; j < searchText.Length; j++)
            {
                if (char.ToLower(line[i + j]) != char.ToLower(searchText[j]))
                {
                    break;
                }
                else if (j == searchText.Length - 1)
                {
                    occurrences++;
                    containsWord = true;
                }
            }
        }
        else if (!insensitive && line.Contains(searchText))
        {
            containsWord = true;
            occurrences += Regex.Matches(line, searchText).Count;
            break;
        }   
    }
    return containsWord;
}
void GetArguments(string[] arguments, out string path, out string fileName, out string searchText, out bool incursive, out bool invalid, out bool recursive)
{
    path = "";
    fileName = "";
    searchText = "";
    incursive = false;
    invalid = false;
    recursive = false;
    if (arguments.Length < 3 || arguments.Length > 6)
    {
        Console.WriteLine("Wrong number of arguments");
        invalid = true;
    }
    else if (arguments.Length == 3)
    {
        path = arguments[0];
        fileName = arguments[1];
        searchText = arguments[2];
    }
    else if (arguments.Length == 4)
    {
        if (arguments[0] == "-R")
        {
            recursive = true;
        }
        else if (arguments[0] == "-i")
        {
            insensitive = true;
        }
        else if (arguments[0] is "-iR" or "-Ri")
        {
            recursive = true;
            insensitive = true;
        }
        path = arguments[1];
        fileName = arguments[2];
        searchText = arguments[3];
    }
    else if (arguments.Length == 5)
    {
        recursive = true;
        insensitive = true;
        path = arguments[2];
        fileName = arguments[3];
        searchText = arguments[4];
    }
}
#endregion