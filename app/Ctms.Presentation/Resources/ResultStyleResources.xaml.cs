using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Ctms.Applications.DataModels;

namespace Ctms.Presentation.Resources
{
    public partial class ResultStyleResources
    {
        private void ResultWrapper_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement scatterViewItem = e.Source as FrameworkElement;
            ResultDataModel result = scatterViewItem.DataContext as ResultDataModel;

            if (e.PreviousSize.Height == 0.0 && e.PreviousSize.Width == 0.0)
            {
                result.StdWidth = e.NewSize.Width;
                result.StdHeight = e.NewSize.Height;
                result.Width = e.NewSize.Width;
                result.Height = e.NewSize.Height;
            }

            /*
            if (e.PreviousSize.Height != 0.0 && e.PreviousSize.Width != 0.0)
            {
                if (e.NewSize.Width == result.StdWidth || e.NewSize.Height == result.StdHeight)
                {

                }
                else
                {
                    //not initial rendering
                    if (e.NewSize.Width > e.PreviousSize.Width || e.NewSize.Height > e.PreviousSize.Height)
                    {
                        //item is getting bigger
                        if (e.NewSize.Width > result.StdWidth * 2 || e.NewSize.Height > result.StdHeight * 2)
                        {
                            result.IsDetail = true;
                            scatterViewItem.Height = 300.0;
                            scatterViewItem.Width = 400.0;
                        }
                    }
                    else
                    {
                        //item is getting smaller
                        if (e.NewSize.Width < result.StdWidth * 2 || e.NewSize.Height < result.StdHeight * 2)
                        {
                            result.IsDetail = false;
                        }
                    }
                }
            }
            else
            {
                //initial rendering | save items standard width/height
                result.StdWidth = e.NewSize.Width;
                result.StdHeight = e.NewSize.Height;
                result.Width = e.NewSize.Width;
                result.Height = e.NewSize.Height;
            }
            */
        }
    }
}
