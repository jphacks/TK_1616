(ns feedback
  (:require
   [environ.core :refer [env]]
   [monger.core :as mg]
   [monger.collection :as mc]))


(def connection (mg/connect-via-uri "mongodb://klab:0nsayken9@ds143717.mlab.com:43717/comutrain"))
(def conn (:conn connection))
(def db   (:db connection))
(def document "feedback")

;;
(defn save-feedback [m]
  (mc/insert-and-return db document m))
;;                         {:sum 30 :preference "プロキシ" :userLineId "XXX"}))

;; (save-feedback)

