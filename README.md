# コミトレ(コミュニケーショントレーナー)
## 製品概要
### Edu-Tech?

### 背景（製品開発のきっかけ、課題等）

```
少子化が深刻な問題となっている。大きな原因として未婚率の継続的な上昇がある。
2010年の総務省「国勢調査」によると、男性では、25～29歳で71.8％、30～34歳で47.3％、35歳～39歳で35.6％、女性では、25～29歳で60.3％、30～34歳で34.5％、35～39歳で23.1％が未婚となっている。
また、リクルートが行った「恋愛観調査2014」によれば、一度も異性と付き合ったことのない20代男性は41.6%で、その中の66.3%は恋人が欲しいと考えている。
恋人が欲しい人は少なくないにもかかわらず付き合えない理由として、異性とのコミュニケーションに苦手意識を持っていることが挙げられる。
異性とのコミュニケーションに対する苦手意識は練習を行うことで解決できることが期待されるが、現実の女性に対して練習をすることは現実的ではない。
従って、異性との初対面時におけるコミュニケーションを模したトレーニングシステムを開発した。
```

### 製品説明（具体的な製品の説明）
### 特長
1. 特長1 初対面時コミュニケーション
2. 特長2 没入感による緊張
3. 特長3 コミュニケーションのフィードバック機能(レポート)

### 解決出来ること
- 現実的には用意することが困難な場面の提供
- 初対面時における異性感でのコミュニケーションの苦手意識の克服

### 今後の展望
- より詳細なフィードバック

### 注力したこと（こだわり等）

- Oculus(VR HMD)を用いた没入感/緊張感のあるコミュニケーション・トレーニングシステム
- 単語の極性評価/適した単語を活用して自己紹介を行えているかの評価に使用
- Line botを活用したトレーニングのフィードバック

## 開発技術
### 活用した外部技術
#### API・データ
* SkyWay SpeechRecognition [[使用したコード](https://github.com/jphacks/TK_1616/blob/master/SpeechRecognition/sr.html)]
* goo/形態素解析 [[使用したコード](https://github.com/jphacks/TK_1616/blob/master/text/src/goo.clj)]
* Line Messaging API [[使用したコード](https://github.com/jphacks/TK_1616/blob/master/LineChatBot/app.py)]
* VoiceText Web API

#### フレームワーク・ライブラリ・モジュール
* word2vec(実装は自作)
* OVRLipSync

#### デバイス
* Oculus Rift

### 独自技術
#### 期間中に開発した独自機能・技術
* 単語分散表現からの初対面時における使用単語の極性推定 [[コード](https://github.com/jphacks/TK_1616/blob/master/text/src/polarity_estimation.clj), [学習データ](https://github.com/jphacks/TK_1616/blob/master/text/polarity.csv), [Demo](https://github.com/jphacks/TK_1616/blob/master/text/README.md)]
* トレーニングで活用する対話管理部[[コード](https://github.com/jphacks/TK_1616/blob/master/text/src/chat.clj) ]
* websocketを活用し、任意のタイミングで音声認識を開始/終了できるように [[コード](https://github.com/jphacks/TK_1616/blob/master/SpeechRecognition/app.js)]


#### 研究内容（任意）
* もし、製品に研究内容を用いた場合は、研究内容の詳細及び具体的な活用法について、こちらに記載をしてください。
