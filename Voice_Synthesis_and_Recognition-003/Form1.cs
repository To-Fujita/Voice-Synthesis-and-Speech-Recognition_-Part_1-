// Voice Synthesis and Recongnition on 2021/02/04 by T. Fujita
// NuGetでNMeCabとLinqToWikiをインストールし、参照でSystem.Speechを追加すること

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Diagnostics;

using NMeCab;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Globalization;
using LinqToWiki.Generated;
using LinqToWiki;

namespace Voice_Synthesis_and_Recognition
{
    public partial class Form1 : Form
    {
        public static int D_Box_x = 400;                                // ダイアログボックスの表示位置X
        public static int D_Box_y = 100;                                // ダイアログボックスの表示位置Y
        public static int Speech_flag = 0;                              // 0: 音声合成 OFF, 1: 音声合成 ON
        public static int Talk_flag = 0;                                // 0: 音声認識実行前, 1: 音声認識結果表示 ON, -1: 音声認識結果表示 OFF
        public static int Wiki_flag = 0;                                // 0: Wiki使用せず, 1: Wikiを使用
        public static string Talk_url = "./Data/Talk_List_001.csv";     // URL for Talk Data
        public static SpeechRecognitionEngine engine;                   // 音声認識用エンジンの登録
        public static Wiki wiki = new Wiki("LinqToWiki.Samples", "https://ja.wikipedia.org", "/w/api.php");

        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            Dialog_Voice_SynRec dialog1 = new Dialog_Voice_SynRec();
            dialog1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    // 音声合成・認識用ダイアログボックス
    public class Dialog_Voice_SynRec : Form
    {
        public Label label_01;
        public Label label_02;
        public Label label_Speech;
        public Label label_Talk;
        public Label label_Wiki;
        public Label label_Info;
        public TextBox textBox_01;
        public TextBox textBox_02;
        public Button Keitaiso_button;
        public Button Send_button;
        public Button Speech_button;
        public Button Talk_button;
        public Button Reset_button;
        public Button Wiki_button;
        public Button Close_button;
        public static string sSource;
        public static MeCabNode mcNode;
        public static string news_Text;
        public static string news_Source;
        public static string weather_Text;
        public static string Pos = "東京";
        public static string Speech_Status;
        public static string Talk_Status;
        public static string Wiki_Status;

        public Dialog_Voice_SynRec()
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
            this.Text = "形態素解析と会話";                             // タイトル

            label_01 = new Label()
            {
                Text = "入力用Box",
                Location = new Point(10, 15),
                Size = new Size(70, 25),
            };
            Send_button = new Button()
            {
                Text = "入力情報送信",
                Location = new Point(90, 5),
                Size = new Size(90, 25),
            };
            Send_button.Click += new EventHandler(Send_button_Clicked);

            Keitaiso_button = new Button()
            {
                Text = "形態素解析",
                Location = new Point(190, 5),
                Size = new Size(80, 25),
            };
            Keitaiso_button.Click += new EventHandler(Keitaiso_button_Clicked);

            textBox_01 = new TextBox()
            {
                Text = "",
                Location = new Point(20, 40),
                Size = new Size(250, 50),
                ReadOnly = false,
            };

            label_02 = new Label()
            {
                Text = "出力用Box",
                Location = new Point(10, 75),
                Size = new Size(100, 25),
            };
            textBox_02 = new TextBox()
            {
                Text = "私：　何か入力して下さい。\r\n",
                Location = new Point(20, 100),
                Size = new Size(250, 150),
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
            };

