using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class MainPage : ContentPage
    {
        Session session = Session.getInstance();
        NotesCollection notCol = NotesCollection.getInstance();
        public MainPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Padding = new Thickness(20, 38, 20, 20);
            }
            SubscribeColChanging();
            if (!session.Online)
                notCol.Read();
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            
        }
        private async void LogoutBtnClicked(Object sender, EventArgs e)     //Нажата кнопка выхода
        {
            session.writeSession();
            session.Email = "";
            session.Online = false;
            session.Modified = false;
            notCol.Save();
            notCol.ClearAll();
            await Navigation.PopAsync();
        }
        bool toolBarBlocked = false;
        private string currentPage = "All";
        public async void RefreshBtnClicked(object sender, EventArgs e)      //Обновление кол-ва заметок
        {
                string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    session.Online = true;
                    conn.Close();
                }
                catch(Exception ex)
                {
                await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                Console.WriteLine(ex.Message);
                session.Online = false;
                    return;
                }
            if (session.Modified && session.Online)
            {
                session.clearNotes();
                session.insertAllNotes();
            }
            session.getElemsFromDB();
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
        }
        private void SubscribeColChanging()     //Функция для изменения отображения кол-ва заметок на главной странице при изменении коллекции
        {
            MessagingCenter.Subscribe<Page>(
              this, // кто подписывается на сообщения
              "CollectionChanged!",   // название сообщения
              (sender) => {
                      session.writeSession();
                      if (session.Modified && session.Online)
                      {
                          session.clearNotes();
                          session.insertAllNotes();
                          session.Modified = false;
                      }
                      int cntNotCol = notCol.Notes.Count;
                      int cntNotColPriv = notCol.Notes.Count(i => i.Type == 0);
                      int cntNotColGroup = notCol.Notes.Count(i => i.Type == 1);

                          notesList.BindingContext = notCol.Notes;
                          if (cntNotCol < 100)
                              label_all.Text = "Все(" + cntNotCol.ToString() + ")";
                          else
                              label_all.Text = "Все>99";
                          if (cntNotColPriv < 100)
                              label_priv.Text = "Личные(" + cntNotColPriv.ToString() + ")";
                          else
                              label_priv.Text = "Личные>99";
                          if (cntNotColGroup < 100)
                              label_group.Text = "Групповые(" + cntNotColGroup.ToString() + ")";
                          else
                              label_group.Text = "Групповые>99";
                          if (currentPage == "Private")
                          {
                              notCol.fillPrivateTemp(session.Email);
                                notesList.BindingContext = notCol.Notes_temp;
                            }
                          if (currentPage == "Group")
                          {
                              notCol.fillGroupTemp(session.Email);
                              notesList.BindingContext = notCol.Notes_temp;
                            }
                          if (currentPage == "All")
                            notesList.BindingContext = notCol.Notes;
              });    // вызываемое действие
                              MessagingCenter.Subscribe<Page>(
                this, // кто подписывается на сообщения
                "Show Toolbar!",   // название сообщения
                (sender) => {
                    toolbar_layout.TranslateTo(0, 0, 150);

                });
            MessagingCenter.Subscribe<Page>(
                this, // кто подписывается на сообщения
                "Group Changed!",   // название сообщения
                (sender) => {
                    toolbar_layout.TranslateTo(0, 0, 150);
                    session.clearNotes();
                    session.insertAllNotes();
                    session.getElemsFromDB();
                    session.Modified = false;
                });

        }
        private async void addBtnClicked(object sender, EventArgs args)       //Обработка нажатия на кнопку "Добавить"
        {
            await Navigation.PushAsync(new AddPage());
        }
        private double previousScrollPosition = 0;          //Переменная для хранения значения предыдущей позиции
        private int scrollXCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем уберется тулбар
        private int scrollYCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем появится тулбар
        private void scrollHandler(object sender, Xamarin.Forms.ScrolledEventArgs e)        //Функция отображения/скрытия тулбара
        {
            if (!toolBarBlocked)
            {
                if (Convert.ToInt16(e.ScrollY) > 10)
                {
                    if (previousScrollPosition < e.ScrollY)
                    {
                        if (scrollXCnt > 1)
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
        }
        bool hided = true;
        private async void OnListViewItemSelected(object sender, EventArgs args) {                //Функция полученя заметки, на которую нажал пользователь
            var frame = (Frame)sender;
            frame.BackgroundColor = Color.FromHex("DCE1C6");
            Note SelectedItem = notCol.Notes.FirstOrDefault(itm => itm.Number == Convert.ToInt32(((TappedEventArgs)args).Parameter.ToString()) && itm.Number == Convert.ToInt32(((TappedEventArgs)args).Parameter.ToString()));
            await Navigation.PushAsync(new ModifyPage(SelectedItem));
            frame.BackgroundColor = Color.FromHex("F9FFE2");
        }
        private async void OnSwiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Right:
                    if (currentPage == "Group")
                    {
                        underline_priv.TranslationX = 0;
                        underline_priv.BackgroundColor = Color.FromHex("BBFA8A");
                        currentPage = "Private";
                        underline_group.BackgroundColor = Color.FromHex("5873FF");
                        underline_group.TranslationX = 1000;
                        await Grid_messages.TranslateTo(1000, 0, 150);
                        if (toolBarBlocked)
                        {
                            SearchClicked(null, null);
                        }
                        else
                            notCol.fillPrivateTemp(session.Email);
                        notesList.BindingContext = notCol.Notes_temp;
                        await Grid_messages.TranslateTo(-1000, 0, 0);
                        await Grid_messages.TranslateTo(0, 0, 150);
                    } else if(currentPage == "Private")
                    {
                        underline_all.TranslationX = 0;
                        underline_all.BackgroundColor = Color.FromHex("BBFA8A");
                        currentPage = "All";
                        underline_priv.BackgroundColor = Color.FromHex("5873FF");
                        underline_priv.TranslationX = 1000;
                        await Grid_messages.TranslateTo(1000, 0, 150);
                        if (toolBarBlocked)
                        {
                            SearchClicked(null, null);
                        }
                        if (!toolBarBlocked)
                        {
                            notesList.BindingContext = notCol.Notes;
                        }
                        await Grid_messages.TranslateTo(-1000, 0, 0);
                        await Grid_messages.TranslateTo(0, 0, 150);
                    }
                    break;
                case SwipeDirection.Left:
                    if (currentPage == "All")
                    {
                        underline_priv.TranslationX = 0;
                        underline_priv.BackgroundColor = Color.FromHex("BBFA8A");
                        currentPage = "Private";
                        underline_all.BackgroundColor = Color.FromHex("5873FF");
                        underline_all.TranslationX = 1000;
                        await Grid_messages.TranslateTo(-1000, 0, 150);
                        if (toolBarBlocked)
                        {
                            SearchClicked(null, null);
                        }
                        else
                            notCol.fillPrivateTemp(session.Email);
                        notesList.BindingContext = notCol.Notes_temp;
                        await Grid_messages.TranslateTo(1000, 0, 0);
                        await Grid_messages.TranslateTo(0, 0, 150);
                    }
                    else if(currentPage == "Private")
                    {
                        underline_group.TranslationX = 0;
                        underline_group.BackgroundColor = Color.FromHex("BBFA8A");
                        currentPage = "Group";
                        underline_priv.BackgroundColor = Color.FromHex("5873FF");
                        underline_priv.TranslationX = 1000;
                        await Grid_messages.TranslateTo(-1000, 0, 150);
                        if (toolBarBlocked)
                        {
                            SearchClicked(null, null);
                        }
                        else
                            notCol.fillGroupTemp(session.Email);
                        notesList.BindingContext = notCol.Notes_temp;
                        await Grid_messages.TranslateTo(1000, 0, 0);
                        await Grid_messages.TranslateTo(0, 0, 150);
                    }
                    break;
            }
        }
        private async void PrivateNotesClicked(object sender, EventArgs e)      //Переход на страницу с личными заметками
        {
            underline_priv.TranslationX = 0;
            underline_priv.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "All")
            {
                currentPage = "Private";
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                else
                    notCol.fillPrivateTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "Group")
            {
                currentPage = "Private";
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                else
                    notCol.fillPrivateTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
        }
        private async void AllNotesClicked(object sender, EventArgs e)      //Переход на страницу со всеми заметками
        {
            underline_all.TranslationX = 0;
            underline_all.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                currentPage = "All";
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                if (!toolBarBlocked)
                {
                    notesList.BindingContext = notCol.Notes;
                }
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "Group")
            {
                currentPage = "All";
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                if (!toolBarBlocked)
                {
                    notesList.BindingContext = notCol.Notes;
                }
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
        }
        private async void GroupNotesClicked(object sender, EventArgs e)      //Переход на страницу с групповыми заметками
        {
            underline_group.TranslationX = 0;
            underline_group.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                currentPage = "Group";
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                else
                notCol.fillGroupTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "All")
            {
                currentPage = "Group";
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                if (toolBarBlocked)
                {
                    SearchClicked(null, null);
                }
                else
                notCol.fillGroupTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
        }

        private void FindBtdClicked(object sender, EventArgs e)                //Функция инициализации поиска
        {
                toolbar_layout.TranslateTo(0, 250, 150);
                var rotateAnimation = new Animation(v => SearchEntry.HeightRequest = v, 0, 60);
                rotateAnimation.Commit(this, "HeightAnimation", 16, 500, Easing.SinIn, null, null);
                SearchToolbarLayout.TranslateTo(0, 0, 250);
                SearchToolbarLayoutArrow.TranslateTo(0, 0, 250);
                toolBarBlocked = true;
        }
        private void ShowBtnsClicked(object sender, EventArgs e) //Функция скрывания/показа кнопок удаления и изменения
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
            }
        }
        private void SearchBackClicked(object sender, EventArgs e)      //Функция сохранения изменений в заметке
        {var rotateAnimation = new Animation(v => SearchEntry.HeightRequest = v, 0, 60);
            notCol.Notes_temp.Clear();
            if (currentPage == "All")
            {
                notesList.BindingContext = notCol.Notes;
            }
            else if (currentPage == "Private")
            {
                notCol.fillPrivateTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
            }
            else if (currentPage == "Group")
            {
                notCol.fillGroupTemp(session.Email);
                notesList.BindingContext = notCol.Notes_temp;
            }
            var rotateAnimationBack = new Animation(v => SearchEntry.HeightRequest = v, 60, 0);
            rotateAnimationBack.Commit(this, "HeightAnimationBack", 16, 500, Easing.SinIn, null, null);
            toolBarBlocked = false;
            toolbar_layout.TranslateTo(0, 0, 150);
            SearchToolbarLayout.TranslateTo(100, 0, 250);
            SearchToolbarLayoutArrow.TranslateTo(100, 0, 250);
            hided = true;
            SearchEntry.Text = "";
        }
        private void SearchClicked(object sender, EventArgs e)      //Нажата кнопка поиска
        {       //Функция поиска заметки
            string find_str = SearchEntry.Text;
            if (find_str == null)
                find_str = "";
            if (currentPage == "All")
            {
                notCol.findAllByPart(find_str);
            }
            else if (currentPage == "Private")
            {
                notCol.findPrivateByPart(find_str,session.Email);
            }
            else if (currentPage == "Group")
            {
                notCol.findGroupByPart(find_str,session.Email);
            }
            notesList.BindingContext = notCol.Notes_temp;
        }
    }
}

