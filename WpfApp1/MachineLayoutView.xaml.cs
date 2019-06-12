using RAL.RealTime.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RAL.RealTime
{
    /// <summary>
    /// Interaction logic for MachineLayoutView.xaml
    /// </summary>
    public partial class MachineLayoutView : ReactiveUserControl<MachineLayoutViewModel>
    {
        public MachineLayoutView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(this.ViewModel,
                                    viewModel => viewModel.MachineStatuses,
                                    view => view.MachineStatuses.ItemsSource).WithDisposable(disposableRegistration);
                                    
            });
            
        }
    }
}
