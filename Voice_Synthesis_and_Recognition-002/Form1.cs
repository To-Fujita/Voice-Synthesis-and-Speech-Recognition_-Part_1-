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
using System.Speech.Recognition;
using System.Globalization;

namespace Voice_Synthesis_and_Recongnition
{
    public partial class Form1 : Form
    {
        public static int D_Box_x = 400;                        // ダイアログボックスの表示位置X
        public static int D_Box_y = 100;                        // ダイアログボックスの表示位置Y
        public static int Talk_flag = 0;                        // 0: 音声認識実行前, 1: 音声認識結果表示 ON, -1: 音声認識結果表示 OFF
        public static SpeechRecognitionEngine engine;           // 音声認識用エンジンの登録

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

    // 音声合成・認識用ダイアログボックス
    public class Dialog_Synthesis : Form
    {
        public Label label_01;
        public Label label_02;
        public Label label_03;
        public Label label_Talk;
        public TextBox textBox_01;
        public TextBox textBox_02;
        public TextBox textBox_03;
        public Button Send_button;
        public Button Talk_button;
        public Button Reset_button;
        public Button Close_button;
        public static string sSource;
        public static string Pos;
        public static string Talk_Status;

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
            this.Text = "音声合成・認識";                               // タイトル

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
                Location = new Point(10, 80),
                Size = new Size(100, 25),
            };
            textBox_02 = new TextBox()
            {
                Text = "日本語で入力してください。\r\n英語は話しません。\r\n",
                Location = new Point(20, 105),
                Size = new Size(250, 40),
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
            };

            label_03 = new Label()
            {
                Text = "音声認識用Box",
                Location = new Point(10, 155),
                Size = new Size(100, 25),
            };
            textBox_03 = new TextBox()
            {
                Text = "",
                Location = new Point(20, 180),
                Size = new Size(250, 100),
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
            };

            if (Form1.Talk_flag <= 0)
            {
                Talk_Status = " OFF";
            }
            else
            {
                Talk_Status = " ON ";
            }
            Talk_button = new Button()
            {
                Text = "音声認識 ON/OFF: ",
                Location = new Point(20, 300),
                Size = new Size(150, 25),
            };
            Talk_button.Click += new EventHandler(Talk_button_Clicked);
            label_Talk = new Label()
            {
                Text = Talk_Status,
                Location = new Point(180, 305),
                Size = new Size(100, 25),
            };

            Reset_button = new Button()
            {
                Text = "音声認識 Reset: ",
                Location = new Point(20, 330),
                Size = new Size(150, 25),
            };
            Reset_button.Click += new EventHandler(Reset_button_Clicked);

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(200, 360),
            };
            Close_button.Click += new EventHandler(Close_button_Clicked);

            this.Controls.AddRange(new Control[]
            {
                label_01, label_02, label_03, label_Talk,
                Send_button, Talk_button, Reset_button, Close_button,
                textBox_01, textBox_02, textBox_03
            });
        }

        // Send_buttonが押された時の処理
        void Send_button_Clicked(object sender, EventArgs e)
        {
            var words = textBox_01.Text;
            if (words != "")
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SetOutputToDefaultAudioDevice();
                synth.SelectVoice("Microsoft Haruka Desktop");      // 日本語用
                // synth.SelectVoice("Microsoft Zira Desktop");     // 英語用

                synth.Speak(words);                                 // 音声を同期で生成
                // synth.SpeakAsync(answer);                        // 音声を非同期で生成
            }
        }

        // Talk_buttonが押された時の処理（音声認識用）
        void Talk_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.Talk_flag == 1)
            {
                Form1.Talk_flag = -1;
                Talk_Status = " OFF ";
            }
            else
            {
                if (Form1.Talk_flag == 0)
                {
                    Talk_Execute();
                }
                Form1.Talk_flag = 1;
                Talk_Status = " ON  ";
                textBox_02.Text = "音声認識は、日本語のみです。\r\n";
            }
            label_Talk.Text = Talk_Status;
        }

        // Reset buttonが押された時の処理（音声認識用）
        void Reset_button_Clicked(object sender, EventArgs e)
        {
            Talk_Status = " OFF ";
            label_Talk.Text = Talk_Status;
            SpeechDispose();
        }

        // 音声認識用ルーチン
        public void Talk_Execute()
        {
            Form1.engine = new SpeechRecognitionEngine(new CultureInfo("ja-JP"));             
            Form1.engine.SetInputToDefaultAudioDevice();
            Form1.engine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
            Form1.engine.LoadGrammarAsync(new DictationGrammar());
            Form1.engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Form1.Talk_flag == 1)
            {
                var words = e.Result.Text;
                int textBox_03_Lines = textBox_03.Lines.Length;
                if (textBox_03_Lines > 50) { textBox_03.Text = ""; }
                textBox_03.Text = textBox_03.Text + words + "\r\n";
                textBox_03.SelectionStart = textBox_03.Text.Length;
                textBox_03.ScrollToCaret();
            }
        }

        public void SpeechDispose()
        {
            if (Form1.Talk_flag != 0)
            {
                Form1.engine.Dispose();
                Form1.Talk_flag = 0;
            }
        }

        // クローズボタンが押された時の処理
        void Close_button_Clicked(object sender, EventArgs e)
        {
            Talk_Status = " OFF ";
            label_Talk.Text = Talk_Status;
            SpeechDispose();
            Form1.D_Box_x = this.Left;
            Form1.D_Box_y = this.Top;
            this.Close();
        }
    }
}
