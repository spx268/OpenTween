// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Thumbnail;
using System.Threading;

namespace OpenTween
{
    public partial class TweetThumbnail : UserControl
    {
        protected internal List<OTPictureBox> pictureBox = new List<OTPictureBox>();

        private Task task = null;
        private CancellationTokenSource cancelTokenSource;

        public event EventHandler ThumbnailLoading;
        public event EventHandler ThumbnailLoadCompleted;
        public event EventHandler<ThumbnailDoubleClickEventArgs> ThumbnailDoubleClick;
        public event EventHandler<ThumbnailImageSearchEventArgs> ThumbnailImageSearchClick;

        private object uiLockObj = new object();

        public ThumbnailInfo Thumbnail
        {
            get { return this.pictureBox[this.scrollBar.Value].Tag as ThumbnailInfo; }
        }

        private Point? popupMouseDownPos = null;

        public TweetThumbnail()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(32, 32, 32);
        }

        public Task ShowThumbnailAsync(PostClass post)
        {
            this.CancelAsync();

            this.scrollBar.Enabled = false;

            this.cancelTokenSource = new CancellationTokenSource();
            var cancelToken = this.cancelTokenSource.Token;

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            this.task = Task.Factory.StartNew(() => this.GetThumbailInfo(post), cancelToken, TaskCreationOptions.None, TaskScheduler.Default)
                .ContinueWith(t => /* await使いたい */
                {
                    var thumbnails = t.Result;

                    lock (this.uiLockObj)
                    {
                        this.SetThumbnailCount(thumbnails.Count);
                        if (thumbnails.Count == 0) return;

                        for (int i = 0; i < thumbnails.Count; i++)
                        {
                            var thumb = thumbnails[i];
                            var picbox = this.pictureBox[i];

                            picbox.Tag = thumb;
                            picbox.ContextMenu = CreateContextMenu(thumb);

                            picbox.ShowInitialImage();

                            thumb.LoadThumbnailImageAsync(cancelToken)
                                .ContinueWith(t2 =>
                                {
                                    if (t2.IsFaulted)
                                        t2.Exception.Flatten().Handle(x => x is WebException || x is InvalidImageException || x is TaskCanceledException);

                                    if (t2.IsFaulted || t2.IsCanceled)
                                    {
                                        picbox.ShowErrorImage();
                                        return;
                                    }

                                    picbox.Image = t2.Result;

                                    picbox.MouseDown += this.pictureBox_MouseDown;
                                    picbox.MouseUp += this.pictureBox_MouseUp;
                                    picbox.MouseMove += this.pictureBox_MouseMove;
                                },
                                CancellationToken.None, TaskContinuationOptions.AttachedToParent, uiScheduler)
                                .ContinueWith(t2 =>
                                {
                                    if (picbox.Image != null)  // 画像読み込みの成否をチェック
                                    {
                                        if (this.ThumbnailLoadCompleted != null)
                                            this.ThumbnailLoadCompleted(picbox, new EventArgs());
                                    }
                                },
                                cancelToken, TaskContinuationOptions.OnlyOnRanToCompletion, uiScheduler);

                            var tooltipText = thumb.TooltipText;
                            if (!string.IsNullOrEmpty(tooltipText))
                            {
                                this.toolTip.SetToolTip(picbox, tooltipText);
                            }

                            cancelToken.ThrowIfCancellationRequested();
                        }

                        if (thumbnails.Count > 1)
                            this.scrollBar.Enabled = true;
                    }

                    if (this.ThumbnailLoading != null)
                        this.ThumbnailLoading(this, EventArgs.Empty);
                },
                cancelToken,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                uiScheduler);

            return this.task;
        }

        private ContextMenu CreateContextMenu(ThumbnailInfo thumb)
        {
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(CreateImageSearchMenuItem(thumb));
            return contextMenu;
        }

