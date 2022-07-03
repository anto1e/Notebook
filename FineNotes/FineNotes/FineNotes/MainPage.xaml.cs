using System;
using System.Collections.Generic;
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
        public MainPage()
        {
            InitializeComponent();
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
                if (previousScrollPosition < e.ScrollY)
                {
                if (scrollXCnt > 10)
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
                if (scrollYCnt > 5)
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

