using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class DeckSerializer
{
    static string filePath = Application.persistentDataPath + "/decks";

    public static void Serialize()
    {
        if (false)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            var data = DeckMenuManager.preserved_decks;

            formatter.Serialize(fileStream, data);

            fileStream.Close();
        }
    }

    static void FillDefault()
    {
        Debug.Log("Default");
        if (DeckMenuManager.preserved_decks == null)
        {
            DeckMenuManager.preserved_decks = new List<Deck>();
            for (int i = 0; i < 5; ++i)
            {
                DeckMenuManager.preserved_decks.Add(null);
            }
        }    
    }

    public static void Deserialize()
    {
        FillDefault();

        return;

        FileInfo fileInfo = new FileInfo(filePath);

        if (fileInfo.Exists && fileInfo.Length > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            try
            {
                DeckMenuManager.preserved_decks = (List<Deck>)formatter.Deserialize(fileStream);
            }
            catch (Exception ex)
            {
                FillDefault();
            }

            fileStream.Close();
        }
        else
        {
            FillDefault();
        }

    }
}
