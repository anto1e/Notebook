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
        protected internal ObservableCollection<Note> Notes_group { get; set; }
        private static NotesCollection instance = null;
        public NotesCollection()
        {
            Notes_temp = new ObservableCollection<Note> { };
            Notes = new ObservableCollection<Note> { };
            Notes_group = new ObservableCollection<Note> { };
        }
        public void JoinAllAndGroup()           //Объединение общих и групповых заметок
        {
            foreach (var elem in Notes_group)
            {
                Notes.Add(new Note { Number = elem.Number * -1, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date, Type = elem.Type });
            }
        }

        public static NotesCollection getInstance()
        {
            if (instance == null)
                instance = new NotesCollection();
            return instance;
        }
        public void addNote(string header, string message, string author, string date, int type)       //Добавление заметки в коллекцию
        {
            var index = Notes.Count()+1;
                Notes.Add(new Note { Number = index, Header = header, Message = message, Author = author, Date = date,Type = type });

        }
        public void fillPrivateTemp(string email)   //Наполнение временной коллекции личными заметками
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Type == 0)
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date });
                }

            }
        }

        public void fillGroupTemp(string email)   //Наполнение временной коллекции личными заметками
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Type == 1)
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date });
                }

            }
        }
        public void findAllByPart(string str)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length > 0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date });
                }
            }
        }
        public void findPrivateByPart(string str, string email)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length > 0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (elem.Type == 0)
                        Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date });
                }
            }
        }
        public void findGroupByPart(string str, string email)
        {
            Notes_temp.Clear();
            foreach (var elem in Notes)
            {
                if (elem.Header.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0 || (elem.Message.Length > 0 && elem.Message.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (elem.Type == 1)
                        Notes_temp.Add(new Note { Number = elem.Number, Header = elem.Header, Message = elem.Message, Author = elem.Author, Date = elem.Date });
                }
            }
        }
        public void change_indexes()
        {
            foreach (var elem in Notes)
            {
                if (elem.Type == 0)
                {
                    var indx = Notes.IndexOf(elem) + 1;
                    elem.Number = indx;
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
            string filename = "Notes.json";
            if (File.Exists(Path.Combine(folderPath, filename)))
            {
                string readText = File.ReadAllText(Path.Combine(folderPath, filename));
                Notes = JsonConvert.DeserializeObject<ObservableCollection<Note>>(readText);
            }
        }
        public void ClearAll()
        {
            for (int i = Notes.Count - 1; i >= 0; i--)
            {
                Notes.RemoveAt(i);
            }
            for (int i=Notes_group.Count - 1; i >= 0; i--)
                {
                    Notes_group.RemoveAt(i);
                }
        }
    }
}