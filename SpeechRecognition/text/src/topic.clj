(ns topic
  (:require [goo :refer [text2words]]
            [clojure.string :refer [split]]))


(defn topic-likelihood [wl text]
  (let [words (text2words text "名詞")]
    (->> words
         (keep (fn [word]
                 (let [freq (get wl word)]
                   (when freq
                     {:word word :freq freq}))))
         (sort-by :freq <))))

;; (topic-likelihood (clojure.edn/read-string (slurp "twitter_newspaper.wl")) "隣の客はよく柿食う客だ")
