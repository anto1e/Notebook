using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FineNotes
{
    public class NotesCollection                        //Коллекция заметок, паттерн одиночка
    {
        protected internal ObservableCollection<Note> Notes { get; set; }
        private static NotesCollection instance = null;
        public NotesCollection()
        {
            Notes = new ObservableCollection<Note> {
            new Note{Number=1,Header="Test",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=2,Header="Test1",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=3,Header="Test2",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=4,Header="Test3",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=5,Header="Test4",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            };
        }
        public static NotesCollection getInstance()
        {
            if (instance == null)
                instance = new NotesCollection();
            return instance;
        }
        public void addNote(string header,string message, string author, string date)
        {
            var index = Notes.Count()+1;
            Notes.Add(new Note {Number = index, Header = header, Message = message, Author = author, Date = date});
            
        }
    }
}