        private MenuItem CreateImageSearchMenuItem(ThumbnailInfo thumb)
        {
            var item = new MenuItem();
            item.Text = Properties.Resources.SearchSimilarImageText;
            var search_targe_url = thumb.FullSizeImageUrl ?? thumb.ThumbnailUrl ?? null;

            if (search_targe_url != null)
            {
                item.Click += (sender, e) =>
                {
                    string uri = GetImageSearchUri(search_targe_url);
                    if (this.ThumbnailImageSearchClick != null)
                    {
                        this.ThumbnailImageSearchClick(this, new ThumbnailImageSearchEventArgs(uri));
                    }
                };
            }
            else
            {
                item.Enabled = false;
            }

            return item;
        }

        private string GetImageSearchUri(string image_uri)
        {
            return @"https://www.google.com/searchbyimage?image_url=" + Uri.EscapeDataString(image_uri);
        }

        protected virtual List<ThumbnailInfo> GetThumbailInfo(PostClass post)
        {
            return ThumbnailGenerator.GetThumbnails(post);
        }

        public void CancelAsync()
        {
            if (this.task == null || this.task.IsCompleted) return;

            this.cancelTokenSource.Cancel();
        }

        /// <summary>
        /// 表示するサムネイルの数を設定する
        /// </summary>
        /// <param name="count">表示するサムネイルの数</param>
        protected void SetThumbnailCount(int count)
        {
            this.SuspendLayout();

            this.panelPictureBox.Controls.Clear();
            foreach (var picbox in this.pictureBox)
            {
                var memoryImage = picbox.Image;
                picbox.ContextMenu.Dispose();
                picbox.Dispose();

                if (memoryImage != null)
                    memoryImage.Dispose();
            }
            this.pictureBox.Clear();

            this.scrollBar.Maximum = (count > 0) ? count - 1 : 0;
            this.scrollBar.Value = 0;

            for (int i = 0; i < count; i++)
            {
                var picbox = CreatePictureBox("pictureBox" + i);
                picbox.Visible = (i == 0);
                picbox.DoubleClick += this.pictureBox_DoubleClick;

                this.panelPictureBox.Controls.Add(picbox);
                this.pictureBox.Add(picbox);
            }

            this.ResumeLayout(false);
        }

        protected virtual OTPictureBox CreatePictureBox(string name)
        {
            return new OTPictureBox()
            {
                Name = name,
                SizeMode = PictureBoxSizeMode.Zoom,
                WaitOnLoad = false,
                Dock = DockStyle.Fill,
            };
        }

        public void ScrollUp()
        {
            var newval = this.scrollBar.Value + this.scrollBar.SmallChange;

            if (newval > this.scrollBar.Maximum)
                newval = this.scrollBar.Maximum;

            this.scrollBar.Value = newval;
        }

        public void ScrollDown()
        {
            var newval = this.scrollBar.Value - this.scrollBar.SmallChange;

            if (newval < this.scrollBar.Minimum)
                newval = this.scrollBar.Minimum;

            this.scrollBar.Value = newval;
        }

        private void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();

            var value = this.scrollBar.Value;
            for (var i = 0; i < this.pictureBox.Count; i++)
            {
                this.pictureBox[i].Visible = (i == value);
            }

            this.ResumeLayout(false);
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            var thumb = ((PictureBox)sender).Tag as ThumbnailInfo;

            if (thumb == null) return;

