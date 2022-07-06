using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace FineNotes
{
    public class NotesCollection                        //Коллекция заметок, паттерн одиночка
    {
        readonly string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        protected internal ObservableCollection<Note> Notes { get; set; }
        protected internal ObservableCollection<Note> Notes_temp { get; set; }
        private static NotesCollection instance = null;
        public NotesCollection()
        {
            Notes_temp = new ObservableCollection<Note> {};
            Notes = new ObservableCollection<Note> {};
        }
        public static NotesCollection getInstance()
        {
            if (instance == null)
                instance = new NotesCollection();
            return instance;
        }
        public void addNote(string header,string message, string author, string date,List<string> list)
        {
            var index = Notes.Count()+1;
            Notes.Add(new Note {Number = index, Header = header, Message = message, Author = author, Date = date, Allowers=list});
        }
        public void fillPrivateTemp()   //Наполнение временной коллекции личными заметками
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Author == "123@mail.ru" && elem.Allowers.Count() == 0)
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Allowers = elem.Allowers });
                }
                
            }
        }
        public void fillGroupTemp()   //Наполнение временной коллекции групповыми заметками
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Author != "123@mail.ru" || elem.Allowers.Count() != 0)
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Allowers = elem.Allowers });
                }

            }
        }
        public void findAllByPart(string str)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length >0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Allowers = elem.Allowers });
                }
            }
        }
        public void findPrivateByPart(string str)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length > 0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (elem.Author == "123@mail.ru" && elem.Allowers.Count() == 0)
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Allowers = elem.Allowers });
                }
            }
        }
        public void findGroupByPart(string str)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length > 0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (elem.Author != "123@mail.ru" || elem.Allowers.Count() != 0)
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Allowers = elem.Allowers });
                }
            }
        }
        public void Save()          //Запись коллекции в файл(Json)
        {
            string filename = "Notes.json";
            if (String.IsNullOrEmpty(filename)) return;
            // перезаписываем файл
            string json = JsonConvert.SerializeObject(Notes.ToArray());
            File.WriteAllText(Path.Combine(folderPath, filename), json);
            // обновляем список файлов
        }
        public void Read()          //Чтение коллекции из файла(Json)
        {
            //Save();
            string filename = "Notes.json";
            if (File.Exists(Path.Combine(folderPath, filename)))
            {
                string readText = File.ReadAllText(Path.Combine(folderPath, filename));
                Notes = JsonConvert.DeserializeObject<ObservableCollection<Note>>(readText);
            }
        }
    }
}

