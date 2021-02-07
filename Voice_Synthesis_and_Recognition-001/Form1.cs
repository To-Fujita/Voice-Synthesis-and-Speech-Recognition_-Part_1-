// Voice Synthesis & Recognition on 2021/01/25 by T. Fujita
// 参照でSystem.Speechを追加すること

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Speech;
using System.Speech.Synthesis;
using System.Globalization;

namespace Voice_Synthesis
{
    public partial class Form1 : Form
    {
        public static int D_Box_x = 400;                            // ダイアログボックスの表示位置X
        public static int D_Box_y = 100;                            // ダイアログボックスの表示位置Y
        public static int Speech_flag = 0;                          // 0: 日本語用、1: 英語用

        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            Dialog_Synthesis dialog1 = new Dialog_Synthesis();
            dialog1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    // 音声合成用ダイアログボックス
    public class Dialog_Synthesis : Form
    {
        public Label label_01;
        public Label label_02;
        public Label label_Speech;
        public TextBox textBox_01;
        public TextBox textBox_02;
        public Button Send_button;
        public Button Speech_button;
        public Button Close_button;
        public static string Pos;
        public static string Speech_Status;
        string Temp_Text;

        public Dialog_Synthesis()
        {
            // ダイアログボックスの設定
            this.MaximizeBox = false;                                   // 最大化ボタン
            this.MinimizeBox = false;                                   // 最小化ボタン
            this.ShowInTaskbar = true;                                  // タスクバー上に表示
            this.FormBorderStyle = FormBorderStyle.FixedDialog;         // 境界のスタイル
            this.StartPosition = FormStartPosition.Manual;              // 任意の位置に表示
            this.Size = new Size(300, 480);                             // ダイアログボックスのサイズ指定
            this.Location = new Point(Form1.D_Box_x, Form1.D_Box_y);    // ダイアログボックスの表示位置
            this.BackColor = Color.FromArgb(220, 220, 220);             // 背景色
            this.Text = "音声合成";                                     // タイトル

            label_01 = new Label()
            {
                Text = "キー入力用Box",
                Location = new Point(10, 15),
                Size = new Size(100, 25),
            };
            Send_button = new Button()
            {
                Text = "音声合成実行",
                Location = new Point(150, 5),
                Size = new Size(90, 25),
            };
            Send_button.Click += new EventHandler(Send_button_Clicked);
            textBox_01 = new TextBox()
            {
                Text = "",
                Location = new Point(20, 40),
                Size = new Size(250, 50),
                ReadOnly = false,
            };

            label_02 = new Label()
            {
                Text = "情報用Box",
                Location = new Point(10, 85),
                Size = new Size(100, 25),
            };
            if (Form1.Speech_flag == 0)
            {
                Temp_Text = "日本語で入力してください。\r\n英語は話しません。\r\n";
            }
            else
            {
                Temp_Text = "Please input in english.\r\nI don't speak Japanese.\r\n";
            }
            textBox_02 = new TextBox()
            {
                Text = Temp_Text,
                Location = new Point(20, 110),
                Size = new Size(250, 40),
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
            };

            if (Form1.Speech_flag == 0)
            {
                Speech_Status = "日本語";
            }
            else
            {
                Speech_Status = " 英語 ";
            }
            Speech_button = new Button()
            {
                Text = "日本語／英語の切替: ",
                Location = new Point(20, 290),
                Size = new Size(150, 25),
            };
            Speech_button.Click += new EventHandler(Speech_button_Clicked);
            label_Speech = new Label()
            {
                Text = Speech_Status,
                Location = new Point(180, 295),
                Size = new Size(100, 25),
            };

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(200, 360),
            };
            Close_button.Click += new EventHandler(Close_button_Clicked);

            this.Controls.AddRange(new Control[]
            {
                label_01, label_02, label_Speech,
                Send_button, Speech_button, Close_button,
                textBox_01, textBox_02
            });
        }

        // Speech_buttonが押された時の処理（音声合成用）
        void Speech_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.Speech_flag == 1)
            {
                Speech_Status = "日本語";
                Form1.Speech_flag = 0;
                Temp_Text = "日本語で入力してください。\r\n英語は話しません。\r\n";
            }
            else
            {
                Speech_Status = " 英語 ";
                Form1.Speech_flag = 1;
                Temp_Text = "Please input in english.\r\nI don't speak Japanese.\r\n";
            }
            textBox_02.Text = Temp_Text;
            label_Speech.Text = Speech_Status;
        }

        // 送信ボタンが押された時の処理
        void Send_button_Clicked(object sender, EventArgs e)
        {
            var words = textBox_01.Text;
            if (words != "")
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SetOutputToDefaultAudioDevice();
                if (Form1.Speech_flag == 0)
                {
                    synth.SelectVoice("Microsoft Haruka Desktop");
                } 
                else
                {
                    synth.SelectVoice("Microsoft Zira Desktop");
                }

                synth.Speak(words);                 // 音声を同期で生成
                // synth.SpeakAsync(answer);        // 音声を非同期で生成
            }
        }

        // クローズボタンが押された時の処理
        void Close_button_Clicked(object sender, EventArgs e)
        {
            Form1.D_Box_x = this.Left;
            Form1.D_Box_y = this.Top;
            this.Close();
        }

    }
}