            if (Form1.Speech_flag == 0)
            {
                Speech_Status = " OFF";
            }
            else
            {
                Speech_Status = " ON ";
            }
            Speech_button = new Button()
            {
                Text = "音声合成 ON/OFF: ",
                Location = new Point(20, 270),
                Size = new Size(150, 25),
            };
            Speech_button.Click += new EventHandler(Speech_button_Clicked);
            label_Speech = new Label()
            {
                Text = Speech_Status,
                Location = new Point(180, 275),
                Size = new Size(100, 25),
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

            if (Form1.Wiki_flag == 0)
            {
                Wiki_Status = " OFF";
            }
            else
            {
                Wiki_Status = " ON ";
            }
            Wiki_button = new Button()
            {
                Text = "Wiki検索 ON/OFF",
                Location = new Point(20, 360),
                Size = new Size(150, 25),
            };
            label_Wiki = new Label()
            {
                Text = Wiki_Status,
                Location = new Point(180, 365),
                Size = new Size(100, 25),
            };
            Wiki_button.Click += new EventHandler(Wiki_button_Clicked);

            label_Info = new Label()
            {
                Text = "Wiki検索をONにするとデータによって、返答に時間が掛かることがあります。",
                Location = new Point(10, 390),
                Size = new Size(190, 50),
            };

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(200, 400),
            };
            Close_button.Click += new EventHandler(Close_button_Clicked);

            this.Controls.AddRange(new Control[]
            {
                label_01, label_02, label_Speech, label_Talk, label_Wiki, label_Info,
                Send_button, Keitaiso_button, Speech_button, Talk_button, Reset_button, Wiki_button, Close_button,
                textBox_01, textBox_02
            });
        }

        // Send_buttonが押された時の処理
        void Send_button_Clicked(object sender, EventArgs e)
        {
            var words = textBox_01.Text;
            if (words == "") { words = "　"; }
            int textBox_02_Lines = textBox_02.Lines.Length;
            if (textBox_02_Lines > 50) { textBox_02.Text = ""; }
            Recog(words);
        }

        // 形態素解析ボタンが押された時の処理
        void Keitaiso_button_Clicked(object sender, EventArgs e)
        {
            sSource = textBox_01.Text;
            string sResult = "";
            MeCabParam mcParam = new MeCabParam();
            mcParam.DicDir = @"..\..\dic\ipadic\";
            MeCabTagger mcTagger = MeCabTagger.Create(mcParam);
            mcNode = mcTagger.ParseToNode(sSource);
            while (mcNode != null)
            {
                if (mcNode.CharType > 0)
                {
                    sResult = sResult + mcNode.Surface + "\t" + mcNode.Feature + "\r\n";

                }
                mcNode = mcNode.Next;
            }
            textBox_02.Text = sResult;
        }

        // Speech_buttonが押された時の処理（音声合成用）
        void Speech_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.Speech_flag == 1)
            {
                Form1.Speech_flag = 0;
                Speech_Status = " OFF ";
            }
            else
            {
                Form1.Speech_flag = 1;
                Speech_Status = " ON  ";
            }
            label_Speech.Text = Speech_Status;
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

