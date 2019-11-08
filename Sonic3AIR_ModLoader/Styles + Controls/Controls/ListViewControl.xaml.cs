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

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for ListViewControl.xaml
    /// </summary>
    public partial class ListViewControl : UserControl
    {
        public ListViewControl()
        {
            InitializeComponent();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;           
        }
    }

    public class AutoSizedGridView : GridView
    {

        protected override void PrepareItem(ListViewItem item)
        {
            foreach (GridViewColumn column in Columns)
            {
                // Setting NaN for the column width automatically determines the required
                // width enough to hold the content completely.

                // If the width is NaN, first set it to ActualWidth temporarily.
                if (double.IsNaN(column.Width))
                    column.Width = column.ActualWidth;

                // Finally, set the column with to NaN. This raises the property change
                // event and re computes the width.
                column.Width = double.NaN;
            }
            base.PrepareItem(item);
        }
    }
}
