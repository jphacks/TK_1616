(ns goo
  (:require [org.httpkit.client :as http]
            [clojure.data.json :as json]))


(defn text2morphs [text]
  (let [result @(http/post "https://labs.goo.ne.jp/api/morph"
                           {:keepalive 100000
                            :timeout 4000
                            :headers {"Content-Type" "application/json"}
                            :body (json/write-str  {"app_id" "c260f221d40c764f71810cf53fc563e93636ab56ec2c607c52b0c9c416af2079" "sentence" text})})]
    (-> result
        :body
        json/read-str
        (get "word_list")
        first)))


(defn text2words
  [text & [part]]
  (->> (text2morphs text)
       (filter #(if part (= (second %) part) identity))
       (map first)
       (remove #(some (fn[x] (= x %)) ["何" "なに" "好き"]))
       distinct))


;; (text2words "明日を走る" "名詞")
