(ns feedback
  (:require
   [environ.core :refer [env]]
   [monger.core :as mg]
   [monger.collection :as mc]))


(def connection (mg/connect-via-uri (env :feedback)))
(def conn (:conn connection))
(def db   (:db connection))
(def document "feedback")

;;
(defn save-feedback [m]
  (mc/insert-and-return db document m))
;;                         {:sum 30 :preference "プロキシ" :userLineId "XXX"}))

;; (save-feedback)

