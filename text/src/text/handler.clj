(ns text.handler
  (:require [compojure.core :refer :all]
            [compojure.route :as route]
            [compojure.handler :as handler]
            [org.httpkit.server :refer [run-server]]
            [ring.middleware.reload :refer [wrap-reload]]
            [ring.middleware.defaults :refer [wrap-defaults site-defaults]]
            [environ.core :refer [env]]
            [clojure.data.json :as json]
            [clojure.java.io :refer (reader)]
            [goo :refer [text2morphs text2words]]
            [polarity-estimation :refer [load-model load-polarity-file polarity-estimation polarity-estimation-from-text]]
            [topic :refer [topic-likelihood]]
            [chat :refer [preference-reply script-chat]]
            [feedback :refer [save-feedback]]))


(def wl-path "twitter_newspaper.wl")
(println (str "Loading " wl-path " ..."))
(def wl (clojure.edn/read-string  (slurp wl-path)))
(println "Done")
(def em-path "twitter_newspaper_200h_top100000.embedding")
(println (str  "Loading " em-path " ..."))
(def em (load-model em-path))
(println "Done")
(def polarity-path "polarity.csv")
(println (str "Loading " polarity-path " ..."))
(def polarity-dic (load-polarity-file polarity-path))
(println "done")


(defroutes app-routes
  (GET "/" [] "Hello World")
  (GET "/em" {params :params}
       (if-let [it (get em (:word params))]
         (apply str it)
         "no result"))
  (GET "/wl" {params :params}
       (if-let [it (get wl (:word params))]
         (apply str it)
         "no result"))
  (GET "/topic" {params :params}
       (apply str (topic-likelihood wl (:text params))))
  (GET "/text2morphs" {params :params} (apply str (interpose ", "(text2morphs (:text params)))))
  (GET "/text2words"  {params :params} (apply str (interpose ", "(text2words (:text params) "名詞"))))
  (GET "/polarity-estimation" {params :params}
       (let [result (polarity-estimation em (load-polarity-file "polarity.csv") (:word params))]
         (if (= "true" (:debug params))
           (apply str (interpose "<BR>" result))
           (json/write-str result))))
  (GET "/polarity-estimation-from-text" {params :params}
       (let [result (polarity-estimation-from-text em polarity-dic (:text params))]
         (if (= "true" (:debug params))
           (apply str (interpose "<BR>" result))
           (json/write-str result))))
  (GET "/preference-reply" {params :params}
       (apply str (preference-reply (:text params) wl em polarity-dic)))
  (GET "/script-chat" {params :params}
       (let [result (script-chat (keyword (:reply-key params)) (:text params) wl em polarity-dic (:sum-point params))]
         (if (= "true" (:debug params))
           (apply str (interpose "<BR>" result))
           (json/write-str result))))
  (POST "/feedback" request
        (let [r request
              _(println request)
              body (.readLine (reader (:body request)))
              fb (json/read-str body)]
          (println body)
          (println fb)
         (save-feedback fb)
         {:body "ok" :content-type :json :as :json}))
  (route/not-found "Not Found"))

(def app
  (-> #'app-routes
;;       (wrap-defaults site-defaults)
      (wrap-defaults (assoc-in site-defaults [:security :anti-forgery] false))
      handler/site
      (wrap-reload)
      ))

(defn -main [& [port]]
  (let [port (Integer. (or port (env :port) 3003))]
    (run-server app {:port port})))
