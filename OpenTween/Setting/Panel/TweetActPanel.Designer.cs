﻿namespace OpenTween.Setting.Panel
{
    partial class TweetActPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetActPanel));
            this.CheckHashSupple = new System.Windows.Forms.CheckBox();
            this.CheckAtIdSupple = new System.Windows.Forms.CheckBox();
            this.ComboBoxPostKeySelect = new System.Windows.Forms.ComboBox();
            this.Label27 = new System.Windows.Forms.Label();
            this.CheckRetweetNoConfirm = new System.Windows.Forms.CheckBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.CheckUseRecommendStatus = new System.Windows.Forms.CheckBox();
            this.StatusText = new System.Windows.Forms.TextBox();
            this.CheckUseUnofficialPostMenus = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CheckHashSupple
            // 
            resources.ApplyResources(this.CheckHashSupple, "CheckHashSupple");
            this.CheckHashSupple.Name = "CheckHashSupple";
            this.CheckHashSupple.UseVisualStyleBackColor = true;
            // 
            // CheckAtIdSupple
            // 
            resources.ApplyResources(this.CheckAtIdSupple, "CheckAtIdSupple");
            this.CheckAtIdSupple.Name = "CheckAtIdSupple";
            this.CheckAtIdSupple.UseVisualStyleBackColor = true;
            // 
            // ComboBoxPostKeySelect
            // 
            this.ComboBoxPostKeySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPostKeySelect.FormattingEnabled = true;
            this.ComboBoxPostKeySelect.Items.AddRange(new object[] {
            resources.GetString("ComboBoxPostKeySelect.Items"),
            resources.GetString("ComboBoxPostKeySelect.Items1"),
            resources.GetString("ComboBoxPostKeySelect.Items2")});
            resources.ApplyResources(this.ComboBoxPostKeySelect, "ComboBoxPostKeySelect");
            this.ComboBoxPostKeySelect.Name = "ComboBoxPostKeySelect";
            // 
            // Label27
            // 
            resources.ApplyResources(this.Label27, "Label27");
            this.Label27.Name = "Label27";
            // 
            // CheckRetweetNoConfirm
            // 
            resources.ApplyResources(this.CheckRetweetNoConfirm, "CheckRetweetNoConfirm");
            this.CheckRetweetNoConfirm.Name = "CheckRetweetNoConfirm";
            this.CheckRetweetNoConfirm.UseVisualStyleBackColor = true;
            // 
            // Label12
            // 
            resources.ApplyResources(this.Label12, "Label12");
            this.Label12.Name = "Label12";
            // 
            // CheckUseRecommendStatus
            // 
            resources.ApplyResources(this.CheckUseRecommendStatus, "CheckUseRecommendStatus");
            this.CheckUseRecommendStatus.Name = "CheckUseRecommendStatus";
            this.CheckUseRecommendStatus.UseVisualStyleBackColor = true;
            this.CheckUseRecommendStatus.CheckedChanged += new System.EventHandler(this.CheckUseRecommendStatus_CheckedChanged);
            // 
            // StatusText
            // 
            resources.ApplyResources(this.StatusText, "StatusText");
            this.StatusText.Name = "StatusText";
            // 
            // CheckUseUnofficialPostMenus
            // 
            resources.ApplyResources(this.CheckUseUnofficialPostMenus, "CheckUseUnofficialPostMenus");
            this.CheckUseUnofficialPostMenus.Name = "CheckUseUnofficialPostMenus";
            this.CheckUseUnofficialPostMenus.UseVisualStyleBackColor = true;
            // 
            // TweetActPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.CheckUseUnofficialPostMenus);
            this.Controls.Add(this.CheckHashSupple);
            this.Controls.Add(this.CheckAtIdSupple);
            this.Controls.Add(this.ComboBoxPostKeySelect);
            this.Controls.Add(this.Label27);
            this.Controls.Add(this.CheckRetweetNoConfirm);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.CheckUseRecommendStatus);
            this.Controls.Add(this.StatusText);
            this.Name = "TweetActPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox CheckHashSupple;
        internal System.Windows.Forms.CheckBox CheckAtIdSupple;
        internal System.Windows.Forms.ComboBox ComboBoxPostKeySelect;
        internal System.Windows.Forms.Label Label27;
        internal System.Windows.Forms.CheckBox CheckRetweetNoConfirm;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.CheckBox CheckUseRecommendStatus;
        internal System.Windows.Forms.TextBox StatusText;
        internal System.Windows.Forms.CheckBox CheckUseUnofficialPostMenus;
    }
}
