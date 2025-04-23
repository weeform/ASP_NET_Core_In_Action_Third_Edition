using System;
using System.Drawing;
using System.Windows.Forms;

namespace HelloWorldDialog;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        textBox1 = new TextBox();
        button1 = new Button();
        SuspendLayout();
        //
        // textBox1
        //
        textBox1.Location = new Point(12, 23);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(260, 80);
        textBox1.TabIndex = 0;
        //
        // button1
        //
        button1.Location = new Point(12, 109);
        button1.Name = "button1";
        button1.Size = new Size(260, 29);
        button1.TabIndex = 1;
        button1.Text = "显示消息";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        //
        // Form1
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(284, 161);
        Controls.Add(button1);
        Controls.Add(textBox1);
        Name = "Form1";
        Text = "Hello World Dialog";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private Button button1;
}
