using Agrin.Helper.ComponentModel;
using Agrin.Helper.Threads;
using Agrin.ViewModels.WindowLayouts.Asuda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.WindowLayouts.Asuda
{
    /// <summary>
    /// Interaction logic for BasketReceiverWindow.xaml
    /// </summary>
    public partial class BasketReceiverWindow : Window
    {
        public BasketReceiverWindow()
        {
            ApplicationHelperBase.AddThreadDispather(this.Dispatcher);
            InitializeComponent();
            this.Top = this.Left = 0;
            Closing += BasketReceiverWindow_Closing;
            this.Deactivated += BasketReceiverWindow_Deactivated;
            this.LocationChanged += BasketReceiverWindow_LocationChanged;
            //autoLocation.Initialize(this, basketReceiverControl.dataPopUp);
        }



        //Agrin.ViewModels.ControlHelpers.PopupAutoLocationHelper autoLocation = new Agrin.ViewModels.ControlHelpers.PopupAutoLocationHelper();
        private void BasketReceiverWindow_Deactivated(object sender, EventArgs e)
        {
            Popup_PreviewMouseLeftButtonUp(null, null);
        }

        void BasketReceiverWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void BasketReceiverControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        static BasketReceiverWindow currentWindow = null;
        public static BasketReceiverPinnedWindow currentPinnedWindow = null;
         static BasketReceiverDataWindow currentDataWindow = null;

        static HorizontalAlignment lastPositionGridHorizontalAlignment;
        static VerticalAlignment lastPositionGridVerticalAlignment;

        static bool _IsPinned = false;
        public static bool IsPinned
        {
            get
            {
                return _IsPinned;
            }
            set
            {
                _IsPinned = value;
                var bvm = currentDataWindow.DataContext as BaseViewModels.WindowLayouts.Asuda.BasketReceiverBaseViewModel;
                bvm.IsShowList = false;
                Task task = new Task(() =>
                {
                    System.Threading.Thread.Sleep(250);
                    if (value)
                    {
                        currentWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            currentWindow.Hide();
                            currentDataWindow.Hide();
                        }));

                        currentPinnedWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lastPositionGridHorizontalAlignment = currentDataWindow.PositionGridHorizontalAlignment;
                            lastPositionGridVerticalAlignment = currentDataWindow.PositionGridVerticalAlignment;
                            //currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Right;
                            //currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Center;
                            //currentDataWindow.Top = currentPinnedWindow.Top - (currentDataWindow.Height / 2) + (currentPinnedWindow.Height / 2);
                            //currentDataWindow.Left = currentPinnedWindow.Left - currentDataWindow.Width + currentPinnedWindow.Width;
                            currentPinnedWindow.IsShow = false;
                            currentPinnedWindow.Show();
                            //Views.WindowLayouts.Asuda.AsudaPopup.ShowPopup(new Views.WindowLayouts.Asuda.AsudaPopupInfo() { Message = "لینک شما تکمیل شده است.", Mode = Views.WindowLayouts.Asuda.AsudaPopupTypeEnum.Complete });

                        }));
                    }
                    else
                    {

                        currentPinnedWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            currentPinnedWindow.Hide();
                            currentDataWindow.Hide();
                        }));

                        currentWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            currentDataWindow.PositionGridHorizontalAlignment = lastPositionGridHorizontalAlignment;
                            currentDataWindow.PositionGridVerticalAlignment = lastPositionGridVerticalAlignment;
                            currentWindow.canMove = true;
                            currentWindow.mustMoving = true;
                            currentWindow.Top = SystemParameters.WorkArea.Height / 2;
                            currentWindow.Left = SystemParameters.WorkArea.Width / 2;
                            currentWindow.ReDrawView(null);
                            currentWindow.Show();
                            currentDataWindow.Show();
                        }));
                    }
                });
                task.Start();
            }
        }

        public static void ShowDataWindowPinned()
        {
            currentWindow.ReDrawViewPinned();
        }

        public static void HideDataWindowPinned()
        {
            var bvm = currentDataWindow.DataContext as BaseViewModels.WindowLayouts.Asuda.BasketReceiverBaseViewModel;
            bvm.IsShowList = false;
        }

        public static void ShowBasket()
        {
            if (currentWindow != null)
            {
                if (IsPinned)
                {
                    currentPinnedWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        currentPinnedWindow.Show();
                    }));
                }
                else
                {
                    currentWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        currentWindow.Show();
                        currentDataWindow.Show();
                    }));
                }
                return;
            }

            System.Threading.Thread thread = null;
            thread = new System.Threading.Thread(() =>
            {
                try
                {
                    currentWindow = new BasketReceiverWindow();
                    currentDataWindow = new BasketReceiverDataWindow();
                    currentPinnedWindow = new BasketReceiverPinnedWindow();
                    currentPinnedWindow.Left = SystemParameters.WorkArea.Width - currentPinnedWindow.Width;
                    currentPinnedWindow.Top = (SystemParameters.WorkArea.Height + currentPinnedWindow.Height) / 2;

                    currentDataWindow.Show();
                    currentWindow.Show();

                    currentWindow.Owner = currentDataWindow;
                    currentPinnedWindow.Owner = currentDataWindow;

                    //currentDataWindow.Focusable = false;
                    //currentDataWindow.Activated += (s, e) =>
                    //{
                    //    currentDataWindow.Topmost = false;
                    //    currentDataWindow.Topmost = true;
                    //};
                    currentPinnedWindow.DataContext = currentDataWindow.DataContext = currentWindow.basketReceiverControl.DataContext;
                    currentDataWindow.Top = currentWindow.Top - 290 + 55;
                    currentDataWindow.Left = currentWindow.Left + 30;


                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "ShowBasket Dispatcher");
                }

            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

        }

        public static void HideBasket()
        {
            currentWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                currentPinnedWindow.Hide();
                currentWindow.Hide();
                currentDataWindow.Hide();
            }));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var ps = e.PreviousSize;
            //var ns = e.NewSize;
            //var delta = ns.Height - ps.Height;
            //Top -= delta;
        }

        bool canMove = false;
        bool mustMoving = false;
        byte oldMode = 0;
        bool canMoveBottomLeft = true;
        bool canMoveBottomRight = false;
        bool canMoveTopLeft = false;
        bool canMoveTopRight = false;
        ActionRunner lastRunner = null;
        ActionRunner lastPinnedRunner = null;
        bool lastIsShowList = false;
        bool canSetlastIsShowList = true;
        private void Popup_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
            {
                return;
            }
            ReDrawView(e);
        }

        public void ReDrawViewPinned()
        {
            try
            {
                //var newMousePosition = this.PointToScreen(e == null ? new Point(0, 0) : e.GetPosition(this));
                //var offset = newMousePosition - oldMousePosition;

                //var newPoint = e == null ? new Point(0, 0) : e.GetPosition(this);
                //if (newMousePosition.X < oldMousePosition.X - 5 || newMousePosition.X > oldMousePosition.X + 5)
                //{
                //    canMove = true;
                //}
                //else if (newMousePosition.Y < oldMousePosition.Y - 5 || newMousePosition.Y > oldMousePosition.Y + 5)
                //{
                //    canMove = true;
                //}

                //if (canMove)
                //{
                //oldMousePosition = newMousePosition;

                var bvm = currentDataWindow.DataContext as BaseViewModels.WindowLayouts.Asuda.BasketReceiverBaseViewModel;

                bvm.IsShowList = false;
                //if (lastPinnedRunner != null)
                //    lastPinnedRunner.Dispose();
                currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Right;
                currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Center;
                currentDataWindow.Top = currentPinnedWindow.Top - (currentDataWindow.Height / 2) + (currentPinnedWindow.Height / 2);
                currentDataWindow.Left = currentPinnedWindow.Left - currentDataWindow.Width + currentPinnedWindow.Width;
                bvm.IsShowList = true;
                //}
                currentDataWindow.Show();
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "ReDrawView Basket");
            }
        }

        public void ReDrawView(MouseEventArgs e)
        {
            try
            {
                var newMousePosition = this.PointToScreen(e == null ? new Point(0, 0) : e.GetPosition(this));
                var offset = newMousePosition - oldMousePosition;

                var newPoint = e == null ? new Point(0, 0) : e.GetPosition(this);
                if (newMousePosition.X < oldMousePosition.X - 5 || newMousePosition.X > oldMousePosition.X + 5)
                {
                    canMove = true;
                }
                else if (newMousePosition.Y < oldMousePosition.Y - 5 || newMousePosition.Y > oldMousePosition.Y + 5)
                {
                    canMove = true;
                }

                if (canMove)
                {
                    oldMousePosition = newMousePosition;

                    if (_NormalActionRunner != null && !mustMoving)
                        _NormalActionRunner.Run();
                    else if (mustMoving)
                    {
                        this.Left += offset.X;
                        this.Top += offset.Y;

                        var bvm = currentDataWindow.DataContext as BaseViewModels.WindowLayouts.Asuda.BasketReceiverBaseViewModel;
                        if (canSetlastIsShowList)
                        {
                            lastIsShowList = bvm.IsShowList;
                            canSetlastIsShowList = false;
                        }
                        bool skeep = !lastIsShowList;

                        if (isLeftOverflow && isBottomOverflow || oldMode == 0)
                        {
                            if (oldMode == 0)
                            {
                                currentDataWindow.Top = this.Top - 290 + 60;
                                currentDataWindow.Left = this.Left + 30;
                            }
                            else
                            {
                                if (!canMoveBottomLeft)
                                {
                                    canMoveBottomLeft = true;
                                    canMoveBottomRight = false;
                                    canMoveTopLeft = false;
                                    canMoveTopRight = false;

                                    bvm.IsShowList = false;
                                    if (lastRunner != null)
                                        lastRunner.Dispose();
                                    lastRunner = ActionRunner.Run(new TimeSpan(0, 0, 0, 0, 500), () =>
                                    {
                                        ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                        {
                                            oldMode = 0;
                                            currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Left;
                                            currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Bottom;
                                            bvm.ArcAlignMode = BaseViewModels.WindowLayouts.Asuda.AlignMode.BottomLeft;
                                            currentDataWindow.Top = this.Top - 290 + 60;
                                            currentDataWindow.Left = this.Left + 30;
                                            bvm.IsShowList = !skeep;
                                            canSetlastIsShowList = true;
                                        }, currentDataWindow.Dispatcher);
                                    }, () => canMoveBottomLeft = false, skeep);
                                }
                            }

                        }
                        if (isLeftOverflow && isTopOverflow || oldMode == 1)
                        {
                            if (oldMode == 1)
                            {
                                currentDataWindow.Top = this.Top + 30;
                                currentDataWindow.Left = this.Left + 45;
                            }
                            else
                            {
                                if (!canMoveTopLeft)
                                {
                                    canMoveTopLeft = true;
                                    canMoveBottomLeft = false;
                                    canMoveBottomRight = false;
                                    canMoveTopRight = false;

                                    bvm.IsShowList = false;
                                    if (lastRunner != null)
                                        lastRunner.Dispose();
                                    lastRunner = ActionRunner.Run(new TimeSpan(0, 0, 0, 0, 500), () =>
                                    {
                                        ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                        {
                                            oldMode = 1;
                                            currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Left;
                                            currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Top;
                                            bvm.ArcAlignMode = BaseViewModels.WindowLayouts.Asuda.AlignMode.TopLeft;
                                            currentDataWindow.Top = this.Top + 30;
                                            currentDataWindow.Left = this.Left + 45;
                                            bvm.IsShowList = !skeep;
                                            canSetlastIsShowList = true;
                                        }, currentDataWindow.Dispatcher);
                                    }, () => canMoveTopLeft = false, skeep);
                                }
                            }

                        }
                        if (isRightOverflow && isTopOverflow || oldMode == 2)
                        {
                            if (oldMode == 2)
                            {
                                currentDataWindow.Top = this.Top + 50;
                                currentDataWindow.Left = this.Left - 405;
                            }
                            else
                            {
                                if (!canMoveTopRight)
                                {
                                    canMoveTopRight = true;
                                    canMoveBottomLeft = false;
                                    canMoveBottomRight = false;
                                    canMoveTopLeft = false;

                                    bvm.IsShowList = false;
                                    if (lastRunner != null)
                                        lastRunner.Dispose();
                                    lastRunner = ActionRunner.Run(new TimeSpan(0, 0, 0, 0, 500), () =>
                                    {
                                        ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                        {
                                            oldMode = 2;
                                            currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Right;
                                            currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Top;
                                            bvm.ArcAlignMode = BaseViewModels.WindowLayouts.Asuda.AlignMode.TopRight;
                                            currentDataWindow.Top = this.Top + 50;
                                            currentDataWindow.Left = this.Left - 405;
                                            bvm.IsShowList = !skeep;
                                            canSetlastIsShowList = true;
                                        }, currentDataWindow.Dispatcher);
                                    }, () => canMoveTopRight = false, skeep);
                                }
                            }

                        }
                        if (isRightOverflow && isBottomOverflow || oldMode == 3)
                        {
                            if (oldMode == 3)
                            {
                                currentDataWindow.Top = this.Top - 290 + 60;
                                currentDataWindow.Left = this.Left - 420;
                            }
                            else
                            {
                                if (!canMoveBottomRight)
                                {
                                    canMoveBottomLeft = false;
                                    canMoveTopLeft = false;
                                    canMoveTopRight = false;
                                    canMoveBottomRight = true;
                                    bvm.IsShowList = false;
                                    if (lastRunner != null)
                                        lastRunner.Dispose();
                                    lastRunner = ActionRunner.Run(new TimeSpan(0, 0, 0, 0, 500), () =>
                                    {
                                        ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                        {
                                            oldMode = 3;
                                            currentDataWindow.PositionGridHorizontalAlignment = HorizontalAlignment.Right;
                                            currentDataWindow.PositionGridVerticalAlignment = VerticalAlignment.Bottom;
                                            bvm.ArcAlignMode = BaseViewModels.WindowLayouts.Asuda.AlignMode.BottomRight;
                                            currentDataWindow.Top = this.Top - 290 + 60;
                                            currentDataWindow.Left = this.Left - 420;
                                            bvm.IsShowList = !skeep;
                                            canSetlastIsShowList = true;
                                        }, currentDataWindow.Dispatcher);
                                    }, () => canMoveBottomRight = false, skeep);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AutoLogger.LogError(ex, "ReDrawView Basket");
            }
        }

        //double x = 0, y = 0;

        private void Popup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (_NormalActionRunner != null)
            {
                _NormalActionRunner.Dispose();
                _NormalActionRunner = null;
            }
            if (mustMoving)
            {
                //BasketReceiverWindow_LocationChanged(null, null);
                //ReDrawView(e);
                basketReceiverControl.btnData.ReleaseMouseCapture();
            }
            mustMoving = false;
            canMove = false;
            mouseDown = false;
        }

        bool mouseDown = false;
        Point oldMousePosition;
        NormalActionRunner _NormalActionRunner = null;
        private void Popup_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mustMoving = false;
            canMove = false;
            mouseDown = true;
            canSetlastIsShowList = true;
            if (_NormalActionRunner == null)
            {
                _NormalActionRunner = new NormalActionRunner(() =>
                {
                    basketReceiverControl.btnData.CaptureMouse();
                    this.Cursor = Cursors.ScrollAll;
                    mustMoving = true;
                });
            }

            oldMousePosition = this.PointToScreen(e.GetPosition(this));
        }

        bool isRightOverflow = false;
        bool isLeftOverflow = true;
        bool isTopOverflow = false;
        bool isBottomOverflow = true;

        private void BasketReceiverWindow_LocationChanged(object sender, EventArgs e)
        {
            var width = SystemParameters.WorkArea.Width;
            var height = SystemParameters.WorkArea.Height;



            if (this.Left < (!isLeftOverflow ? 420 : 0))
            {
                isLeftOverflow = true;
                isRightOverflow = false;
            }
            if (this.Left + (!isRightOverflow ? 500 : 0) > width)
            {
                isRightOverflow = true;
                isLeftOverflow = false;
            }


            if (this.Top + (!isBottomOverflow ? 280 : 0) > height)
            {
                isTopOverflow = false;
                isBottomOverflow = true;
            }

            if (this.Top < (!isTopOverflow ? 235 : 0))
            {
                isBottomOverflow = false;
                isTopOverflow = true;
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DragEnterData(e);
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DragOverData(e);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DropData(e.Data);
        }
    }
}
