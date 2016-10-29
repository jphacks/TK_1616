(ns text.handler
  (:require [compojure.core :refer :all]
            [compojure.route :as route]
            [compojure.handler :as handler]
            [org.httpkit.server :refer [run-server]]
            [ring.middleware.reload :refer [wrap-reload]]
            [ring.middleware.defaults :refer [wrap-defaults site-defaults]]
            [environ.core :refer [env]]))

(defroutes app-routes
  (GET "/" [] "Hello World")
  (route/not-found "Not Found"))

(def app
  (-> #'app-routes
      (wrap-defaults site-defaults)
      handler/site
      (wrap-reload)
      ))


(defn -main [& [port]]
  (let [port (Integer. (or port (env :port) 3003))]
    (run-server app {:port port})))
