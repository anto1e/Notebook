using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace FineNotes
{
    public class Note : INotifyPropertyChanged
    {
        private string header;
        private string message;
        private string author;
        private string date;
        private int number;
        List<string> allowers = new List<string>();

        public int Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    OnPropertyChanged("Number");
                }
            }
        }
        public string Header
        {
            get { return header; }
            set
            {
                if (header != value)
                {
                    header = value;
                    OnPropertyChanged("Header");
                }
            }
        }
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged("Message");
                }
            }
        }
        public string Author
        {
            get { return author; }
            set
            {
                if (author != value)
                {
                    author = value;
                    OnPropertyChanged("Author");
                }
            }
        }
        public string Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged("Date");
                }
            }
        }
        public List<string> Allowers
        {
            get { return allowers; }
            set
            {
                allowers = value;
                OnPropertyChanged("Allowers");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

