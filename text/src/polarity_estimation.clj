(ns polarity-estimation
  (:require [clojure.string :refer [split]]
            [taoensso.nippy :refer [thaw-from-in!]]
            [goo :refer [text2words]]))

(defn load-model [target-path]
  (with-open [w (clojure.java.io/input-stream target-path)]
    (thaw-from-in! (java.io.DataInputStream. w))))

(defn times
  ([v1 v2]
   (when-not (= (count v1) (count v2)) (throw (Exception. "vectors must be same length")))
   (let [n (count v1)
         ret (float-array n)
         _ (dotimes [x n] (aset ^floats ret x (float (* (aget ^floats v1 x) (aget ^floats v2 x)))))]
     ret))
  ([v1 v2 & more]
   (reduce #(times %1 %2) (times v1 v2) more)))

(defn dot [v1 v2]
  (let [s (times v1 v2)]
    (areduce ^floats s i ret (float 0) (+ ret (aget ^floats s i)))))

(defn word2vec [embedding word]
  (get embedding word))

(defn similarity
  "cosine similarity between 2 words or vectors
  word vectors are assumed to be l2 normalized vectors"
  ([v1 v2]
   (float (dot v1 v2)))
  ([em a b]
   (let [v1 (if (string? a) (word2vec em a) a)
         v2 (if (string? b) (word2vec em b) b)]
     (similarity v1 v2))))

(defn polarity-estimation
  "dic likes [[\"best\" 5] [\"worst\" 1]]"
  [em dic target-word]
  (let [em (if (string? em) (load-model em) em)
        vdic (vec dic)]
    (->> vdic
         (mapv (fn [[word rating]]
                 {:similarity (similarity em target-word word)
                  :word word
                  :rating (Integer/parseInt rating)}))
         (sort-by :similarity >))))


;; (vec {"word1" 5 "word2" 1})

(defn load-polarity-file [dic-path]
  (let [ratings (split (slurp dic-path) #"\r\n|\n|\r")]
    (->> ratings
         (mapv #(split % #",")))))


(defn polarity-estimation-from-text
  [em dic text]
  (->> (text2words text "名詞")
       (filter #(get em %))
       (mapv (fn[word]
               (let [it (first (polarity-estimation em dic word))]
                 (assoc it :word word :most-sim-word (:word it)))))))





;; (polarity-from-utt nil "隣の客はよく柿食う客だ")

