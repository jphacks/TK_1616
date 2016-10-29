(ns chat
  (:require [clojure.string :refer [split]]
            [topic :refer [topic-likelihood]]))



(defn preference-reply
  [text wl]
  (when (re-find #".+好き" text)
    (let [tl (topic-likelihood wl text)
          kword (:word (first tl))]
      (str kword "がお好きなんですね"))))
