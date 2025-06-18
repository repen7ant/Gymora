using Gymora.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gymora.ViewModels
{
    public class IntroScreenViewModel : BaseViewModel
    {
        #region Properties
        public ObservableCollection<IntroScreenModel> IntroScreens { get; set; } = new ObservableCollection<IntroScreenModel>();
        #endregion
        private string _buttonText = "Next";
        public string ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, value);
        }

        private int _position;
        public int Position
        {
            get => _position;
            set => SetProperty(ref _position, value, onChanged: (() =>
            {
                if (value == IntroScreens.Count - 1)
                {
                    ButtonText = "Start";
                }
                else
                {
                    ButtonText = "Next";
                }
            }));
        }

        public IntroScreenViewModel()
        {
            IntroScreens.Add(new IntroScreenModel
            {
                IntroTitle = "Персональный фитнес-план и контроль питания!",
                IntroDescription = "создавай индивидуальные программы",
                IntroDescription1 = "отслеживай прогресс",
                IntroDescription2 = "считай калории",
                IntroImage = "gymorais"
            });

            IntroScreens.Add(new IntroScreenModel
            {
                IntroTitle = "Друзья, мотивация и прогресс!",
                IntroDescription = "следи за прогрессом друзей",
                IntroDescription1 = "делись успехами",
                IntroDescription2 = "общайся",
                IntroImage = "gymorais"
            });

            IntroScreens.Add(new IntroScreenModel
            {
                IntroTitle = "Все залы в твоем телефоне!",
                IntroDescription = "находи лучшие варианты рядом",
                IntroDescription1 = "выбирай удобное время",
                IntroDescription2 = "сравнивай цены",
                IntroImage = "gymorais"
            });

        }
        public ICommand NextCommand => new Command(async () =>
        {
            if (Position >= IntroScreens.Count - 1)
            {
                await Shell.Current.GoToAsync($"///SignInSignUp");
            }
            else
            {
                Position += 1;
            }
        });

    }
}