        // Wiki buttonが押された時の処理
        void Wiki_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.Wiki_flag == 0)
            {
                Wiki_Status = "ON ";
                Form1.Wiki_flag = 1;
            }
            else
            {
                Wiki_Status = "OFF";
                Form1.Wiki_flag = 0;
            }
            label_Wiki.Text = Wiki_Status;
        }

        // クローズボタンが押された時の処理
        void Close_button_Clicked(object sender, EventArgs e)
        {
            Form1.D_Box_x = this.Left;
            Form1.D_Box_y = this.Top;
            Talk_Status = " OFF ";
            label_Talk.Text = Talk_Status;
            SpeechDispose();
            this.Close();
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
                int textBox_02_Lines = textBox_02.Lines.Length;
                if (textBox_02_Lines > 50) { textBox_02.Text = ""; }
                Recog(words);
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

        // MeCabによる形態素解析と返答作成
        void Recog(string words)
        {
            var mcTagger = NMeCab.MeCabTagger.Create();
            var db = new QADatabase(@Form1.Talk_url, mcTagger);
            var sentence = ToArray(mcTagger.ParseToNode(words));
            string frontWord = "";
            string backWord = "";
            string predWord = "";
            string wikiWord = "";
            sSource = words;
            weather_Text = "";

            MecabResult mr = new MecabResult();
            foreach (var tmp in mr.nodes.Where(n => n.品詞 != "*"))
            {
                if (tmp.品詞 == "名詞")
                {
                    frontWord = tmp.表層形;
                    if (tmp.品詞細分類1 == "固有名詞" && tmp.品詞細分類2 == "地域")
                    {
                        Pos = tmp.表層形;
                    }
                }
                if (predWord == "")
                {
                    if (tmp.品詞 == "名詞")
                    {
                        predWord = tmp.表層形;
                        wikiWord = tmp.表層形;
                    }
                    else if (tmp.品詞 == "動詞")
                    {
                        predWord = tmp.表層形;
                        wikiWord = tmp.表層形;
                    }
                    else if (tmp.品詞 == "形容詞")
                    {
                        predWord = tmp.表層形;
                        wikiWord = tmp.表層形;
                    }
                    else if (tmp.品詞 == "副詞")
                    {
                        predWord = tmp.表層形;
                        wikiWord = tmp.表層形;
                    }
                    else if (tmp.品詞 == "助詞")
                    {
                        predWord = tmp.表層形;
                    }
                    else
                    {
                        predWord = "取りあえず何か";
                    }
                }
            }

            for (int i = 0; i < sentence.Length; i++)
            {
                if (sentence[i] == "は" || sentence[i] == "が" || sentence[i] == "に" || sentence[i] == "を" || sentence[i] == "の" || sentence[i] == "から")
                {
                    if (i > 0)
                    {
                        frontWord = sentence[i - 1];
                    }
                    if (i < sentence.Length - 1)
                    {
                        backWord = sentence[i + 1];
                    }
                    else
                    {
                        backWord = predWord;
                    }
                }
                if (sentence[i] == "ました" || sentence[i] == "ます" || sentence[i] == "だ" || sentence[i] == "だった" || sentence[i] == "でした")
                {
                    if (i > 0)
                    {
                        frontWord = sentence[i - 1];
                    }
                    if (i >= sentence.Length - 1)
                    {
                        frontWord = predWord;
                    }
                }
                if (sentence[i] == "ニュース" || sentence[i] == "出来事" || sentence[i] == "話題")
                {
                    RSS_Receive();
                }
                if (sentence[i] == "天気" || sentence[i] == "天気予報")
                {
                    Weather_Receive(Pos);
                }
            }
             if (frontWord == "私" || frontWord == "わたし" || frontWord == "わたくし" || frontWord == "僕" || frontWord == "ぼく" || frontWord == "ボク")
            {
                frontWord = "あなた";
            }
            if (frontWord == "あなた" || frontWord == "貴方" || frontWord == "貴女" || frontWord == "君" || frontWord == "きみ" || frontWord == "あんた")
            {
                frontWord = "私";
            }

            string answer = db.NearestAnswer(sentence);
            if (frontWord != "")
            {
                answer = answer.Replace("#F#", frontWord);
            }
            else
            {
                answer = answer.Replace("#F#", "取りあえず何か");
            }
            if (backWord != "")
            {
                answer = answer.Replace("#B#", backWord);
            }
            else
            {
                answer = answer.Replace("#B#", "取りあえず何か");
            }
            if (news_Source != "")
            {
                answer = answer.Replace("#NEWS#", news_Text);
                answer = answer.Replace("#SOURCE#", news_Source);
            }
            else
            {
                answer = "\r\n特にニュースはありません！\r\n";
            }
            if (weather_Text != "")
            {
                answer = answer.Replace("#WEATHER#", weather_Text);
                answer = answer + "\r\n　　以上です。";
            }
            if (answer.IndexOf("#WIKI#") >= 0 )
            {
                if (wikiWord != "")
                {
                    predWord = wiki_Received(wikiWord);
                    answer = "ウィキペディアによりますと\r\n" + answer.Replace("#WIKI#", predWord);
                    answer = answer + "\r\n　　以上です。";
                }
                else
                {
                    answer = "良く分かりませんわ！";
                }               
            }

            textBox_02.Text = textBox_02.Text + "\r\nあなた：　" + sSource + "\r\n私：　" + answer + "\r\n";
            textBox_02.SelectionStart = textBox_02.Text.Length;
            textBox_02.ScrollToCaret();
            textBox_01.Text = "";
            if (Form1.Speech_flag == 1)
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SetOutputToDefaultAudioDevice();
                synth.Speak(answer);                 // 音声を同期で生成
                // synth.SpeakAsync(answer);         // 音声を非同期で生成
            }
        }

        // MeCabによる形態素解析（単語に分解）
        public static string[] ToArray(MeCabNode mcNode)
        {
            var words = new List<string>();
            while(mcNode != null)
            {
                if(mcNode.Stat != MeCabNodeStat.Bos && mcNode.Stat != MeCabNodeStat.Eos)
                {
                    words.Add(mcNode.Surface);
                }
                mcNode = mcNode.Next;
            }
            return words.ToArray();
        }

        // データベースから返答抽出
        public class QADatabase
        {
            List<string[]> question;
            List<string> answer;
            public QADatabase(string filename, MeCabTagger mcTagger)
            {
                question = new List<string[]>();
                answer = new List<string>();
                using(var infile = new StreamReader(filename, System.Text.Encoding.GetEncoding("Shift_JIS")))
                {
                    while(infile.Peek() != -1)
                    {
                        var x = infile.ReadLine().Split(',');
                        int Temp_count = 0;
                        for (int i = 0; i < x.Length; i++)
                        {
                            if (x[i] != "")
                            {
                                Temp_count = Temp_count + 1;
                            }
                        }
                        question.Add(ToArray(mcTagger.ParseToNode(x[0])));
                        Random rnd = new System.Random();
                        int temp = rnd.Next(1, Temp_count);
                        answer.Add(x[temp]);
                    }
                    infile.Close();
                }
            }

            // 似た質問を探す
            static double similarity(string[] x, string[] y)
            {
                var words = new HashSet<string>();
                foreach(string s in x)
                {
                    words.Add(s);
                }
                int count = 0;
                foreach(string s in y)
                {
                    if(words.Contains(s)) { count++; }
                }
                return (double)count / (Math.Sqrt((double)x.Length * y.Length));
            }
            public string NearestAnswer(string[] q)
            {
                double maxsim = -1;
                int maxind = -1;
                for (int i = 0; i < question.Count; i++)
                {
                    double sim = similarity(question[i], q);
                    if(sim > maxsim)
                    {
                        maxsim = sim;
                        maxind = i;
                    }
                }
                return answer[maxind];
            }
        }

        // MeCabによる形態素解析結果
        public class MecabResult
        {
            public List<MecabResultItem> nodes { get; set; }           
            public MecabResult()
            {
                MeCabParam mcParam = new MeCabParam();
                mcParam.DicDir = @"..\..\dic\ipadic\";
                MeCabTagger mcTagger = MeCabTagger.Create(mcParam);
                MeCabNode pnode = mcTagger.ParseToNode(sSource);
                nodes = new List<MecabResultItem>();
                int itempos = 0;
                while (pnode != null)
                {
                    MecabResultItem addItem = new MecabResultItem();
                    if (pnode.CharType > 0)
                    {
                        addItem.表層形 = pnode.Surface;
                        string[] tmpStrs = pnode.Feature.Split(',');
                        if (tmpStrs.Length <= 9)
                        {
                            try
                            {
                                addItem.品詞 = tmpStrs[0];
                                addItem.品詞細分類1 = tmpStrs[1];
                                addItem.品詞細分類2 = tmpStrs[2];
                                addItem.品詞細分類3 = tmpStrs[3];
                                addItem.活用形 = tmpStrs[4];
                                addItem.活用型 = tmpStrs[5];
                                addItem.原形 = tmpStrs[6];
                                addItem.読み = tmpStrs[7];
                                addItem.発音 = tmpStrs[8];
                            }
                            catch
                            {
                                return;
                            }
                        }
                        addItem.位置 = itempos;
                        nodes.Add(addItem);
                    }
                    itempos++;
                    pnode = pnode.Next;
                }
            }
            public partial class MecabResultItem
            {
                public string 表層形 { get; set; }
                public string 品詞 { get; set; }
                public string 品詞細分類1 { get; set; }
                public string 品詞細分類2 { get; set; }
                public string 品詞細分類3 { get; set; }
                public string 活用形 { get; set; }
                public string 活用型 { get; set; }
                public string 原形 { get; set; }
                public string 読み { get; set; }
                public string 発音 { get; set; }
                public int 位置 { get; set; }
            }
        }

        // ニュース取得
        void RSS_Receive()
        {
            news_Text = "";
            string[,] RSS_url = {
                {"http://www3.nhk.or.jp/rss/news/cat0.xml", "NHKニュース"},
                {"https://www.lifehacker.jp/index.xml", "ライフハッカー" },
                {"https://rss.itmedia.co.jp/rss/2.0/netlab.xml", "ねとらぼ"},
                {"https://rocketnews24.com/feed/", "ロケットニュース24"},
                {"https://gori.me/feed", "gori.me" },
                {"https://togetter.com/rss/hot", "Togetterまとめ"},
                {"https://gigazine.net/news/rss_2.0/", "GIGAZINE"},
                {"http://www.sanspo.com/rss/chumoku/news/allentertainments-n.xml", "サンスポ"}
            };
            int NewsCount = 0;
            Random rnd_0 = new System.Random();
            int RSS_No = rnd_0.Next(0, RSS_url.Length / 2);
            Random rnd_1 = new System.Random();
            int temp = 2;                                   // この番号まで、Title + Descriptionを表示
            news_Source = RSS_url[RSS_No, 1];

            try
            {
                XElement element = XElement.Load(RSS_url[RSS_No, 0]);
                XElement channelElement = element.Element("channel");
                IEnumerable<XElement> elementItems = channelElement.Elements("item");
                Random rnd_2 = new System.Random();
                NewsCount = rnd_2.Next(0, 3);
                XElement item = elementItems.ElementAt(NewsCount);
                if (RSS_No > temp)
                {
                    news_Text = "\r\n" + item.Element("title").Value + "\r\n";
                }
                else
                {
                    news_Text = "\r\n" + item.Element("title").Value + "\r\n" + item.Element("description").Value + "\r\n";
                }
            }
            catch (Exception)
            {
                news_Text = "\r\n特にニュースはありません！\r\n";
            }
        }

        // 天気予報データ取得
        void Weather_Receive(string Pos)
        {
            weather_Text = "";
            string url = "";

            if (Pos == "北海道" || Pos == "札幌")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/1400.xml";
            }
            if (Pos == "青森")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/3110.xml";
            }
            if (Pos == "盛岡" || Pos == "岩手")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/3310.xml";
            }
            if (Pos == "秋田")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/3210.xml";
            }
            if (Pos == "山形")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/3510.xml";
            }
            if (Pos == "宮城" || Pos == "仙台")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/3410.xml";
            }
            if (Pos == "東京")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4410.xml";
            }
            if (Pos == "神奈川" || Pos == "横浜")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4610.xml";
            }
            if (Pos == "さいたま" || Pos == "埼玉")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4310.xml";
            }
            if (Pos == "千葉")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4510.xml";
            }
            if (Pos == "茨城" || Pos == "水戸")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4010.xml";
            }
            if (Pos == "栃木" || Pos == "宇都宮")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4110.xml";
            }
            if (Pos == "群馬" || Pos == "前橋")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4210.xml";
            }
            if (Pos == "山梨" || Pos == "甲府")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4910.xml";
            }
            if (Pos == "新潟")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5410.xml";
            }
            if (Pos == "長野")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/4810.xml";
            }
            if (Pos == "富山")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5510.xml";
            }
            if (Pos == "石川" || Pos == "金沢")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5610.xml";
            }
            if (Pos == "福井")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5710.xml";
            }
            if (Pos == "愛知" || Pos == "名古屋")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5110.xml";
            }
            if (Pos == "岐阜")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5210.xml";
            }
            if (Pos == "静岡")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5010.xml";
            }
            if (Pos == "三重" || Pos == "津")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/5310.xml";
            }
            if (Pos == "大阪")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6200.xml";
            }
            if (Pos == "兵庫" || Pos == "神戸")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6310.xml";
            }
            if (Pos == "京都")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6110.xml";
            }
            if (Pos == "滋賀" || Pos == "大津")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6010.xml";
            }
            if (Pos == "奈良")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6410.xml";
            }
            if (Pos == "和歌山")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6510.xml";
            }
            if (Pos == "鳥取")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6910.xml";
            }
            if (Pos == "島根" || Pos == "松江")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6810.xml";
            }
            if (Pos == "岡山")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6610.xml";
            }
            if (Pos == "広島")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/6710.xml";
            }
            if (Pos == "山口")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8120.xml";
            }
            if (Pos == "徳島")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/7110.xml";
            }
            if (Pos == "香川" || Pos == "高松")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/7200.xml";
            }
            if (Pos == "愛媛" || Pos == "松山")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/7310.xml";
            }
            if (Pos == "高知")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/7410.xml";
            }
            if (Pos == "福岡" || Pos == "博多")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8210.xml";
            }
            if (Pos == "佐賀")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8510.xml";
            }
            if (Pos == "長崎")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8410.xml";
            }
            if (Pos == "熊本")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8610.xml";
            }
            if (Pos == "大分")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8310.xml";
            }
            if (Pos == "宮崎")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8710.xml";
            }
            if (Pos == "鹿児島")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/8810.xml";
            }
            if (Pos == "沖縄" || Pos == "那覇")
            {
                url = "https://rss-weather.yahoo.co.jp/rss/days/9110.xml";
            }

            if (url == "")
            {
                weather_Text = "\r\n県名あるいは県庁所在地等のみが対象です。\r\n";
                return;
            }

            try
            {
                XElement element = XElement.Load(url);
                XElement channelElement = element.Element("channel");
                IEnumerable<XElement> elementItems = channelElement.Elements("item");
                for (int i = 0; i < 3; i++)
                {
                    XElement item = elementItems.ElementAt(i);
                    weather_Text = weather_Text + item.Element("title").Value + "\r\n";
                }
            }
            catch (Exception)
            {
                weather_Text = "\r\n天気の取得に失敗しました！\r\n";
            }
        }

        // Wikipediaから情報入手
        private string wiki_Received(string inputData)
        {
            string outputData;
            if (Form1.Wiki_flag == 0)
            {
                outputData = "Wiki 検索が ON になっていません。";
                return outputData;
            }
            PagesSource<Page> pages = Form1.wiki.Query.allpages()
                                           .Where(X => X.prefix == inputData)
                                           .Pages;

            IEnumerable<string> articles = pages.Select(
                                           page =>
                                           page.revisions()
                                               .Select(r => r.value)
                                               .FirstOrDefault()
                                           ).ToEnumerable();
            outputData = articles.FirstOrDefault();

            if (outputData != null)
            {
                if (outputData.Substring(0, 9) == "#REDIRECT")
                {
                    outputData = outputData.Replace("#REDIRECT", "");
                    outputData = outputData.Replace("[", "");
                    outputData = outputData.Replace("]", "");
                    outputData = wiki_Received(outputData);
                }
                outputData = forming(outputData);
            }
            else
            {
                outputData = "良く分からないみたいです。";
            }
            return outputData;
        }

        // ウィキペディアから入手したデータの成型
        private string forming(string F_temp_Text)
        {
            int count = 0;
            string F_temp = "";
            string S1_temp = "";
            string S2_temp = "";

            count = F_temp_Text.IndexOf("==");
            if (count > 0)
            {
                F_temp = F_temp_Text.Substring(0, count);
            }
            else
            {
                F_temp = F_temp_Text;
            }
            F_temp_Text = F_temp;

            S1_temp = "{{";
            S2_temp = "}}";
            F_temp_Text = reduce_Back(F_temp_Text, S1_temp, S2_temp);

            S1_temp = "<";
            S2_temp = ">";
            F_temp_Text = reduce_Back(F_temp_Text, S1_temp, S2_temp);

            S1_temp = "（";
            S2_temp = "）";
            F_temp_Text = reduce_Back(F_temp_Text, S1_temp, S2_temp);

            S1_temp = "(";
            S2_temp = ")";
            F_temp_Text = reduce_Back(F_temp_Text, S1_temp, S2_temp);

            F_temp_Text = F_temp_Text.Replace("'","");

            S1_temp = "http";
            S2_temp = " ";
            F_temp_Text = reduce_Front(F_temp_Text, S1_temp, S2_temp);

            return F_temp_Text;
        }

        private string reduce_Back(string R_temp_Text, string R1_temp, string R2_temp)
        {
            int count = 0;
            int R_count = R1_temp.Length;
            int str_Count = 0;
            int end_Count = 0;
            string temp_Text;

            while (str_Count >= 0 && end_Count >= 0)
            {
                count = R_temp_Text.Length;
                end_Count = R_temp_Text.IndexOf(R2_temp);
                if (end_Count > 0)
                {
                    str_Count = R_temp_Text.LastIndexOf(R1_temp, end_Count);
                }
                else
                {
                    str_Count = R_temp_Text.IndexOf(R1_temp);
                }
                if (str_Count >= 0 && end_Count > 0 && count > 0)
                {
                    if (str_Count == 0)
                    {
                        temp_Text = R_temp_Text.Substring((end_Count + R_count), (count - (end_Count + R_count)));
                    }
                    else
                    {
                        temp_Text = R_temp_Text.Substring(0, str_Count) + R_temp_Text.Substring((end_Count + R_count), (count - (end_Count + R_count)));
                    }
                    R_temp_Text = temp_Text;
                }
            }
            return R_temp_Text;
        }

        private string reduce_Front(string R_temp_Text, string R1_temp, string R2_temp)
        {
            int count = 0;
            int R1_count = R1_temp.Length;
            int R2_count = R2_temp.Length;
            int str_Count = 0;
            int end_Count = 0;
            string temp_Text;

            while (str_Count >= 0 && end_Count >= 0)
            {
                count = R_temp_Text.Length;
                str_Count = R_temp_Text.IndexOf(R1_temp);
                if (str_Count >= 0)
                {
                    end_Count = R_temp_Text.IndexOf(R2_temp, str_Count);
                    if (str_Count == 0)
                    {
                        temp_Text = R_temp_Text.Substring((end_Count + R2_count), (count - (end_Count + R2_count)));
                        R_temp_Text = temp_Text;
                    }
                    else if (str_Count > 0)
                    {
                        temp_Text = R_temp_Text.Substring(0, str_Count) + R_temp_Text.Substring((end_Count + R2_count), (count - (end_Count + R2_count)));
                        R_temp_Text = temp_Text;
                    }
                }
            }
            return R_temp_Text;
        }
    }
}

