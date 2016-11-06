(ns chat
  (:require [clojure.string :refer [split]]
            [goo :refer [text2words]]
            [polarity-estimation :refer [polarity-estimation]]
            [topic :refer [topic-likelihood]]))


(defn back-channnel?
  [text]
  (re-find #"そうなんだ|へー|ふーん|ほう" text))

(defn good-reply4disclosure?
  [text disclosure-kword]
  (let [coll (text2words text "名詞")]
    (some (fn[word] (some #(= % word) disclosure-kword)) coll)))

;; (good-reply4disclosure? "映画いいですよね" ["映" "映画"])

(defn ask?
  [text]
  (re-find #"なに|何|なぜ|何故|どうして|どこ|何処|いつ|どんな|どのような" text))



(defn preference-reply
  [text wl em dic]
  ;;   (when (re-find #".+好き" text)
  (let [tl (topic-likelihood wl text)
        kword (:word (first tl))
        t (str kword "ですか、" kword)
        p (:rating (first (polarity-estimation em dic kword)))
        point (condp = p  1 -10 2 -5 3 0 4 5 5 10)]
    (cond (or (= p 1) (= p 2))
          {:text (str t "は少し苦手です") :point point :kword kword :kword-point p}
          (or (= p 3) (= p 4) (= p 5))
          {:text (str t "はいいですよね") :point point :kword kword :kword-point p}
          (ask? text)
          {:text "どうでしょう" :point -5 :kword nil :kword-point 0}
          t
          {:text (str t "についてはよく知りません") :point -5 :kword nil :kword-point 0}
          :else
          {:text "よくわかりませんでした☆" :point -5 :kword nil :kword-point 0})))

(defn disclosure-reply
  [text wl]
  ;;   (when
  (let [disclosure-word? (re-find #"映画|料理" text)
        tl (topic-likelihood wl text)
        kword (:word (first tl))]
    (cond disclosure-word?
          {:text "そうですね" :point 10 :action "action1"}
          (back-channnel?  text)
          {:text "..." :point -10 :action "action2"}
          (ask? text)
          {:text "どうでしょう" :point -10 :action "action3"}
          kword
          {:text "はい" :point -10 :action "action4"}
          :else
          {:text "よくわかりませんでした☆" :point -5 :action "action4"})))

;; (disclosure-reply "野球はいいね" {"映画" 2 "野球" 1})




(defn hobby-reply
  [text wl em dic]
  (let [tl (topic-likelihood wl text)
        kword (:word (first tl))
        t (str kword "ですか、" kword)
        p (:rating (first (polarity-estimation em dic kword)))
        point (condp =  p 1 -10 2 -5 3 0 4 5 5 10)]
    (println p)
    (cond (or (= p 1) (= p 2))
          {:text (str t "にはあまりいい印象がないです") :point point :kword kword :kword-point p}
          (or (= p 3) (= p 4) (= p 5))
          {:text (str t "にはいい印象があります") :point point :kword kword :kword-point p}
          :else
          {:text (str t "についてはよく知りません") :point -5 :kword kword :kword-point p})))


(defn ask-cousine-reply
  [text]
  (let [words (text2words text "名詞")]
    (cond (and (re-find #"日本" text) (ask? text))
          {:text "秘密です" :point 0}
          (re-find #"日本" text)
          {:text "そうですね" :point 0}
          (some #(contains? #{"イタリアン" "イタリア" "イタリア料理" "フランス料理" "フランス" "ロシア" "ロシアン" "ロシア料理"} %) words)
          {:text "その料理は好きです" :point 0}
          (some #(contains? #{"中華" "中国" "韓国" "ベトナム" "イギリス" "アメリカ"} %) words)
          {:text "あまり食べないです" :point 0}
          (back-channnel? text)
          {:text "はい" :point -5}
          :else
          (if (ask? text) {:text "秘密です" :point 0} {:text "そうですか" :point 0}))))


(defn ask-dinner [text sum-point]
  (if (nil? sum-point)
    {:text "no supplied value for sum-point"}
    (let [words (text2words text "名詞")
          bonus (if (some #(contains? #{"イタリアン" "イタリア" "イタリア料理" "フランス料理" "フランス" "ロシア" "ロシアン" "ロシア料理"} %) words) 10 0)]
      (if (>= (+ (Integer/parseInt sum-point) bonus) 80)
        {:text "いいですね、是非"    :point 10}
        {:text "また今度お願いします" :point 0}))))
;; (food-utt "お寿司")


(defn script-chat
  [reply-key text wl em dic & [sum-point]]
  (condp = reply-key
    :preference
    (preference-reply text wl em dic)
    :disclosure
    (disclosure-reply text wl)
    :hobby
    (hobby-reply text wl em dic)
    :ask-cousine
    (ask-cousine-reply text)
    :ask-dinner
    (ask-dinner text sum-point)))

