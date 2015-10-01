using System;
using System.Collections.Generic;
using System.ComponentModel;
using Archimedes.Localisation;

namespace Archimedes.Patterns.WPF.ViewModels
{
    public abstract class ChooseItemDialogeBase : DialogViewModel
    {
        private readonly ICollectionView _itemPresenter;


        protected ChooseItemDialogeBase(ICollectionView itemPresenter)
        {
            if (itemPresenter == null)
                throw new ArgumentNullException("itemPresenter");
            _itemPresenter = itemPresenter;
        }

        /// <summary>
        /// Items which are chooseable
        /// </summary>
        public ICollectionView Items {
            get { return _itemPresenter; }
        }

        protected override IEnumerable<DialogCommand> BuildCommands()
        {
           yield return BuildDefaultCommand(Translator.GetTranslation("dialog.abort"), DialogResultType.Cancel, false, true);

           var chooseCommand = BuildDefaultCommand(Translator.GetTranslation("dialog.choose"), DialogResultType.Affirmative, true);
           chooseCommand.CustomAction = o => ChooseSelectedItem();
           chooseCommand.CustomCanExecute = o => CanChooseSelectedItem;
           yield return chooseCommand;
        }


        protected abstract void ChooseSelectedItem();
        protected abstract bool CanChooseSelectedItem { get; }
    }
}
