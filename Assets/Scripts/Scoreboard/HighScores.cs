using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

public class HighScores : MonoBehaviour
{
    public class HighScore : IComparable<HighScore>
    {
        public string Name;
        public int Score;

        public HighScore(int score, string name)
        {
            this.Score = score;
            this.Name = name;
        }

        public int CompareTo(HighScore other)
        {
            // In case of tie, sort by name
            if (this.Score == other.Score)
                return this.Name.CompareTo(other.Name);

            // Descending order
            return other.Score.CompareTo(this.Score);
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", this.Score, this.Name);
        }
    }

    private List<HighScore> scores;

    /// <summary>
    /// Separator for saving high scores to file.
    /// </summary>
    private const char separator = ',';

    /// <summary>
    /// Current song in order to separate different songs' high scores.
    /// </summary>
    [Tooltip("Unique song descriptor for different high scores.")]
    public string song = "undefined";

    public void Save()
    {
        // TODO: Maybe best to write async, but it's probably small enough for now
        FileStream fs = File.Open("highscores_"+ song + ".csv", FileMode.Create, FileAccess.Write);

        StreamWriter sw = new StreamWriter(fs);

        foreach(HighScore entry in scores)
        {
            sw.WriteLine(string.Format("{1}{0}{2}", separator, entry.Score, entry.Name));
        }

        sw.Flush();
        
        fs.Close();
    }

    public void Load()
    {
        scores = new List<HighScore>();

        FileStream fs = File.Open("highscores_" + song + ".csv", FileMode.OpenOrCreate, FileAccess.Read);

        StreamReader sr = new StreamReader(fs);

        while(sr.Peek() != -1)
        {
            string[] values = sr.ReadLine().Split(separator);

            scores.Add(new HighScore(int.Parse(values[0]), values[1]));
        }

        scores.Sort();

        fs.Close();
    }

    /// <summary>
    /// Adds a new entry and returns its current position in the high scores.
    /// </summary>
    /// <param name="score"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public int Add(int score, string name)
    {
        // Remove separator from string to avoid issues
        name.Replace(separator.ToString(), "");

        HighScore obj = new HighScore(score, name);

        scores.Add(obj);
        scores.Sort();

        return scores.IndexOf(obj);
    }


	void Start()
    {
        Load();

        /*
        Add(103, "Byzantite");
        Add(73, "Crystal");
        Add(1001, "Artichoke");
        Add(1001, "Cheater");

        Save();
        */
	}
}
