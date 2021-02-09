# Voice-Synthesis-and-Speech-Recognition
These are some programs based on visual studio c# for voice synthesis and speech recognition. In this time, it is recognized only Japanese. 
## 1. Description
　無料で使用可能な音声合成と音声認識プログラムをお届けします。　本プログラムは、Visual Studio 2019 の windowsフォームアプリケーション（.NET Framework）C# で作成しました。　音声合成／音声認識エンジンは「System.Speech」を形態素解析エンジンには「NMeCab」を使用しました。　さらに、ウィキペディアから情報を入手するため、
「LinqToWiki」を使用しております。
 
## 2. Details
　本プログラムは、３つのパートから構成されており、上記「Code」から一括ダウンロードできます。　ダウンロードしたファイルを解凍し、それぞれの「*.sln」を開いて「開始」ボタンでコンパイルすることで実行ファイルが作成されます。　以下にそれぞれのパートについて説明します。
### Part-1: 音声合成
「Voice_Synthesis_and_Recognition-001」フォルダーに対象プログラムが収容されています。　本プログラムは、シンプルに入力された文字を読み上げるだけですが、標準的な日本語用Windows10では、日本語と英語に対応しています。

### Part-2: 音声認識
「Voice_Synthesis_and_Recognition-002」フォルダーに対象プログラムが収容されています。　本プログラムは、上記音声合成に音声認識を追加したものです。　標準入力デバイスであるマイクから入力された音声を文字で表示します。　認識できる言語は日本語のみです。　残念ながら、まだ誤認識が多い様に見受けられます。

### Part-3: 音声合成と音声認識を使用した人口無脳との会話
「Voice_Synthesis_and_Recognition-003」フォルダーに対象プログラムが収容されています。　本プログラムは、音声合成と音声認識を利用して人工無脳と会話可能なものです。　人口無脳ですから、シナリオに沿った受け答えしかできませんが、「./bin/Debug/Data/Talk_List_001.csv」ファイルに会話データを追加することで、好みの会話ができます。　会話データは、「Talk_List_001.csv」をエクセルで開き、A列にあなたの言葉／質問をB列以降に人工無脳からの返答を数個記載することで、任意の返答がランダムに返されることになります。　また、「東京（県庁所在地名）の天気は？」と聞くことで天気予報を、「ニュースはありますか」や「今日の出来事は」と聞くとRSSニュースサイトから何かニュースを拾ってきてくれます。　さらに、「＊＊について教えて」や「＊＊を検索して」でウキペディアからサマリーを入手してくれます。　音声認識の誤認識と併せてお楽しみください。  
  
追記：　作成したプログラム（exe ファイル）を他のフォルダーに移動あるいはコピーした場合、「exe」ファイルを実行するとエラーが生じる場合があります。 
その場合、「char.bin」のパスが異なるのが原因と思われますので、エラーで表示されたフォルダーに「dic」フォルダー全体をコピーしてください。

## 3. Reference
[Visual Studio](https://visualstudio.microsoft.com/ja/)  
[System.Speech.Synthesis](https://docs.microsoft.com/ja-jp/dotnet/api/system.speech.synthesis?view=netframework-4.8)  
[SpeechRecognitionEngine](https://docs.microsoft.com/ja-jp/dotnet/api/system.speech.recognition.speechrecognitionengine?view=netframework-4.8)  
[MeCab: Yet Another Part-of-Speech and Morphological Analyzer](http://taku910.github.io/mecab/)  
[LinqToWiki](https://github.com/svick/LINQ-to-Wiki)  

## 4. License
MIT

## 5. Author
[T. Fujita](https://github.com/To-Fujita)
