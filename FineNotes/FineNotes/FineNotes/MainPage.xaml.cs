using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class MainPage : ContentPage
    {
        protected internal ObservableCollection<Note> Notes { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Notes = new ObservableCollection<Note> {
            new Note{Number=1,Header="Test",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=2,Header="Test1",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=3,Header="Test2",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=4,Header="Test3",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            new Note{Number=5,Header="Test4",Message="Message",Author="123@mail.ru",Date="03.07.2022"},
            };
            notesList.BindingContext = Notes;
        }

        public async void addBtnClicked(object sender, EventArgs args)       //Обработка нажатия на кнопку "Добавить"
        {
            await Navigation.PushAsync(new AddPage());
        }
        private double previousScrollPosition = 0;          //Переменная для хранения значения предыдущей позиции
        private int scrollXCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем уберется тулбар
        private int scrollYCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем появится тулбар
        void scrollHandler(object sender, Xamarin.Forms.ScrolledEventArgs e)        //Функция отображения/скрытия тулбара
        {
            if (Convert.ToInt16(e.ScrollY) > 10)
            {
                if (previousScrollPosition < e.ScrollY)
                {
                    if (scrollXCnt > 5)
                    {
                        //scrolled down
                        previousScrollPosition = e.ScrollY;
                        toolbar_layout.TranslateTo(0, 250, 150);
                    }
                    else
                    {
                        scrollXCnt++;
                        scrollYCnt = 0;
                    }
                }
                else
                {
                    if (scrollYCnt > 1)
                    {
                        if (Convert.ToInt16(e.ScrollY) == 0)
                            previousScrollPosition = 0;
                        previousScrollPosition = e.ScrollY;
                        toolbar_layout.TranslateTo(0, 0, 150);
                    }
                    else
                    {
                        scrollYCnt++;
                        scrollXCnt = 0;
                    }
                }
            }
        }
        void OnListViewItemSelected(object sender, EventArgs args) {                //Функция полученя заметки, на которую нажал пользователь
            Note SelectedItem = Notes.FirstOrDefault(itm => itm.Number == Convert.ToInt32(((TappedEventArgs)args).Parameter.ToString()));
        }
        protected internal void AddNote(Note note)
        {
            Notes.Add(note);
        }
    }
}

