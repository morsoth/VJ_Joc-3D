using System;
using System.IO;
using UnityEngine;

public class TerrainReading : MonoBehaviour
{
    public string fileName = "Level1";

    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "Levels", fileName);

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                int commentIndex = line.IndexOf('%');
                string processedLine = line;

                if (commentIndex >= 0)
                {
                    processedLine = line.Substring(0, commentIndex).Trim();
                }

                if (string.IsNullOrWhiteSpace(processedLine)) continue;

                string[] nums = processedLine.Split(' ');

                foreach (string num in nums)
                {
                    try
                    {
                        int n = int.Parse(num);
                        Debug.Log($"Number: {n}");
                    }
                    catch (FormatException)
                    {
                        Debug.LogError($"Invalid format for: '{num}'");
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Debug.LogError($"File {fileName} wasn't found in {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Error when reading the file: {e.Message}");
        }
    }
}