            if (this.ThumbnailDoubleClick != null)
            {
                this.ThumbnailDoubleClick(this, new ThumbnailDoubleClickEventArgs(thumb));
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & ThumbnailWindow.Trigger) == ThumbnailWindow.Trigger)
            {
                this.popupMouseDownPos = e.Location;
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & ThumbnailWindow.Trigger) == ThumbnailWindow.Trigger)
            {
                this.popupMouseDownPos = null;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & ThumbnailWindow.Trigger) == ThumbnailWindow.Trigger &&
                this.popupMouseDownPos.HasValue)
            {
                // ダブルクリック時の些細な移動で反応するのを阻止
                if (Math.Abs(e.X - this.popupMouseDownPos.Value.X) < 3 ||
                    Math.Abs(e.Y - this.popupMouseDownPos.Value.Y) < 3) return;

                var originPos = this.popupMouseDownPos.Value;
                this.popupMouseDownPos = null;

                try
                {
                    ThumbnailWindow.Show((OTPictureBox)sender, originPos);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public class ThumbnailDoubleClickEventArgs : EventArgs
    {
        public ThumbnailInfo Thumbnail { get; private set; }

        public ThumbnailDoubleClickEventArgs(ThumbnailInfo thumbnail)
        {
            this.Thumbnail = thumbnail;
        }
    }

    public class ThumbnailImageSearchEventArgs : EventArgs
    {
        public string ImageUrl { get; private set; }

        public ThumbnailImageSearchEventArgs(string url)
        {
            this.ImageUrl = url;
        }
    }

    public class ThumbnailWindow : Form
    {
        public static MouseButtons Trigger = MouseButtons.Left;

        private readonly Rectangle startupBounds;
        private readonly Point originPos;

        private bool disposed = false;

        protected ThumbnailWindow(MemoryImage thumbnail, Rectangle startupBounds, Point originPos)
        {
            this.startupBounds = startupBounds;
            this.originPos = originPos;

            this.SuspendLayout();

            this.DoubleBuffered = true;

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = startupBounds;

            this.Capture = true;

            this.MouseMove += ThumbnailWindow_MouseMove;
            this.MouseUp += ThumbnailWindow_MouseUp;

            var picbox = new OTPictureBox()
            {
                Name = "picbox",
                Image = thumbnail,
                SizeMode = PictureBoxSizeMode.Zoom,
                WaitOnLoad = false,
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(picbox);

            picbox.MouseMove += ThumbnailWindow_MouseMove;
            picbox.MouseUp += ThumbnailWindow_MouseUp;

            this.ResumeLayout(false);
        }

        private void ThumbnailWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & Trigger) == Trigger)
            {
                var movepos = this.PointToScreen(e.Location);
                var xmove = (int)((movepos.X - this.originPos.X) * 1.5);
                var ymove = (int)((movepos.Y - this.originPos.Y) * 1.5);

                var left = this.startupBounds.X + ((xmove <= 0) ? xmove : 0);
                var width = this.startupBounds.Width + xmove * ((xmove <= 0) ? -1 : 1);
                if (left < SystemInformation.VirtualScreen.Left)  // upper-left 方向へは仮想スクリーン座標で制限
                {
                    width += left - SystemInformation.VirtualScreen.Left;
                    left = SystemInformation.VirtualScreen.Left;
                }

                var top = this.startupBounds.Y + ((ymove <= 0) ? ymove : 0);
                var height = this.startupBounds.Height + ymove * ((ymove <= 0) ? -1 : 1);
                if (top < SystemInformation.VirtualScreen.Top)  // upper-left 方向へは仮想スクリーン座標で制限
                {
                    height += top - SystemInformation.VirtualScreen.Top;
                    top = SystemInformation.VirtualScreen.Top;
                }

                this.SetBounds(left, top, width, height);
            }
        }

        private void ThumbnailWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & Trigger) == Trigger)
            {
                this.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed) return;

            try
            {
                if (disposing)
                {
                    var picbox = (OTPictureBox)this.Controls["picbox"];
                    this.Controls.Remove(picbox);
                    picbox.Dispose();
                }

                this.disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public static void Show(OTPictureBox picbox, Point originPos)
        {
            if (picbox == null || picbox.Image == null)
                throw new ArgumentNullException("picbox");

            if (picbox.IsDisposed)
                throw new ObjectDisposedException("picbox");

            // ポップアップ表示中は、サムネイル枠の画像を乗っ取ってダミーとすり替えておく
            var thumbnail = picbox.Image;
            picbox.Image = null;

            var popup = new ThumbnailWindow(thumbnail, picbox.RectangleToScreen(picbox.Bounds), picbox.PointToScreen(originPos))
            {
                BackColor = picbox.BackColor,
            };
            popup.Disposed += (s, e) =>
            {
                // 乗っ取った画像は、サムネイル枠に戻せるなら戻し、戻せないなら破棄する
                if (!picbox.IsDisposed)
                    picbox.Image = thumbnail;
                else
                    thumbnail.Dispose();
            };

            popup.Show();
            picbox.Update();
        }
    }
}
